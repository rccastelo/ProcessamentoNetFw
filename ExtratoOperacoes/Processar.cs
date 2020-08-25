using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

class PessoaDados
{
  public PessoaDados(string nome, string documento, int qtdOperacoes, double saldo) {
    this.Nome = nome;
    this.Documento = documento;
    this.QtdOperacoes = qtdOperacoes;
    this.Saldo = saldo;
  }

  public string Nome;
  public string Documento;
  public int QtdOperacoes;
  public double Saldo;
}

class Processar
{
  static bool processarDados = false;
  static int qtdArquivos = 1;
  static int qtdArqsExiste = 0;
  static int qtdArqsProcs = 0;
  static string nomeArquivo = "arquivoSaida";
  static int qtdRegsLidos = 0;
  static object lockQtdReg = new object();
  static int qtdThreads = 5;
  static Dictionary<string, PessoaDados> lista;
  static object lockLista = new object();
  static CultureInfo cultura = CultureInfo.GetCultureInfo("en-US");

  static void TratarRegistro(string registro) 
  {
    /*
    campos[0] = Data dd/mm/aaaa (10)
    campos[1] = Hora hh:mm:ss.mmm (12)
    campos[2] = Documento 99999999999999 (14)
    campos[3] = Nome (50)
    campos[4] = Tipo Operação 999 (3)
    campos[5] = Valor Operação 9999999999.99 (13)
    campos[6] = Descrição Operação (30)
    */

    string[] campos = registro.Split(';');

    double saldo = 0;
    string nome = campos[3].Trim();
    int tipo = int.Parse(campos[4]);
    double valor = double.Parse(campos[5], cultura);

    lock (lockLista) {
      if (!lista.ContainsKey(nome)) {
        saldo = ((tipo == 1) ? (saldo + valor) : (saldo - valor));
        lista.Add(nome, new PessoaDados(nome, campos[2], 1, saldo));
      } else {
        PessoaDados pessoa = lista[nome];

        saldo = ((tipo == 1) ? (pessoa.Saldo + valor) : (pessoa.Saldo - valor));

        pessoa.Saldo = saldo;
        pessoa.QtdOperacoes++;
      }
    }
  }

  static void LerArquivoThread(object arquivo) {
    FileStream fs;
    StreamReader sr;
    string linha;
    string arqNome = arquivo.ToString();
    int arqRegsLidos = 0;

    Console.WriteLine(arqNome);

    fs = new FileStream(arqNome, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.SequentialScan);

    using (sr = new StreamReader(fs))
    {
      linha = sr.ReadLine();

      while (!String.IsNullOrEmpty(linha)) {
        TratarRegistro(linha);

        arqRegsLidos++;

        linha = sr.ReadLine();
      }
    }

    fs.Close();
    fs.Dispose();

    lock(lockQtdReg) qtdRegsLidos+=arqRegsLidos;
  }

  static void LerArquivo()
  {
    var sw = System.Diagnostics.Stopwatch.StartNew();

    Console.WriteLine("LerArquivo - inicio...");

    Thread[] lerArquivoThreads = new Thread[qtdThreads];
    int indiceArq = 1;

    while (qtdArqsProcs < qtdArquivos) {
      if (File.Exists($"{nomeArquivo}{indiceArq}.txt")) {
        for (int t = 0;((t < lerArquivoThreads.Length) && (t < qtdArquivos));t++) {
          if (lerArquivoThreads[t] == null) {
            Console.WriteLine($"Nova thread {t}: {nomeArquivo}{indiceArq}.txt");

            lerArquivoThreads[t] = new Thread(new ParameterizedThreadStart(LerArquivoThread));
            lerArquivoThreads[t].Start($"{nomeArquivo}{indiceArq}.txt");

            qtdArqsProcs++;
            qtdArqsExiste++;
            indiceArq++;
          } else {
            if (lerArquivoThreads[t].ThreadState != ThreadState.Running) {
              Console.WriteLine($"Thread existe {t}: {nomeArquivo}{indiceArq}.txt");

              lerArquivoThreads[t] = new Thread(new ParameterizedThreadStart(LerArquivoThread));
              lerArquivoThreads[t].Start($"{nomeArquivo}{indiceArq}.txt");

              qtdArqsProcs++;
              qtdArqsExiste++;
              indiceArq++;
            }
          }
        }
      } else {
        Console.WriteLine($"não existe {nomeArquivo}{indiceArq}.txt");

        qtdArqsProcs++;
        indiceArq++;
      }
    }

    for (int t = 0;t < lerArquivoThreads.Length;t++) {
      if (lerArquivoThreads[t] != null) {
        Console.WriteLine(lerArquivoThreads[t].ThreadState.ToString());

        lerArquivoThreads[t].Join();

        Console.WriteLine(lerArquivoThreads[t].ThreadState.ToString());
        Console.WriteLine($"terminou [{t}]");
      }
    }

    sw.Stop();
    var elapsed = sw.Elapsed.TotalMilliseconds;

    Console.WriteLine("LerArquivo: {0,6} ms", elapsed.ToString("N0", CultureInfo.InvariantCulture));
  }

   static void TratarParametros(string[] args) 
  {
    int valor;

    if (args.Length > 0) {
      for (int i = 0;i < args.Length;i++) {
        if (args[i] == "processar") {
          processarDados = true;
        } else {
          string[] partes = args[i].Split(':');

          if (partes.Length == 2) {
            switch (partes[0]) {
              case "qtdArq":
                if (int.TryParse(partes[1], out valor)) {
                  qtdArquivos = valor;
                  if ((qtdArquivos < 1) || (qtdArquivos > 30)) qtdArquivos = 1;
                }
                break;
              case "qtdThr":
                if (int.TryParse(partes[1], out valor)) {
                  qtdThreads = valor;
                  if ((qtdThreads < 1) || (qtdThreads > 10)) qtdThreads = 5;
                }
                break;
            }
          }
        }
      }
    } else {
      StringBuilder sb = new StringBuilder();

      sb.AppendLine("");
      sb.AppendLine("Parâmetros necessários...");
      sb.AppendLine("");
      sb.AppendLine("Habilitar processamento                  =>   processar");
      sb.AppendLine("Qtd Arquivos;  Padrão(1);  entre 1 e 30  =>   qtdArq:30");
      sb.AppendLine("Qtd Threads;  Padrão(5);  entre 1 e 10   =>   qtdThr:10");
      sb.AppendLine("");
      sb.AppendLine("Exemplo                                  =>   Processar.exe processar qtdArq:2 qtdThr:8");
      sb.AppendLine("");

      Console.WriteLine(sb.ToString());
    }
  }

 static void Principal(string[] args)
  {
    StringBuilder sb = new StringBuilder();

    TratarParametros(args);

    if (processarDados) {
      lista = new Dictionary<string, PessoaDados>();

      LerArquivo();

      sb.AppendLine("");
      sb.AppendLine($"Qtd Arquivos Informados: {qtdArqsProcs.ToString()}");
      sb.AppendLine($"Qtd Arquivos Existentes: {qtdArqsExiste.ToString()}");
      sb.AppendLine($"Qtd Registros: {qtdRegsLidos.ToString()}");
      sb.AppendLine("");
      sb.AppendLine($"Qtd Pessoas: {lista.Count}");
      sb.AppendLine("");

      Console.WriteLine(sb.ToString());

      foreach (PessoaDados pessoa in lista.Values) {
        Console.WriteLine($"{pessoa.Nome} - doc {pessoa.Documento} - qtd {pessoa.QtdOperacoes} - saldo {pessoa.Saldo}");
      }

      Console.WriteLine("");
    }
  }

  static void Main(string[] args)
  {
    try
    {
      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();

      var collCount = GC.CollectionCount(0);
      var sw = System.Diagnostics.Stopwatch.StartNew();

      Principal(args);

      sw.Stop();

      var elapsed = sw.Elapsed.TotalMilliseconds;
      collCount = GC.CollectionCount(0) - collCount;

      Console.WriteLine("Tempo: {0,6} ms (GCs={1,3})", elapsed.ToString("N0", CultureInfo.InvariantCulture), collCount);

      Environment.Exit(0);
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.ToString());
      
      Environment.Exit(-1);
    }
  }
}
