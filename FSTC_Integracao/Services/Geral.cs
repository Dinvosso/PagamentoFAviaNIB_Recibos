//using Primavera.Servico.EnvioEmails.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using ErpBS100;
using System.Globalization;




//using ErpBS100;
//using StdPlatBS100;

namespace FSTC_Integracao.Services
{
    public class Geral
    {
        //public string utilPrimav { get; set; }
        //public string IDNome { get; set; }
        public int instancia { get; set; }
        public string empresa { get; set; }
        public string utilizador { get; set; }
        public string password { get; set; }

        //public StdBSInterfPub plat { get; set; }


        public ErpBS bso { get; set; }
        public Geral() { }
        public Geral(dynamic BSO)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            this.bso = BSO;
            //AbrePlataforma(bso);
        }

        public Geral(int instancia, string empresa, string utilizador, string password)
        {
            this.instancia = instancia;
            this.empresa = empresa;
            this.utilizador = utilizador;
            this.password = password;
        }

        //public void AbrePlataforma(int tipoPlataforma, string Company, string User, string Password)
        //{
        //    StdBSConfApl objAplConf = new StdBSConfApl();
        //    StdPlatBS Plataforma = new StdPlatBS();
        //    ErpBS MotorLE = new ErpBS();

        //    EnumTipoPlataforma objTipoPlataforma;
        //    objTipoPlataforma = tipoPlataforma == 0 ? EnumTipoPlataforma.tpEmpresarial : EnumTipoPlataforma.tpProfissional;

        //    objAplConf.Instancia = "Default";
        //    objAplConf.AbvtApl = "ERP";
        //    objAplConf.PwdUtilizador = Password;
        //    objAplConf.Utilizador = User;
        //    objAplConf.LicVersaoMinima = "10.00";

        //    StdBETransaccao objStdTransac = new StdBETransaccao();

        //    try
        //    {
        //        Plataforma.AbrePlataformaEmpresa(Company, objStdTransac, objAplConf, objTipoPlataforma);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }

        //    if (Plataforma.Inicializada)
        //    {
        //        plat = Plataforma.InterfacePublico;

        //        MotorLE.AbreEmpresaTrabalho(objTipoPlataforma, Company, User, Password, objStdTransac, "Default");

        //        // Use this service to trigger the API events.
        //        ExtensibilityService service = new ExtensibilityService();

        //        // Suppress all message box events from the API.
        //        //Plataforma.ExtensibilityLogger.AllowInteractivity = false;
        //        service.Initialize(MotorLE);

        //        // Check if service is operational
        //        if (service.IsOperational)
        //        {
        //            // Inshore that all extensions are loaded.
        //            service.LoadExtensions();
        //        }

        //        bso = MotorLE;

        //    }

        //    //return engineInstance;
        //}

        //public static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    string assemblyFullName;

        //    System.Reflection.AssemblyName assemblyName;

        //    assemblyName = new System.Reflection.AssemblyName(args.Name);
        //    assemblyFullName = System.IO.Path.Combine(Environment.GetEnvironmentVariable("PERCURSOSGP100", EnvironmentVariableTarget.Machine), assemblyName.Name + ".dll");

        //    if (System.IO.File.Exists(assemblyFullName))
        //        return System.Reflection.Assembly.LoadFile(assemblyFullName);
        //    else
        //        return null;
        //}
        public string daConnectionString()
        {

            return new SqlConnectionStringBuilder()
            {
                DataSource = this.instancia.ToString(),
                InitialCatalog = this.empresa,
                UserID = this.utilizador,
                Password = this.password
            }.ConnectionString;
        }
        public void execCommand(OleDbConnection conn, string sqlText)
        {
            using (OleDbCommand command = new OleDbCommand())
            {

                command.Connection = conn;
                command.CommandText = sqlText;
                command.ExecuteNonQuery();

            }
        }


        public void AddLinhaVazia(DataTable dt)
        {
            try
            {
                DataRow dr = dt.NewRow();
                dt.Rows.InsertAt(dr, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable daListaTabela(string tabela, int maximo = 0, string campos = "", string filtros = "", string juncoes = "", string ordenacao = "")
        {
            string strSql = "select ";

            try
            {

                DataTable dt;

                if (maximo > 0) strSql = strSql + string.Format("Top {0} ", maximo);

                if ((campos.Length > 0))
                {
                    strSql = (strSql + campos);
                }
                else
                {
                    strSql = (strSql + " * ");
                }

                strSql = (strSql + (" from " + tabela));
                if ((juncoes.Length > 0))
                {
                    strSql = (strSql + (" " + juncoes));
                }

                if ((filtros.Length > 0))
                {
                    strSql = (strSql + (" where " + filtros));
                }

                if ((ordenacao.Length > 0))
                {
                    strSql = (strSql + (" order by " + ordenacao));
                }

                dt = this.ConsultaSQLDatatable(strSql);

                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public string GetParameter(string Name)
        {
            try
            {
                string result = "";
                string query = string.Format("select TOP 1 cdu_valor from TDU_Parametros where CDU_Parametro = '{0}' ", Name);
                DataTable dt = ConsultaSQLDatatable(query);

                if (dt.Rows.Count > 0)
                {
                    result = dt.Rows[0]["CDU_Valor"].ToString();
                }
                else
                {
                    new Exception("O parametro {0} não se encontra configurado na tabela de TDU_Parametros");
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public DataTable ConsultaSQLDatatable(string querySql)
        {

            try
            {
                //string conString = PriEngine.Platform.BaseDados.DaConnectionStringNET(PriEngine.Platform.BaseDados.DaNomeBDdaEmpresa(PriEngine.Engine.Contexto.CodEmp),
                //    "Default");

                DataTable dt = new DataTable();

                //SqlConnection con = new SqlConnection(conString);

                //SqlDataAdapter da = new SqlDataAdapter(querySql, con);

                //SqlCommandBuilder cb = new SqlCommandBuilder(da);

                //da.Fill(dt);
               dt= bso.ConsultaDataTable(querySql);
               return dt;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void escreveErro(string pastaLog, string name, string logMessage)
        {
            string ficheiro;
            try
            {
                ficheiro = pastaLog;

                using (StreamWriter w = File.AppendText(ficheiro + "\\" + string.Format("erro_{0}.log", name)))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void escreveLog(string pastaLog, string name, string logMessage)
        {
            string ficheiro;
            try
            {
                ficheiro = pastaLog;

                using (StreamWriter w = File.AppendText(ficheiro + "\\" + string.Format("log_{0}.log", name)))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        public void escreveLog(string logMessage)
        {
            string ficheiro;
            try
            {
                ficheiro = GetParameter("Log_PastaErro");

                using (StreamWriter w = File.AppendText(ficheiro + "\\" + string.Format("log_{0}.txt", DateTime.Now.ToString("ddMMyy"))))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.WriteLine("{0} - {1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), logMessage);
            }
            catch (Exception ex)
            {

            }
        }

        #region Excell
        public List<string> listaFolhaExcel(string caminhoExcell)
        {
            try
            {

                if (!File.Exists(caminhoExcell))
                {
                    throw new Exception("File does not exists");
                }

                List<string> listSheet = new List<string>();

                OleDbConnection conn = new OleDbConnection();

                string connString = ExcelConnection(caminhoExcell);

                conn.ConnectionString = connString;
                conn.Open();

                DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                foreach (DataRow drSheet in dt.Rows)
                {
                    listSheet.Add(drSheet["TABLE_NAME"].ToString());
                }

                return listSheet;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable CarregaListaExcel(string caminhoExcell, int sheetNumber, int linhaInicial)
        {

            DataTable dt;
            DataRow dr;

            try
            {

                if (!File.Exists(caminhoExcell))
                {
                    throw new Exception("File does not exists");
                }

                List<string> listSheet = new List<string>();

                OleDbConnection conn = new OleDbConnection();

                string connString = ExcelConnection(caminhoExcell);

                conn.ConnectionString = connString;
                conn.Open();

                var dtExcel = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                dt = ObtemDadosSheetExcell(conn, dtExcel.Rows[sheetNumber]["TABLE_NAME"].ToString());

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;

        }

        public DataTable ObtemDadosSheetExcell(OleDbConnection conn, string sheet)
        {
            try
            {
                DataTable dt = new DataTable();
                OleDbCommand cmd = new OleDbCommand();
                OleDbDataReader connReader;

                cmd.Connection = conn;
                cmd.CommandText = string.Format("Select * From [{0}]", sheet);

                connReader = cmd.ExecuteReader();
                dt.Load(connReader);

                connReader.Close();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    object id = dt.Rows[i];
                    if (id != null && !String.IsNullOrEmpty(id.ToString().Trim()))
                    {

                    }
                    else
                    {
                        dt.Rows[i].Delete();
                    }
                }

                return dt;

            }
            catch (Exception ex)
            {
                throw new Exception("<ObtemDadosSheetExcell>_" + ex.Message);
            }
        }
        public string ExcelConnection(string fileName)
        {
            string provider = "Microsoft.ACE.OLEDB.12.0";
            string dataSource = fileName;
            string extendProperties = "'Excel 12.0 Xml;HDR=YES'";

            provider = provider.Length > 0 ? provider : "Microsoft.ACE.OLEDB.12.0";

            string conn = string.Format(
                   @"Provider={0};
                    Data Source ={1};
                    Extended Properties={2}",
                   provider, dataSource, extendProperties
                );
            return conn;
        }

        #endregion

        public string GetEnderecos(string cliente, string Grupo, string Obs)
        {
            try
            {
                string enderecos = "";

                string[] strLine = Obs.Split(';');

                foreach (string line in strLine)
                {
                    if (line.Contains("@"))
                        enderecos += line + ";";
                }

                return enderecos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetEnderecos(string cliente, string tipoContacto)
        {
            try
            {
                string enderecos = "";
                DataTable stdLista;

                string strsql;

                strsql = "select LC.TipoEntidade,LC.Entidade,C.PrimeiroNome,isnull(C.Titulo,'') as 'Titulo',isnull(C.Email,'') as Email,";
                strsql = strsql + "isnull(C.EmailAssist,'') EmailAssist, isnull(C.EmailResid,'') as EmailResid ";
                strsql = strsql + "From LinhasContactoEntidades LC ";
                strsql = strsql + "inner join Contactos C on C.Id = LC.IDContacto ";
                strsql = strsql + string.Format("where LC.TipoEntidade ='C' and LC.Entidade = '{0}' and lc.TipoContacto = '{1}'", cliente, tipoContacto);

                stdLista = ConsultaSQLDatatable(strsql);

                foreach (DataRow dr in stdLista.Rows)
                {
                    var str_email = DaString(dr["Email"]);
                    var str_emailAssist = DaString(dr["EmailAssist"]);
                    var str_emailResid = DaString(dr["EmailResid"]);

                    if (str_email.Length > 0)
                        enderecos += str_email + ";";

                    if (str_emailAssist.Length > 0)
                        enderecos += str_emailAssist + ";";

                    if (str_emailResid.Length > 0)
                        enderecos += str_emailResid + ";";
                }

                return enderecos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LinkedList<string> GetEnderecosLista(string cliente, string Grupo, string Obs)
        {
            try
            {
                var lista = new LinkedList<string>();

                string enderecos = "";

                string[] strLine = Obs.Split(';');

                foreach (string line in strLine)
                {
                    if (line.Contains("@"))
                        lista.AddLast(line);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LinkedList<string> GetEnderecosLista(string cliente, string tipoContacto)
        {
            try
            {
                string enderecos = "";
                // Dim EstabDT As DataTable

                string strsql;

                // strsql = "select CDU_EMAIL1 , CDU_EMAIL2 , CDU_EMAIL3 from Clientes where cliente = '" & cliente & "'"
                DataTable stdLista;
                // Dim EstabDT As DataTable


                strsql = "select LC.TipoEntidade,LC.Entidade,C.PrimeiroNome,isnull(C.Titulo,'') as 'Titulo',isnull(C.Email,'') as Email,";
                strsql = strsql + "isnull(C.EmailAssist,'') EmailAssist, isnull(C.EmailResid,'') as EmailResid ";

                strsql = strsql + "From LinhasContactoEntidades LC ";
                strsql = strsql + "inner join Contactos C on C.Id = LC.IDContacto ";
                strsql = strsql + string.Format("where LC.TipoEntidade ='C' and LC.Entidade = '{0}' and lc.TipoContacto = '{1}'", cliente, tipoContacto);

                stdLista = ConsultaSQLDatatable(strsql);

                var lista = new LinkedList<string>();

                foreach (DataRow dr in stdLista.Rows)
                {
                    var str_email = DaString(dr["Email"]);
                    var str_emailAssist = DaString(dr["EmailAssist"]);
                    var str_emailResid = DaString(dr["EmailResid"]);

                    if (str_email.Length > 0)
                        lista.AddLast(str_email);

                    if (str_emailAssist.Length > 0)
                        lista.AddLast(str_emailAssist);

                    if (str_emailResid.Length > 0)
                        lista.AddLast(str_emailResid);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LinkedList<string> GetCCsLista(string utilizador)
        {
            try
            {
                DataTable EstabDT;
                var lista = new LinkedList<string>();

                string strsql;

                strsql =
                    "select CDU_CC1, CDU_CC2, CDU_CC3, CDU_CC4, CDU_CC5, CDU_CC6 from tdu_dadosemail Where CDU_UserPrim = '"
                    + utilizador + "'";

                EstabDT = ConsultaSQLDatatable(strsql);

                if (EstabDT.Rows.Count > 0)
                {
                    var dr = EstabDT.Rows[0];

                    string CDU_CC1 = DaString(dr["CDU_CC1"]);
                    string CDU_CC2 = DaString(dr["CDU_CC2"]);
                    string CDU_CC3 = DaString(dr["CDU_CC3"]);
                    string CDU_CC4 = DaString(dr["CDU_CC4"]);
                    string CDU_CC5 = DaString(dr["CDU_CC5"]);
                    string CDU_CC6 = DaString(dr["CDU_CC6"]);

                    if (CDU_CC1.Length > 0)
                        lista.AddLast(CDU_CC1);

                    if (CDU_CC2.Length > 0)
                        lista.AddLast(CDU_CC2);

                    if (CDU_CC3.Length > 0)
                        lista.AddLast(CDU_CC3);
                    if (CDU_CC4.Length > 0)
                        lista.AddLast(CDU_CC4);
                    if (CDU_CC5.Length > 0)
                        lista.AddLast(CDU_CC5);
                    if (CDU_CC6.Length > 0)
                        lista.AddLast(CDU_CC6);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string DaString(object s)
        {
            if (s is DBNull)
            {
                return "";
            }
            else
                return Convert.ToString(s);
        }

        public Int32 DaInt32(object s)
        {
            if (s is DBNull)
            {
                return 0;
            }
            else
                return Convert.ToInt32(s);
        }


        //public string GetfromStr()
        //{
        //    try
        //    {
        //        string queryStr = "select top (1) CDU_fromStr from TDU_DadosEmail Where CDU_UserPrim = '" + utilPrimav + "'";
        //        DataTable dt = ConsultaSQLDatatable(queryStr);
        //        //if (dt != null && dt.Rows.Count > 0)
        //        return dt.Rows[0]["CDU_fromStr"].ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public string GetsmtpServer()
        //{
        //    try
        //    {
        //        string queryStr = "select top (1) CDU_smtpServer from TDU_DadosEmail Where CDU_UserPrim = '" + utilPrimav + "'";
        //        DataTable dt = ConsultaSQLDatatable(queryStr);
        //        return dt.Rows[0]["CDU_smtpServer"].ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public int GetportaSmtpServer()
        //{

        //    try
        //    {
        //        string queryStr = "select top (1) CDU_portaSmtpServer from TDU_DadosEmail Where CDU_UserPrim = '" + utilPrimav + "'";
        //        DataTable dt = ConsultaSQLDatatable(queryStr);
        //        return Convert.ToInt32(dt.Rows[0]["CDU_portaSmtpServer"]);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public string GetuserSSL()
        //{

        //    try
        //    {
        //        string queryStr = "select top (1) CDU_userSSL from TDU_DadosEmail Where CDU_UserPrim = '" + utilPrimav + "'";
        //        DataTable dt = ConsultaSQLDatatable(queryStr);
        //        return dt.Rows[0]["CDU_userSSL"].ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public string GetpassUserSSL()
        //{

        //    try
        //    {
        //        string queryStr = "select top (1) CDU_passUserSSL from TDU_DadosEmail Where CDU_UserPrim = '" + utilPrimav + "'";
        //        DataTable dt = ConsultaSQLDatatable(queryStr);
        //        return dt.Rows[0]["CDU_passUserSSL"].ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public string DaString(object s, string formato)
        {
            if (s == null)
                return "";
            else
                return Convert.ToDateTime(s).ToString("yyyyMMdd");
        }

        public static Double DaDouble(object s)
        {
            if (s is DBNull)
            {
                return 0;
            }
            else
                return Convert.ToDouble(s);
        }

        public static double DaDouble(object s, string lingua)
        {
            if (s is DBNull)
            {
                return 0;
            }

            System.Globalization.CultureInfo EnglishCulture = new System.Globalization.CultureInfo(lingua);

            CultureInfo invC = CultureInfo.InvariantCulture;

            return Convert.ToDouble(s, EnglishCulture);

        }
        public static Decimal DaDecimal(object s)
        {
            if (s is DBNull)
            {
                return 0;
            }
            else
                return Convert.ToDecimal(s);
        }
        public static float DaFloat(object s)
        {
            if (s is DBNull)
            {
                return 0;
            }
            else
                return float.Parse(s.ToString());
        }


        public void escreveLog(string pastaLog, string name, string logMessage, bool separadaData = true)
        {
            string ficheiro;
            try
            {
                ficheiro = pastaLog;

                name = separadaData ? string.Format("{0}_{1}", name, DateTime.Now.ToString("yyMMdd")) : name;

                using (StreamWriter w = File.AppendText(ficheiro + "\\" + string.Format("erro_{0}.log", name)))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void escreveLog(string nome, string logMessage)
        {
            string ficheiro;
            try
            {
                ficheiro = GetParameter("PastaLog");

                using (StreamWriter w = File.AppendText(ficheiro + "\\" + string.Format("log_{0}.txt", nome)))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //throw new NotImplementedException();
        }

    }
}
