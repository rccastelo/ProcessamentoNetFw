using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

class Info
{
  public bool Existe;
  public int AbeIni;
  public int AbeFim;
  public int FecIni;
  public int FecFim;
  public int ConIni;
  public int ConFim;
  public int TamTag;
  public int TamSem;
  public string ConteudoTag;
  public string ConteudoSem;
}

class Concurso
{
  public int Numero;
  public string Data;
  public int Bola1;
  public int Bola2;
  public int Bola3;
  public int Bola4;
  public int Bola5;
  public int Bola6;
  public int Bola7;
  public int Bola8;
  public int Bola9;
  public int Bola10;
  public int Bola11;
  public int Bola12;
  public int Bola13;
  public int Bola14;
  public int Bola15;
  public double ArrecadacaoTotal;
  public int Ganha15;
  public string Cidade;
  public string UF;
  public int Ganha14;
  public int Ganha13;
  public int Ganha12;
  public int Ganha11;
  public double Valor15;
  public double Valor14;
  public double Valor13;
  public double Valor12;
  public double Valor11;
  public double Acumulado15;
  public double EstimativaPremio;
  public double AcumuladoEspecial;
}

class DezenaOrdem : IComparable<DezenaOrdem>
{
  public DezenaOrdem(int qtd, int dezena)
  {
    this.Qtd = qtd;
    this.Dezena = dezena;
  }

  public int CompareTo(DezenaOrdem dezenaOrdem)
  {
    if (this.Qtd > dezenaOrdem.Qtd) {
      return -1;
    } else if (this.Qtd < dezenaOrdem.Qtd) {
      return 1;
    } else {
      if (this.Dezena > dezenaOrdem.Dezena) {
        return -1;
      } else if (this.Dezena < dezenaOrdem.Dezena) {
        return 1;
      } else {
        return 0;
      }
    }
  }

  public int Qtd;
  public int Dezena;
}

class Lotofacil
{
  static bool processarDados = false;
  static string nomeArquivo = "lotofacil_resultados.htm";
  static string arquivoConteudo = null;
  static int qtdCabecalho = 0;
  static int qtdConcurso = 0;
  static int qtdMaisMunicipio = 0;
  static int qtdTD = 0;
  static Regex rxTD = new Regex(@"<\/td>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
  static CultureInfo cultura = CultureInfo.GetCultureInfo("pt-BR");
  static List<Concurso> concursos;
  static int[] dezenasQtd;
  static int dezenasQtdTotal;
  static SortedSet<DezenaOrdem> dezenaOrdemLista;
  static Random randomPub = new Random();

  static Info ObterTable(string conteudo) {
    Info info = new Info();

    info.AbeIni = conteudo.IndexOf("<table");
    // Console.WriteLine($"info.AbeIni: {info.AbeIni}");

    if(info.AbeIni >= 0) {
      info.AbeFim = conteudo.IndexOf(">", info.AbeIni + 1);
      // Console.WriteLine($"info.AbeFim: {info.AbeFim}");

      info.FecIni = conteudo.IndexOf("</table>");
      // Console.WriteLine($"info.FecIni: {info.FecIni}");
      info.FecFim = info.FecIni + 7;
      // Console.WriteLine($"info.FecFim: {info.FecFim}");

      info.ConIni = info.AbeFim + 1;
      // Console.WriteLine($"info.ConIni: {info.ConIni}");
      info.ConFim = info.FecIni - 1;
      // Console.WriteLine($"info.ConFim: {info.ConFim}");

      info.TamTag = info.FecFim - info.AbeIni + 1;
      // Console.WriteLine($"info.TamTag: {info.TamTag}");
      info.TamSem = info.FecIni - info.AbeFim - 1;
      // Console.WriteLine($"info.TamSem: {info.TamSem}");

      info.ConteudoTag = conteudo.Substring(info.AbeIni, info.TamTag);
      // Console.WriteLine($"info.ConteudoTag: {info.ConteudoTag}");
      info.ConteudoSem = conteudo.Substring(info.ConIni, info.TamSem);
      // Console.WriteLine($"info.ConteudoSem: {info.ConteudoSem}");

      info.Existe = true;
    }

    // Console.WriteLine($"info.Existe: {info.Existe}");

    return info;
  }

  static Info ObterTr(string conteudo) {
    Info info = new Info();

    info.AbeIni = conteudo.IndexOf("<tr");
    // Console.WriteLine($"info.AbeIni: {info.AbeIni}");

    if(info.AbeIni >= 0) {
      info.AbeFim = conteudo.IndexOf(">", info.AbeIni + 1);

      info.FecIni = conteudo.IndexOf("</tr>");
      info.FecFim = info.FecIni + 4;

      info.ConIni = info.AbeFim + 1;
      info.ConFim = info.FecIni - 1;

      info.TamTag = info.FecFim - info.AbeIni + 1;
      info.TamSem = info.FecIni - info.AbeFim - 1;

      info.ConteudoTag = conteudo.Substring(info.AbeIni, info.TamTag);
      info.ConteudoSem = conteudo.Substring(info.ConIni, info.TamSem);

      info.Existe = true;
    }

    // Console.WriteLine($"info.Existe: {info.Existe}");

    return info;
  }

  static Info ObterTd(string conteudo) {
    Info info = new Info();

    info.AbeIni = conteudo.IndexOf("<td");
    // Console.WriteLine($"info.AbeIni: {info.AbeIni}");

    if(info.AbeIni >= 0) {
      info.AbeFim = conteudo.IndexOf(">", info.AbeIni + 1);

      info.FecIni = conteudo.IndexOf("</td>");
      info.FecFim = info.FecIni + 4;

      info.ConIni = info.AbeFim + 1;
      info.ConFim = info.FecIni - 1;

      info.TamTag = info.FecFim - info.AbeIni + 1;
      info.TamSem = info.FecIni - info.AbeFim - 1;

      info.ConteudoTag = conteudo.Substring(info.AbeIni, info.TamTag);
      info.ConteudoSem = conteudo.Substring(info.ConIni, info.TamSem);

      info.Existe = true;
    }

    // Console.WriteLine($"info.Existe: {info.Existe}");

    return info;
  }

  static void TratarTR(string conteudo) 
  {
    string trConteudo = "";
    Info tdInfo;
    string[] tdCampos = new string[40];
    int tdIndice = 0;

    trConteudo = conteudo;

    qtdTD = rxTD.Matches(trConteudo).Count;

    if (trConteudo.Contains("</th>")) {
      // Cabeçalho
      qtdCabecalho++;


    } else if (qtdTD < 4) {
      // Mais de um municipio/estado
      qtdMaisMunicipio++;


    } else {
      // Concurso / Dados principais
      qtdConcurso++;

      // Campos
      tdInfo = ObterTd(trConteudo);

      while(tdInfo.Existe) {
        tdCampos[tdIndice] = tdInfo.ConteudoSem;
        // Console.WriteLine(tdCampos[tdIndice]);
        tdIndice++;

        trConteudo = trConteudo.Substring(tdInfo.TamTag);

        tdInfo = ObterTd(trConteudo);
      }

      // Criar objeto concurso e preencher
      Concurso concurso = new Concurso();

      // Console.WriteLine(tdCampos[0]);
      concurso.Numero = int.Parse(tdCampos[0], cultura); // 01 - Número concurso 
      // Console.WriteLine(tdCampos[1]);
      concurso.Data = tdCampos[1]; // 02 - Data sorteio
      // Console.WriteLine(tdCampos[2]);
      concurso.Bola1 = int.Parse(tdCampos[2], cultura); // 03 - Bola 1
      // Console.WriteLine(tdCampos[3]);
      concurso.Bola2 = int.Parse(tdCampos[3], cultura); // 04 - Bola 2 
      // Console.WriteLine(tdCampos[4]);
      concurso.Bola3 = int.Parse(tdCampos[4], cultura); // 05 - Bola 3 
      // Console.WriteLine(tdCampos[5]);
      concurso.Bola4 = int.Parse(tdCampos[5], cultura); // 06 - Bola 4 
      // Console.WriteLine(tdCampos[6]);
      concurso.Bola5 = int.Parse(tdCampos[6], cultura); // 07 - Bola 5 
      // Console.WriteLine(tdCampos[7]);
      concurso.Bola6 = int.Parse(tdCampos[7], cultura); // 08 - Bola 6 
      // Console.WriteLine(tdCampos[8]);
      concurso.Bola7 = int.Parse(tdCampos[8], cultura); // 09 - Bola 7 
      // Console.WriteLine(tdCampos[9]);
      concurso.Bola8 = int.Parse(tdCampos[9], cultura); // 10 - Bola 8 
      // Console.WriteLine(tdCampos[10]);
      concurso.Bola9 = int.Parse(tdCampos[10], cultura); // 11 - Bola 9 
      // Console.WriteLine(tdCampos[11]);
      concurso.Bola10 = int.Parse(tdCampos[11], cultura); // 12 - Bola 10 
      // Console.WriteLine(tdCampos[12]);
      concurso.Bola11 = int.Parse(tdCampos[12], cultura); // 13 - Bola 11
      // Console.WriteLine(tdCampos[13]);
      concurso.Bola12 = int.Parse(tdCampos[13], cultura); // 14 - Bola 12 
      // Console.WriteLine(tdCampos[14]);
      concurso.Bola13 = int.Parse(tdCampos[14], cultura); // 15 - Bola 13 
      // Console.WriteLine(tdCampos[15]);
      concurso.Bola14 = int.Parse(tdCampos[15], cultura); // 16 - Bola 14 
      // Console.WriteLine(tdCampos[16]);
      concurso.Bola15 = int.Parse(tdCampos[16], cultura); // 17 - Bola 15 
      // Console.WriteLine(tdCampos[17]);
      concurso.ArrecadacaoTotal = double.Parse(tdCampos[17], cultura); // 18 - Arrecadação total 
      // Console.WriteLine(tdCampos[18]);
      concurso.Ganha15 = int.Parse(tdCampos[18], cultura); // 19 - Ganhadores 15 números
      // Console.WriteLine(tdCampos[19]);
      concurso.Cidade = tdCampos[19]; // 20 - Cidade
      // Console.WriteLine(tdCampos[20]);
      concurso.UF = tdCampos[20]; // 21 - UF
      // Console.WriteLine(tdCampos[21]);
      concurso.Ganha14 = int.Parse(tdCampos[21], cultura); // 22 - Ganhadores 14 números
      // Console.WriteLine(tdCampos[22]);
      concurso.Ganha13 = int.Parse(tdCampos[22], cultura); // 23 - Ganhadores 13 números
      // Console.WriteLine(tdCampos[23]);
      concurso.Ganha12 = int.Parse(tdCampos[23], cultura); // 24 - Ganhadores 12 números
      // Console.WriteLine(tdCampos[24]);
      concurso.Ganha11 = int.Parse(tdCampos[24], cultura); // 25 - Ganhadores 11 números
      // Console.WriteLine(tdCampos[25]);
      concurso.Valor15 = double.Parse(tdCampos[25], cultura); // 26 - Valor rateio 15 números
      // Console.WriteLine(tdCampos[26]);
      concurso.Valor14 = double.Parse(tdCampos[26], cultura); // 27 - Valor rateio 14 números
      // Console.WriteLine(tdCampos[27]);
      concurso.Valor13 = double.Parse(tdCampos[27], cultura); // 28 - Valor rateio 13 números
      // Console.WriteLine(tdCampos[28]);
      concurso.Valor12 = double.Parse(tdCampos[28], cultura); // 29 - Valor rateio 12 números
      // Console.WriteLine(tdCampos[29]);
      concurso.Valor11 = double.Parse(tdCampos[29], cultura); // 30 - Valor rateio 11 números
      // Console.WriteLine(tdCampos[30]);
      concurso.Acumulado15 = double.Parse(tdCampos[30], cultura); // 31 - Acumulado 15 números
      // Console.WriteLine(tdCampos[31]);
      concurso.EstimativaPremio = double.Parse(tdCampos[31], cultura); // 32 - Estimativa prêmio
      // Console.WriteLine(concurso.EstimativaPremio);
      // Console.WriteLine(tdCampos[32]);
      concurso.AcumuladoEspecial = double.Parse(tdCampos[32], cultura); // 33 - Valor acumulado especial
      // Console.WriteLine(concurso.AcumuladoEspecial);

      concursos.Add(concurso);
    }
  }

  static void TratarArquivo()
  {
    var sw = System.Diagnostics.Stopwatch.StartNew();

    Console.WriteLine("TratarArquivo - inicio...");

    Info tableInfo;
    string tableSem = "";
    Info trInfo;

    if (!String.IsNullOrEmpty(arquivoConteudo)) {
      arquivoConteudo = arquivoConteudo.Replace("\r", "");
      arquivoConteudo = arquivoConteudo.Replace("\n", "");

      tableInfo = ObterTable(arquivoConteudo);

      if(tableInfo.Existe) {
        tableSem = tableInfo.ConteudoSem;

        trInfo = ObterTr(tableSem);

        while(trInfo.Existe) {
          TratarTR(trInfo.ConteudoSem);

          tableSem = tableSem.Substring(trInfo.TamTag);

          trInfo = ObterTr(tableSem);
        }
      }
    }

    sw.Stop();
    var elapsed = sw.Elapsed.TotalMilliseconds;

    Console.WriteLine("TratarArquivo: {0,6} ms", elapsed.ToString("N0", CultureInfo.InvariantCulture));
  }

  static void LerArquivo()
  {
    var sw = System.Diagnostics.Stopwatch.StartNew();

    Console.WriteLine("LerArquivo - inicio...");

    if (File.Exists(nomeArquivo)) {
      FileStream fs;
      StreamReader sr;

      Console.WriteLine(nomeArquivo);

      fs = new FileStream(nomeArquivo, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.SequentialScan);

      using (sr = new StreamReader(fs))
      {
        arquivoConteudo = sr.ReadToEnd();
      }

      fs.Close();
      fs.Dispose();
    } else {
      Console.WriteLine($"não existe {nomeArquivo}");
    }

    sw.Stop();
    var elapsed = sw.Elapsed.TotalMilliseconds;

    Console.WriteLine("LerArquivo: {0,6} ms", elapsed.ToString("N0", CultureInfo.InvariantCulture));
  }

  static void TratarParametros(string[] args) 
  {
    // int valor;

    if (args.Length > 0) {
      for (int i = 0;i < args.Length;i++) {
        if (args[i] == "processar") {
          processarDados = true;
        // } else {
        //   string[] partes = args[i].Split(':');

        //   if (partes.Length == 2) {
        //     switch (partes[0]) {
        //       // case "qtdArq":
        //       //   if (int.TryParse(partes[1], out valor)) {
        //       //     qtdArquivos = valor;
        //       //     if ((qtdArquivos < 1) || (qtdArquivos > 30)) qtdArquivos = 1;
        //       //   }
        //       //   break;
        //       // case "qtdThr":
        //       //   if (int.TryParse(partes[1], out valor)) {
        //       //     qtdThreads = valor;
        //       //     if ((qtdThreads < 1) || (qtdThreads > 10)) qtdThreads = 5;
        //       //   }
        //       //   break;
        //     }
        //   }
        }
      }
    } else {
      StringBuilder sb = new StringBuilder();

      sb.AppendLine("");
      sb.AppendLine("Parâmetros necessários...");
      sb.AppendLine("");
      sb.AppendLine("Habilitar processamento                  =>   processar");
      // sb.AppendLine("Qtd Arquivos;  Padrão(1);  entre 1 e 30  =>   qtdArq:30");
      // sb.AppendLine("Qtd Threads;  Padrão(5);  entre 1 e 10   =>   qtdThr:10");
      sb.AppendLine("");
      sb.AppendLine("Exemplo                                  =>   Lotofacil.exe processar");
      sb.AppendLine("");

      Console.WriteLine(sb.ToString());
    }
  }

  static void TratarInfos() {
    if(concursos != null) {
      foreach(Concurso c in concursos) {
        dezenasQtd[c.Bola1]++;
        dezenasQtd[c.Bola2]++;
        dezenasQtd[c.Bola3]++;
        dezenasQtd[c.Bola4]++;
        dezenasQtd[c.Bola5]++;
        dezenasQtd[c.Bola6]++;
        dezenasQtd[c.Bola7]++;
        dezenasQtd[c.Bola8]++;
        dezenasQtd[c.Bola9]++;
        dezenasQtd[c.Bola10]++;
        dezenasQtd[c.Bola11]++;
        dezenasQtd[c.Bola12]++;
        dezenasQtd[c.Bola13]++;
        dezenasQtd[c.Bola14]++;
        dezenasQtd[c.Bola15]++;
      }

      for(int i = 1;i <= 25;i++) {
        dezenasQtdTotal+=dezenasQtd[i];

        DezenaOrdem dezOrd = new DezenaOrdem(dezenasQtd[i], i);

        dezenaOrdemLista.Add(dezOrd);
      }
    }
  }

  static void Principal(string[] args)
  {
    dezenaOrdemLista = new SortedSet<DezenaOrdem>();
    StringBuilder sb = new StringBuilder();
    int contador = 0;
    int tirar;
    int colocar;

    Console.WriteLine("");

    TratarParametros(args);

    if (processarDados) {
      concursos = new List<Concurso>();
      dezenasQtd = new int[100];

      LerArquivo();

      TratarArquivo();

      TratarInfos();

      sb.AppendLine("");
      sb.AppendLine($"qtdCabecalho: {qtdCabecalho}");
      sb.AppendLine($"qtdConcurso: {qtdConcurso}");
      sb.AppendLine($"qtdMaisMunicipio: {qtdMaisMunicipio}");
      sb.AppendLine("");

      if(concursos != null) {
        sb.AppendLine($"concursos: {concursos.Count}");
        sb.AppendLine("");

        for(int i = 1;i <= 25;i++) {
          sb.AppendLine($"Dezena {i} = {dezenasQtd[i]}");
        }

        sb.AppendLine("");
        sb.AppendLine($"Total de dezenas [{dezenasQtdTotal}] = [{concursos.Count * 15}] Concursos * 15");
        sb.AppendLine("");
        sb.AppendLine("Ordenado (numeros que mais sairam)...");
        sb.AppendLine("");

        foreach(DezenaOrdem dezOrd in dezenaOrdemLista) {
          sb.AppendLine($"Dezena {dezOrd.Dezena} = {dezOrd.Qtd}");
        }

        sb.AppendLine("");
        sb.AppendLine("Jogos...");
        sb.AppendLine("");

        int[] dezs = new int[25];

        contador = 0;
        foreach(DezenaOrdem dezOrd in dezenaOrdemLista) {
          dezs[contador] = dezOrd.Dezena;
          contador++;
        }

        sb.Append("Jogo 001   |");
        for(int i = 0;i < 15;i++) {
          sb.Append($"  {dezs[i].ToString().PadLeft(2, ' ')}  |");
        }
        sb.AppendLine("");

        for(int j = 2;j <= 3;j++) {
          tirar = randomPub.Next(15);
          colocar = randomPub.Next(10) + 15;
          sb.Append($"Jogo {j.ToString().PadLeft(3, '0')}   |");

          for(int i = 0;i < 15;i++) {
            if(tirar != i) sb.Append($"  {dezs[i].ToString().PadLeft(2, ' ')}  |");
          }
          sb.Append($"  {dezs[colocar].ToString().PadLeft(2, ' ')}  |");
          sb.AppendLine("");
        }

        sb.AppendLine("");
      }

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
