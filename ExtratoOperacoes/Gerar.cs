using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;

public class Gerar
{
  static Random randomPub = new Random();

  static bool gerarOperacoes = false;
  static int qtdTipoDebito = 0;
  static int qtdOperacoes = 10;
  static int qtdOpGeradas = 0;
  static int qtdNome = 100;
  static int qtdMeio = 50;
  static int qtdSobre = 80;
  static int qtdPessoas = 2000;
  static int qtdTipoOperacao = 50;
  static int qtdIteracoes = 0;
  static int qtdArquivos = 1;

  static string[] listaPrimeiroNome = new string[qtdNome];
  static string[] listaNomeMeio = new string[qtdMeio];
  static string[] listaSobrenome = new string[qtdSobre];
  static HashSet<string> conjNomeCompleto;
  static string[,] listaNomeDocumento;
  static string[,] listaTipoOperacao;

  static void CarregarTipoOperacao()
  {
    var sw = System.Diagnostics.Stopwatch.StartNew();

    Console.WriteLine("CarregarTipoOperacao - inicio...");

    FileStream fs;
    StreamReader sr;
    string linha = null;
    string[] partes = null;
    int cont = 0;

    fs = new FileStream(@"tipooperacao.txt", FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.SequentialScan);

    using (sr = new StreamReader(fs))
    {
      linha = sr.ReadLine();

      while ((!String.IsNullOrEmpty(linha)) && (cont < qtdTipoOperacao))
      {
        partes = linha.Split(';');
        listaTipoOperacao[cont,0] = partes[0];
        listaTipoOperacao[cont,1] = partes[1];
        cont++;
        linha = sr.ReadLine();
      }
    }

    fs.Close();
    fs.Dispose();

    sw.Stop();
    var elapsed = sw.Elapsed.TotalMilliseconds;

    Console.WriteLine("CarregarTipoOperacao: {0,6} ms", elapsed.ToString("N0", CultureInfo.InvariantCulture));
  }

  static void CarregarNomes()
  {
    var sw = System.Diagnostics.Stopwatch.StartNew();

    Console.WriteLine("CarregarNomes - inicio...");

    FileStream fs;
    StreamReader sr;
    string linha = null;
    int cont = 0;

    fs = new FileStream(@"nomes.txt", FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.SequentialScan);

    using (sr = new StreamReader(fs))
    {
      linha = sr.ReadLine();

      while ((!String.IsNullOrEmpty(linha)) && (cont < qtdNome))
      {
        listaPrimeiroNome[cont] = linha;
        cont++;
        linha = sr.ReadLine();
      }
    }

    fs.Close();
    fs.Dispose();

    sw.Stop();
    var elapsed = sw.Elapsed.TotalMilliseconds;

    Console.WriteLine("CarregarNomes: {0,6} ms", elapsed.ToString("N0", CultureInfo.InvariantCulture));
  }

  static void CarregarNomesMeio()
  {
    var sw = System.Diagnostics.Stopwatch.StartNew();

    Console.WriteLine("CarregarNomesMeio - inicio...");

    FileStream fs;
    StreamReader sr;
    string linha = null;
    int cont = 0;

    fs = new FileStream(@"nomesmeio.txt", FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.SequentialScan);

    using (sr = new StreamReader(fs))
    {
      linha = sr.ReadLine();

      while ((!String.IsNullOrEmpty(linha)) && (cont < qtdMeio))
      {
        listaNomeMeio[cont] = linha;
        cont++;
        linha = sr.ReadLine();
      }
    }

    fs.Close();
    fs.Dispose();

    sw.Stop();
    var elapsed = sw.Elapsed.TotalMilliseconds;

    Console.WriteLine("CarregarNomesMeio: {0,6} ms", elapsed.ToString("N0", CultureInfo.InvariantCulture));
  }

  static void CarregarSobrenomes()
  {
    var sw = System.Diagnostics.Stopwatch.StartNew();

    Console.WriteLine("CarregarSobrenomes - inicio...");

    FileStream fs;
    StreamReader sr;
    string linha = null;
    int cont = 0;

    fs = new FileStream(@"sobrenomes.txt", FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.SequentialScan);

    using (sr = new StreamReader(fs))
    {
      linha = sr.ReadLine();

      while ((!String.IsNullOrEmpty(linha)) && (cont < qtdSobre))
      {
        listaSobrenome[cont] = linha;
        cont++;
        linha = sr.ReadLine();
      }
    }

    fs.Close();
    fs.Dispose();

    sw.Stop();
    var elapsed = sw.Elapsed.TotalMilliseconds;

    Console.WriteLine("CarregarSobrenomes: {0,6} ms", elapsed.ToString("N0", CultureInfo.InvariantCulture));
  }

  static void GerarNomeCompleto()
  {
    var sw = System.Diagnostics.Stopwatch.StartNew();

    Console.WriteLine("GerarNomeCompleto - inicio...");

    int indiceNome = 0;
    int indiceMeio = 0;
    int indiceSobre = 0;
    int temMeio = 0;
    bool gerar = true;
    StringBuilder sb = new StringBuilder();

    while (gerar)
    {
      qtdIteracoes++;

      sb.Clear();

      temMeio = randomPub.Next(2);
      if (temMeio == 1) indiceMeio = randomPub.Next(qtdMeio);
      indiceNome = randomPub.Next(qtdNome);
      indiceSobre = randomPub.Next(qtdSobre);

      sb.Append(listaPrimeiroNome[indiceNome]);
      if (temMeio == 1) {
        sb.Append(" ");
        sb.Append(listaNomeMeio[indiceMeio]);
      }
      sb.Append(" ");
      sb.Append(listaSobrenome[indiceSobre]);

      if (!conjNomeCompleto.Contains(sb.ToString())) conjNomeCompleto.Add(sb.ToString());

      if (conjNomeCompleto.Count >= qtdPessoas) gerar = false;
    }

    sw.Stop();
    var elapsed = sw.Elapsed.TotalMilliseconds;

    Console.WriteLine("GerarNomeCompleto: {0,6} ms", elapsed.ToString("N0", CultureInfo.InvariantCulture));
  }

  static String GerarCpf()
  {
    int soma = 0, resto = 0;
    int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

    string semente = randomPub.Next(100000000, 999999999).ToString();

    for (int i = 0; i < 9; i++)
      soma += int.Parse(semente[i].ToString()) * multiplicador1[i];

    resto = soma % 11;
    if (resto < 2)
      resto = 0;
    else
      resto = 11 - resto;

    semente = semente + resto;
    soma = 0;

    for (int i = 0; i < 10; i++)
      soma += int.Parse(semente[i].ToString()) * multiplicador2[i];

    resto = soma % 11;

    if (resto < 2)
      resto = 0;
    else
      resto = 11 - resto;

    semente = semente + resto;
    return semente;
  }

  static void CarregarNomeCpf() {
    var sw = System.Diagnostics.Stopwatch.StartNew();

    Console.WriteLine("CarregarNomeCpf - inicio...");

    int indiceNome = 0;

    foreach (string nome in conjNomeCompleto)
    {
      listaNomeDocumento[indiceNome,0] = nome;
      listaNomeDocumento[indiceNome,1] = GerarCpf();

      indiceNome++;
    }

    sw.Stop();
    var elapsed = sw.Elapsed.TotalMilliseconds;

    Console.WriteLine("CarregarNomeCpf: {0,6} ms", elapsed.ToString("N0", CultureInfo.InvariantCulture));
  }

  static string GerarHora()
  {
    StringBuilder sb = new StringBuilder();

    sb.Append(randomPub.Next(24).ToString("00"));
    sb.Append(":");
    sb.Append(randomPub.Next(60).ToString("00"));
    sb.Append(":");
    sb.Append(randomPub.Next(60).ToString("00"));
    sb.Append(".");
    sb.Append(randomPub.Next(1000).ToString("000"));

    return sb.ToString();
  }

  static string GerarData()
  {
    StringBuilder sb = new StringBuilder();
    int dia;
    int mes = randomPub.Next(1, 13);

    switch (mes)
    {
      case 2:
        dia = randomPub.Next(1, 29);
        break;
      case 1:
      case 3:
      case 5:
      case 7:
      case 8:
      case 10:
      case 12:
        dia = randomPub.Next(1, 32);
        break;
      default:
        dia = randomPub.Next(1, 31);
        break;
    };

    sb.Append(randomPub.Next(2000, 2021).ToString("0000"));
    sb.Append("-");
    sb.Append(mes.ToString("00"));
    sb.Append("-");
    sb.Append(dia.ToString("00"));

    return sb.ToString();
  }

  static string GerarValor() {
    int range = randomPub.Next(0,20);
    int valDec = randomPub.Next(0,100);
    int valInt = 0;
    
    if (range == 19) {
      valInt = randomPub.Next(1,1000001);
    } else if (range == 18) {
      valInt = randomPub.Next(1,100001);
    } else if ((range == 17) || (range == 16)) {
      valInt = randomPub.Next(1,10001);
    } else if ((range <= 15) && (range >= 10)) {
      valInt = randomPub.Next(1,101);
    } else {
      valInt = randomPub.Next(1,1001);
    }

    StringBuilder strVal = new StringBuilder();

    strVal.Append(valInt.ToString().PadLeft(10, '0'));
    strVal.Append(".");
    strVal.Append(valDec.ToString().PadLeft(2, '0'));

    return strVal.ToString();
  }

  static void GerarOperacoes()
  {
    var sw = System.Diagnostics.Stopwatch.StartNew();

    Console.WriteLine("GerarOperacoes - inicio...");

    StringBuilder sb = new StringBuilder();
    int indice = 0;
    int tipoOp = 0;
    FileStream fst;
    StreamWriter swr;

    for (int a = 1;a <= qtdArquivos;a++) {
      fst = new FileStream($"arquivoSaida{a}.txt", FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan);

      using (swr = new StreamWriter(fst))
      {
        for (int i = 0; i < qtdOperacoes; i++)
        {
          indice = randomPub.Next(qtdPessoas);
          tipoOp = randomPub.Next(0,qtdTipoOperacao);

          qtdOpGeradas++;
          if (listaTipoOperacao[tipoOp,1] == "D") qtdTipoDebito++;

          sb.Clear();
          sb.Append(GerarData());
          sb.Append(";");
          sb.Append(GerarHora());
          sb.Append(";");
          sb.Append(listaNomeDocumento[indice,1].PadLeft(14, '0'));
          sb.Append(";");
          sb.Append(listaNomeDocumento[indice,0].PadRight(50, ' '));
          sb.Append(";");
          sb.Append((listaTipoOperacao[tipoOp,1] == "C") ? "001" : "002");
          sb.Append(";");
          sb.Append(GerarValor());
          sb.Append(";");
          sb.Append(listaTipoOperacao[tipoOp,0].PadRight(30, ' '));
          
          swr.WriteLine(sb.ToString());
        }
      }

      fst.Close();
      fst.Dispose();
    }

    sw.Stop();
    var elapsed = sw.Elapsed.TotalMilliseconds;

    Console.WriteLine("GerarOperacoes: {0,6} ms", elapsed.ToString("N0", CultureInfo.InvariantCulture));
  }

  static void TratarParametros(string[] args) 
  {
    int valor;

    if (args.Length > 0) {
      for (int i = 0;i < args.Length;i++) {
        if (args[i] == "gerar") {
          gerarOperacoes = true;
        } else {
          string[] partes = args[i].Split(':');

          if (partes.Length == 2) {
            switch (partes[0]) {
              case "qtdOpe":
                if (int.TryParse(partes[1], out valor)) {
                  qtdOperacoes = valor;
                  if ((qtdOperacoes < 1) || (qtdOperacoes > 2000000)) qtdOperacoes = 10;
                }
                break;
              case "qtdArq":
                if (int.TryParse(partes[1], out valor)) {
                  qtdArquivos = valor;
                  if ((qtdArquivos < 1) || (qtdArquivos > 20)) qtdArquivos = 1;
                }
                break;
              case "qtdPes":
                if (int.TryParse(partes[1], out valor)) {
                  qtdPessoas = valor;
                  if ((qtdPessoas < 1) || (qtdPessoas > 5000)) qtdPessoas = 10;
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
      sb.AppendLine("Habilitar geração                         =>   gerar");
      sb.AppendLine("Qtd Operações; Padrão(10); entre 1 e 2M   =>   qtdOpe:200");
      sb.AppendLine("Qtd Arquivos;  Padrão(1);  entre 1 e 20   =>   qtdArq:30");
      sb.AppendLine("Qtd Pessoas;   Padrão(10); entre 1 e 5000 =>   qtdPes:20");
      sb.AppendLine("");
      sb.AppendLine("Exemplo                                  =>   Gerar.exe gerar qtdOpe:150 qtdArq:2 qtdPes:35");
      sb.AppendLine("");

      Console.WriteLine(sb.ToString());
    }
  }

  static void Principal(string[] args)
  {
    StringBuilder sb = new StringBuilder();

    TratarParametros(args);

    if (gerarOperacoes) {
      conjNomeCompleto = new HashSet<string>();
      listaNomeDocumento = new string[qtdPessoas,2];
      listaTipoOperacao = new string[qtdTipoOperacao,2];
  
      CarregarNomes();
      CarregarNomesMeio();
      CarregarSobrenomes();
      GerarNomeCompleto();
      CarregarNomeCpf();
      CarregarTipoOperacao();

      Console.WriteLine("");

      GerarOperacoes();

      sb.AppendLine("");
      sb.Append("Iterações Nome Completo: ");
      sb.AppendLine(qtdIteracoes.ToString());
      sb.Append("Geração Nome Completo: ");
      sb.AppendLine(listaNomeDocumento.GetLength(0).ToString());
      sb.AppendLine("");
      sb.Append("Operações geradas: ");
      sb.AppendLine(qtdOpGeradas.ToString());
      sb.Append("Operações de crédito + : ");
      sb.Append(qtdOpGeradas - qtdTipoDebito);
      sb.AppendLine("");
      sb.Append("Operações de débito - : ");
      sb.Append(qtdTipoDebito);
      sb.AppendLine("");

      Console.WriteLine(sb.ToString());
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
