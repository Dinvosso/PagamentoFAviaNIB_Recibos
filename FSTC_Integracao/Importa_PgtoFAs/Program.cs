using Integracao_recebimentos_bancos;
using Integracao_recebimentos_bancos.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao_recebimentos_bancos.Engine

{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            Console.WriteLine("Starting Application...");

            GestaoPagamentoServicos cls;

            try
            {
                int tipoPlataforma = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Primavera_Instance")); ;
                string empresa = ConfigurationManager.AppSettings.Get("Primavera_Company");
                string utilizador = ConfigurationManager.AppSettings.Get("Primavera_User");
                string password = ConfigurationManager.AppSettings.Get("Primavera_Password");

                PriEngine.CreatContext(tipoPlataforma, empresa, utilizador, password);

                cls = new GestaoPagamentoServicos(tipoPlataforma, empresa, utilizador, password);
                cls.ValidaPagamentoViaNIB();

                // cls.Dispose();
            }
            catch (Exception ex)
            {
                EscreveErro(ConfigurationManager.AppSettings.Get("Erro_Folder"), "PagServicos", ex.Message);
            }
        }

        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyFullName;

            System.Reflection.AssemblyName assemblyName;

            assemblyName = new System.Reflection.AssemblyName(args.Name);
            assemblyFullName = System.IO.Path.Combine(Environment.GetEnvironmentVariable("PERCURSOSGP100", EnvironmentVariableTarget.Machine), assemblyName.Name + ".dll");

            if (System.IO.File.Exists(assemblyFullName))
                return System.Reflection.Assembly.LoadFile(assemblyFullName);
            else
                return null;
        }

        #region Log's
        public static void EscreveErro(string pastaLog, string name, string logMessage)
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

        public static void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.WriteLine("{0} - {1}", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), logMessage);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

    }
}
