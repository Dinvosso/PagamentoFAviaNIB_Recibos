
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.IO;
using System.Globalization;
using CctBE100;
using TesBE100;
using System.Configuration;
using Integracao_recebimentos_bancos.Engine;
namespace Integracao_recebimentos_bancos
{
    public class GestaoPagamentoServicos : Geral
    {


        public string api_host { get; set; }
        public string api_token { get; set; }

        public string api_key { get; set; }
        public string nib_api_key { get; set; }
        public string entity_uuid { get; set; }

        public string logFolder { get; set; }

        public string errorFolder { get; set; }

        public GestaoPagamentoServicos() : base()
        {
            try
            {
                api_host = GetParameter("Servicos_Host");
                api_token = GetParameter("Servicos_Token");
                api_key = GetParameter("Servicos_APIKEY");
                nib_api_key = GetParameter("Servicos_NIB_APIKEY");
                entity_uuid = GetParameter("Servicos_ENTITY_UUID");
                logFolder = ConfigurationManager.AppSettings.Get("Log_Folder");
                errorFolder = ConfigurationManager.AppSettings.Get("Erro_Folder");


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public GestaoPagamentoServicos(int tipoPlataforma, string strEmpresa, string strUtilizador, string strPassword)
            : base(tipoPlataforma, strEmpresa, strUtilizador, strPassword)
        {
            try
            {
                api_host = GetParameter("Servicos_Host");
                api_token = GetParameter("Servicos_Token");
                api_key = GetParameter("Servicos_APIKEY");
                nib_api_key = GetParameter("Servicos_NIB_APIKEY");
                entity_uuid = GetParameter("Servicos_ENTITY_UUID");
                logFolder = ConfigurationManager.AppSettings.Get("Log_Folder");
                errorFolder = ConfigurationManager.AppSettings.Get("Erro_Folder");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetToken()
        {
            try
            {
                string token = "";

                HttpClient client;
                client = new HttpClient();
                // Update port # in the following line.
                //client.BaseAddress = new Uri("http://agregador.incm.gov.mz/api/login");

                client.BaseAddress = new Uri(api_host);
                client.Timeout = new TimeSpan(840000);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                //// Create a new product
                //User user = new User
                //{
                //    username = "delmira.invosso@incentea.com",
                //    password = ""
                //};

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string Get_XSignatureI(string api_key, double amount, string invoice, string entity_uuid)
        {
            var text = string.Format("{0}{1}{2}{3}", api_key, amount, invoice, entity_uuid);

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private string Get_XSignatureI(string api_key, string reference, string entity_uuid)
        {
            var text = string.Format("{0}{1}{2}", api_key, reference, entity_uuid);

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        private string Get_XSignatureI(string api_key, string entity_uuid)
        {
            var text = string.Format("{0}{1}", api_key, entity_uuid);

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public CctBEDocumentoLiq CriaCabecalhoLiquidacao(String TipoEntidade, String Entidade, String TipoDocLiq, DateTime Data,
            String ReferenciaMB, String IdTransBMEPS, string ContaBancaria = "", string MovimentoBancario = "",
            int internalInvoiceNumber = 0)
        {
            CctBEDocumentoLiq objDocLiquidacao;

            try
            {
                objDocLiquidacao = new CctBEDocumentoLiq();
                objDocLiquidacao.TipoEntidade = TipoEntidade;
                objDocLiquidacao.Entidade = Entidade;

                objDocLiquidacao.Tipodoc = TipoDocLiq;
                objDocLiquidacao.Serie = PriEngine.Engine.Base.Series.DaSerieDefeito("M", TipoDocLiq, Data);

                objDocLiquidacao.Moeda = PriEngine.Engine.Contexto.MoedaBase;
                objDocLiquidacao.Utilizador = PriEngine.Engine.Contexto.UtilizadorActual;

                objDocLiquidacao.CamposUtil["CDU_ReferenciaPag"].Valor = ReferenciaMB;
                objDocLiquidacao.CamposUtil["CDU_EntidadePag"].Valor = IdTransBMEPS;
                objDocLiquidacao.CamposUtil["CDU_InternalInvoiceNumber"].Valor = internalInvoiceNumber;

                PriEngine.Engine.PagamentosRecebimentos.Liquidacoes.PreencheDadosRelacionados(objDocLiquidacao);

                objDocLiquidacao.DataDoc = Data;
                objDocLiquidacao.DataIntroducao = Data;

                objDocLiquidacao.ModoPag = MovimentoBancario;
                objDocLiquidacao.ContaBancaria = ContaBancaria;

            }
            catch (Exception ex)
            {
                objDocLiquidacao = null;
                escreveErro(errorFolder, "<CriaCabecalhoLiquidacaoViaRer>", ex.Message);
            }


            return objDocLiquidacao;

        }

        public CctBEDocumentoLiq CriaCabecalhoLiquidacaoViaNIB(String TipoEntidade, String Entidade, String TipoDocLiq, DateTime Data,
           String nib, string refOperacaoB, bool cdu_Pago, string ContaBancaria = "", string MovimentoBancario = "")
        {
            CctBEDocumentoLiq objDocLiquidacao;

            try
            {
                objDocLiquidacao = new CctBEDocumentoLiq();
                objDocLiquidacao.TipoEntidade = TipoEntidade;
                objDocLiquidacao.Entidade = Entidade;

                objDocLiquidacao.Tipodoc = TipoDocLiq;
                objDocLiquidacao.Serie = PriEngine.Engine.Base.Series.DaSerieDefeito("M", TipoDocLiq, Data);

                objDocLiquidacao.Moeda = PriEngine.Engine.Contexto.MoedaBase;
                objDocLiquidacao.Utilizador = PriEngine.Engine.Contexto.UtilizadorActual;

                objDocLiquidacao.CamposUtil["cdu_NrTransacao"].Valor = refOperacaoB;

                // objDocLiquidacao.get_CamposUtil().get_Item("CDU_EntidadePag").Valor = IdTransBMEPS;
                //objDocLiquidacao.get_CamposUtil().get_Item("CDU_InternalInvoiceNumber").Valor = internalInvoiceNumber;

                PriEngine.Engine.PagamentosRecebimentos.Liquidacoes.PreencheDadosRelacionados(objDocLiquidacao);

                objDocLiquidacao.DataDoc = Data;
                objDocLiquidacao.DataIntroducao = Data;

                objDocLiquidacao.ModoPag = MovimentoBancario;
                objDocLiquidacao.ContaBancaria = ContaBancaria;

            }
            catch (Exception ex)
            {
                objDocLiquidacao = null;
                escreveErro(errorFolder, "<CriaCabecalhoLiquidacaoViaNIB>", ex.Message);
            }


            return objDocLiquidacao;

        }

        public bool GravaLigacaoBancos(string TipoEntidade, string Entidade, string Moeda, double Valor,
            DateTime DataDoc, string IdDoc, string RefATM,
            string strTipoDocTes, string strContaBancaria, string strMovBancario, bool DocumentoAdiantamento = false)
        {
            try
            {
                string strSerieDocTes, strErro = "", strAvisos = "";


                TesBEDocumentoTesouraria documentoTesouraria = new TesBEDocumentoTesouraria();
                strSerieDocTes = PriEngine.Engine.Base.Series.DaSerieDefeito("B", strTipoDocTes, DataDoc);

                //'SE O DOCUMENTO DE LOGISTICA FOR UMA ADIANTAMENTO
                if (DocumentoAdiantamento)
                    Valor = Math.Abs(Valor);

                documentoTesouraria.EmModoEdicao = false;
                documentoTesouraria.ModuloOrigem = "M";
                documentoTesouraria.Filial = "000";
                documentoTesouraria.TipoLancamento = "000";
                documentoTesouraria.Tipodoc = strTipoDocTes;
                documentoTesouraria.Serie = strSerieDocTes;
                documentoTesouraria.Data = DataDoc;
                documentoTesouraria.DataIntroducao = DataDoc;
                documentoTesouraria.TipoEntidade = TipoEntidade;
                documentoTesouraria.Entidade = Entidade;
                documentoTesouraria.ContaOrigem = strContaBancaria;
                documentoTesouraria.Moeda = Moeda;
                documentoTesouraria.Cambio = PriEngine.Engine.Contexto.MBaseCambioCompra;
                documentoTesouraria.CambioMBase = PriEngine.Engine.Contexto.MBaseCambioCompra;
                documentoTesouraria.CambioMAlt = PriEngine.Engine.Contexto.MAltCambioCompra;
                documentoTesouraria.IdDocOrigem = IdDoc;
                documentoTesouraria.AgrupaMovimentos = false;

                PriEngine.Engine.Tesouraria.Documentos.AdicionaLinha(documentoTesouraria, strMovBancario,
                    strContaBancaria, PriEngine.Engine.Contexto.MoedaBase, Valor, TipoEntidade, Entidade);

                var linhas = documentoTesouraria.Linhas;
                int count = 0;
                foreach (TesBELinhaDocTesouraria linha in linhas)
                {
                    if (count == 0)
                    {
                        linha.DataMovimento = DataDoc;
                        linha.DataValor = DataDoc;
                        linha.Cambio = PriEngine.Engine.Contexto.MBaseCambioCompra;
                        linha.CambioMBase = PriEngine.Engine.Contexto.MBaseCambioCompra;
                        linha.CambioMAlt = PriEngine.Engine.Contexto.MAltCambioCompra;
                        linha.Descricao = "Ref. " + RefATM;

                    }
                    count++;
                }


                PriEngine.Engine.Tesouraria.Documentos.Actualiza(documentoTesouraria, strAvisos);

                return true;

            }

            catch (Exception ex)
            {
                escreveErro(errorFolder, "GravaLigacaoBancos_referencia", RefATM + "-" + ex.Message);
                return false;
            }



        }
        public void GravaLigacaoBancosNib(string TipoEntidade, string Entidade, string Moeda, double Valor,
                DateTime DataDoc, string IdDoc, string RefATM, string strTipoDocTes, string strContaBancaria, string strMovBancario, bool DocumentoAdiantamento = false)
        {
            try
            {
                string strSerieDocTes, strErro = "", strAvisos = "";
                string Rubrica = "", Descricao = "";
                bool PreDatado = false;

                TesBEDocumentoTesouraria documentoTesouraria = new TesBEDocumentoTesouraria();
                strSerieDocTes = PriEngine.Engine.Base.Series.DaSerieDefeito("B", strTipoDocTes, DataDoc);

                //'SE O DOCUMENTO DE LOGISTICA FOR UMA ADIANTAMENTO
                if (DocumentoAdiantamento)
                    Valor = Math.Abs(Valor);

                documentoTesouraria.EmModoEdicao = false;
                documentoTesouraria.ModuloOrigem = "M";
                documentoTesouraria.Filial = "000";
                documentoTesouraria.TipoLancamento = "000";
                documentoTesouraria.Tipodoc = strTipoDocTes;
                documentoTesouraria.Serie = strSerieDocTes;
                documentoTesouraria.Data = DataDoc;
                documentoTesouraria.DataIntroducao = DataDoc;
                documentoTesouraria.TipoEntidade = TipoEntidade;
                documentoTesouraria.Entidade = Entidade;
                documentoTesouraria.ContaOrigem = strContaBancaria;
                documentoTesouraria.Moeda = Moeda;
                documentoTesouraria.Cambio = PriEngine.Engine.Contexto.MBaseCambioCompra;
                documentoTesouraria.CambioMBase = PriEngine.Engine.Contexto.MBaseCambioCompra;
                documentoTesouraria.CambioMAlt = PriEngine.Engine.Contexto.MAltCambioCompra;
                documentoTesouraria.IdDocOrigem = IdDoc;
                documentoTesouraria.AgrupaMovimentos = false;


                PriEngine.Engine.Tesouraria.Documentos.AdicionaLinha(documentoTesouraria, strMovBancario,
                    strContaBancaria, PriEngine.Engine.Contexto.MoedaBase, Valor, TipoEntidade, Entidade);

                var linhas = documentoTesouraria.Linhas;
                int count = 0;
                foreach (TesBELinhaDocTesouraria linha in linhas)
                {
                    if (count == 0)
                    {
                        linha.DataMovimento = DataDoc;
                        linha.DataValor = DataDoc;
                        linha.Cambio = PriEngine.Engine.Contexto.MBaseCambioCompra;
                        linha.CambioMBase = PriEngine.Engine.Contexto.MBaseCambioCompra;
                        linha.CambioMAlt = PriEngine.Engine.Contexto.MAltCambioCompra;
                        linha.Descricao = "Ref. " + RefATM;

                    }
                    count++;
                }

                PriEngine.Engine.Tesouraria.Documentos.Actualiza(documentoTesouraria, strAvisos);


            }

            catch (Exception ex)
            {
                escreveErro(errorFolder, "GravaLigacaoBancos_pagNib", RefATM + "-" + ex.Message);
                // return false;
                throw ex;
            }



        }

        public void ImprimeDocumento(string tipodoc, string serie, int numdoc, string filial = "000")
        {
            try
            {
                PriEngine.Engine.Vendas.Documentos.ImprimeDocumento(tipodoc, serie, numdoc, filial);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string daUltimoDocumento(string modulo, string tipodoc, string serie)
        {
            try
            {
                return PriEngine.Engine.Base.Series.DaValorAtributo(modulo, tipodoc, serie, "Numerador").ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string daSerieDefeito(string modulo, string tipodoc, DateTime data)
        {
            try
            {
                return PriEngine.Engine.Base.Series.DaSerieDefeito(modulo, tipodoc, data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ValidaPagamentoViaNIB()
        {
            try
            {
                DataTable dtPendente;
                string nib = "";
                string pgtoReferenciaOperacao = "";
                DateTime dataPgto = new DateTime();
                string idAgregador = "";
                string nibPgto = "";
                string numContaPrimavera, nContaBancaria, Nomeficheiro, cliente;
                double montante = 0;
                DateTime dataP = DateTime.Now;
                CultureInfo enUS = new CultureInfo("en-US");
                DataTable dtP, dtCl;
                int indexNib;
                dtP = DaListaPagamentos();

                foreach (DataRow dr in dtP.Rows)
                {
                    try
                    {
                        montante = DaDouble(dr["CDU_Valor"]);
                        nContaBancaria = DaString(dr["CDU_ContaBanc"]);
                        Nomeficheiro = DaString(dr["CDU_NomeFicheiro"]);
                        pgtoReferenciaOperacao = DaString(dr["CDU_Obs"]);
                        nib = DaString(dr["CDU_Ref"]);
                        indexNib = nib.IndexOf('/');
                        nib = nib.Substring(indexNib + 1);
                        nib = VerificaNib(nib);

                        if (nib.Length >0 )
                        {

                            cliente = DaCodCliente(nib);
                            numContaPrimavera = DaNcontaPrimavera(nContaBancaria);

                            if (numContaPrimavera.Length > 0)
                            {
                                bool valida = validaImportacao(Nomeficheiro);
                                if (valida)
                                {
                                    FazPgtoNIB(dataP, nib, pgtoReferenciaOperacao, montante, numContaPrimavera, Nomeficheiro,cliente);
                                }
                            }
                        }
                        else
                        {
                            //caso o nib pago nao esteja associado a nenhum cliente, gera adiantamento para um entidade desconhecida no primavera
                            //cliente = DaCodCliente(nib);
                            numContaPrimavera = DaNcontaPrimavera(nContaBancaria);

                            if (numContaPrimavera.Length > 0)
                            {
                                string strClienteDesconhecido = GetParameter("ClienteDesconhecido");
                                bool valida = validaImportacaoEntDesconhecida(Nomeficheiro, strClienteDesconhecido);
                                if (valida)
                                {
                                    FazPgtoNIB_EntDesc(dataP, nib, pgtoReferenciaOperacao, montante, numContaPrimavera, Nomeficheiro);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        escreveErro(errorFolder, "ValidaPagamentoViaNIB", nib + " " + ex.Message);
                        continue;
                    }

                }

            }
            catch (Exception ex)
            {
                escreveErro(errorFolder, "PagErro", "ValidaPagamentoViaNIB " + ex.Message);
                //throw ex;
            }
        }

        private void FazPgtoNIB(DateTime dataPgto, string nib, string referenciaOperacao, double montante, string contaBancaria, string nomeFich,string codClient)
        {
            try
            {
                string strFilial;
                string strModulo;
                string strTipoDoc;
                string strSerie;
                string strRubrica, strDocValExc, strDocAdiantamento;
                int numdocInt;
                int numPrestacao;
                string strSql;
                string strTipoEntidade = "C";
                string strDocLiquidacao, strEntidade = "";
                int numTransferencia, cnt = 0;
                string strMovimentoBancario = "";
                string strContaBanco = "", tipoDoc = "", serie = "";
                string strIdTransBMEPS = "";
                string strDocTesouraria;
                DataTable dt = new DataTable();
                DateTime dtDataMovBMEPS;
                CctBEDocumentoLiq _docLiq = null; ;
                bool importado = false;
                string dataP = dataPgto.ToString("yyyy-MM-dd");
                double valorTotal = montante;
                double valor = 0;
                int numdoc = 0;
                bool resultTestouraria = false;
                DataTable dtEA=new DataTable();

                strRubrica = GetParameter("RubricaTes");
                strDocLiquidacao = GetParameter("DocumentoLiq");
                strDocTesouraria = GetParameter("DocTesouraria");
                strContaBanco = contaBancaria;
                strDocValExc = GetParameter("DocValExc");
                strDocAdiantamento = GetParameter("DocAdiantamento");

                strSql = string.Format(
                @"select p.Entidade,p.TipoEntidade,
                        p.IdHistorico,p.Filial, p.Modulo,p.TipoDoc,p.Serie,NumdocInt,
                        NumPrestacao,NumTransferencia from Pendentes p with(nolock)
		                left join clientes c on c.Cliente=p.Entidade						
	                    where c.cdu_nib='{0}' and p.TipoDoc not in('ADC','VEC')", nib
                );

                dt = ConsultaSQLDatatable(strSql);
                if (dt.Rows.Count>0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            CctBEPendente _pendente = PriEngine.Engine.PagamentosRecebimentos.Pendentes.EditaID(dr["IdHistorico"].ToString());

                            //strTipoEntidade = DaString(dr["TipoEntidade"]);
                            strEntidade = DaString(dr["Entidade"]);

                            AdicionaLinhasLiquidacao(strDocLiquidacao, strContaBanco, strRubrica, strEntidade, nib, referenciaOperacao, dr, dt, ref _docLiq, ref valorTotal);
                        }
                        catch (Exception ex)
                        {
                            escreveErro(errorFolder, "FazPgtoNIB_AddLinha", nib + "- " + ex.Message);
                        }

                    }

                    if (valorTotal > 0)
                    {
                        try
                        {
                            int i = 0;
                            if (strEntidade != "")
                            {
                                strSql = string.Format(
                                @"select p.Entidade,p.TipoEntidade,
                                p.IdHistorico,p.Filial, p.Modulo,p.TipoDoc,p.Serie,p.NumdocInt
                                from Pendentes p with(nolock)
		                        left join clientes c on c.Cliente=p.Entidade	
	                            left join EntidadesAssociadas EA on c.Cliente=EA.EntidadeAssociada
						        left join clientes cca on cca.cliente=ea.EntidadeAssociada
						        where EA.Entidade='{0}' and cca.CDU_FacturacaoAgrupada=1 and  p.TipoDoc not in('ADC','VEC')", strEntidade);

                                dtEA = ConsultaSQLDatatable(strSql);// entidade associada
                                foreach (DataRow drEA in dtEA.Rows)
                                {
                                    AdicionaLinhasLiquidacao(strDocLiquidacao, strContaBanco, strRubrica, strEntidade, nib, referenciaOperacao, drEA, dtEA, ref _docLiq, ref valorTotal, true);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            escreveErro(errorFolder, "AdicionaPendenteEntAssoc", nib + "- " + ex.Message);
                        }
                    }
                    if (valorTotal > 0 && _docLiq != null)
                    {                 
                        PriEngine.Engine.PagamentosRecebimentos.Liquidacoes.AdicionaValorExcesso(_docLiq, "VEC", 0, valorTotal, referenciaOperacao);
                        _docLiq.ValorRec = montante;
                    }

                    if (_docLiq != null)
                    {
                        PriEngine.Engine.PagamentosRecebimentos.Liquidacoes.Actualiza(_docLiq);

                        PriEngine.Engine.PagamentosRecebimentos.Historico.ActualizaValorAtributo(_docLiq.Tipodoc, "M", _docLiq.Serie, "000", Convert.ToInt32(_docLiq.NumDoc),
                            1, 1, "cdu_NrTransacao", referenciaOperacao);

                        PriEngine.Engine.PagamentosRecebimentos.Historico.ActualizaValorAtributo(_docLiq.Tipodoc, "M", _docLiq.Serie, "000", Convert.ToInt32(_docLiq.NumDoc),
                         1, 1, "cdu_NomeFichPgto", nomeFich);

                        escreveLog(logFolder, "Sucesso", string.Format("O Nib {0} do Cliente {4} gerou o documento {1} {2}/{3} ",
                            nib, _docLiq.Tipodoc, _docLiq.NumDoc, _docLiq.Serie,
                            _docLiq.Entidade));

                        importado = true;
                        if (_docLiq.ValorRec < 0)
                            _docLiq.ValorRec = _docLiq.ValorRec * (-1);

                        //actualiza a tdu_tautomatismo
                        string query = String.Format(@"
                           update TDU_TAutomatismos set cdu_Documento='{0}/{1}/{2}',CDU_DataProc='{3}' 
                            where cdu_Nomeficheiro='{4}'", _docLiq.Tipodoc, _docLiq.NumDoc, _docLiq.Serie, _docLiq.DataDoc.ToString("yyyy-MM-dd HH:mm:ss"), nomeFich);

                        ExecutaQuery(query);

                        //grava ligacao a bancos
                        resultTestouraria = GravaLigacaoBancos(
                                    _docLiq.TipoEntidade, _docLiq.Entidade, _docLiq.Moeda,
                                    _docLiq.ValorRec > 0 ? _docLiq.ValorRec : _docLiq.ValorRec * -1,
                                    _docLiq.DataDoc, _docLiq.ID,
                                    nib, strDocTesouraria, strContaBanco, strRubrica);
                    }
                }
                else
                {
                    try
                    {
                        strSql = string.Format(
                                                    @"select p.Entidade,p.TipoEntidade,
                                p.IdHistorico,p.Filial, p.Modulo,p.TipoDoc,p.Serie,p.NumdocInt
                                from Pendentes p with(nolock)
		                        left join clientes c on c.Cliente=p.Entidade	
	                            left join EntidadesAssociadas EA on c.Cliente=EA.EntidadeAssociada
						        left join clientes cca on cca.cliente=ea.EntidadeAssociada
						        where EA.Entidade='{0}' and cca.CDU_FacturacaoAgrupada=1 and  p.TipoDoc not in('ADC','VEC')", codClient);

                        dtEA = ConsultaSQLDatatable(strSql);// entidade associada
                        if (dtEA.Rows.Count > 0)
                        {
                            foreach (DataRow drEA in dtEA.Rows)
                            {
                                AdicionaLinhasLiquidacao(strDocLiquidacao, strContaBanco, strRubrica, codClient, nib, referenciaOperacao, drEA, dtEA, ref _docLiq, ref valorTotal, true);
                            }

                            if (valorTotal > 0 && _docLiq != null)
                            {                 
                                PriEngine.Engine.PagamentosRecebimentos.Liquidacoes.AdicionaValorExcesso(_docLiq, "VEC", 0, valorTotal, referenciaOperacao);
                                _docLiq.ValorRec = montante;
                            }

                            if (_docLiq != null)
                            {
                                PriEngine.Engine.PagamentosRecebimentos.Liquidacoes.Actualiza(_docLiq);

                                PriEngine.Engine.PagamentosRecebimentos.Historico.ActualizaValorAtributo(_docLiq.Tipodoc, "M", _docLiq.Serie, "000", Convert.ToInt32(_docLiq.NumDoc),
                                    1, 1, "cdu_NrTransacao", referenciaOperacao);

                                PriEngine.Engine.PagamentosRecebimentos.Historico.ActualizaValorAtributo(_docLiq.Tipodoc, "M", _docLiq.Serie, "000", Convert.ToInt32(_docLiq.NumDoc),
                                 1, 1, "cdu_NomeFichPgto", nomeFich);

                                escreveLog(logFolder, "Sucesso", string.Format("O Nib {0} do Cliente {4} gerou o documento {1} {2}/{3} ",
                                    nib, _docLiq.Tipodoc, _docLiq.NumDoc, _docLiq.Serie,
                                    _docLiq.Entidade));

                                importado = true;
                                if (_docLiq.ValorRec < 0)
                                    _docLiq.ValorRec = _docLiq.ValorRec * (-1);

                                //actualiza a tdu_tautomatismo
                                string query = String.Format(@"
                           update TDU_TAutomatismos set cdu_Documento='{0}/{1}/{2}',CDU_DataProc='{3}' 
                            where cdu_Nomeficheiro='{4}'", _docLiq.Tipodoc, _docLiq.NumDoc, _docLiq.Serie, _docLiq.DataDoc.ToString("yyyy-MM-dd HH:mm:ss"), nomeFich);

                                ExecutaQuery(query);

                                //grava ligacao a bancos
                                resultTestouraria = GravaLigacaoBancos(
                                            _docLiq.TipoEntidade, _docLiq.Entidade, _docLiq.Moeda,
                                            _docLiq.ValorRec > 0 ? _docLiq.ValorRec : _docLiq.ValorRec * -1,
                                            _docLiq.DataDoc, _docLiq.ID,
                                            nib, strDocTesouraria, strContaBanco, strRubrica);
                            }

                            if (valorTotal > 0)
                            {
                                CctBEPendente objAdiantamento = new CctBEPendente();

                                var resultAdiantamento = CriaDocumentoAdiantamento(objAdiantamento, strTipoEntidade, codClient, strDocAdiantamento, dataPgto, valorTotal,
                                    referenciaOperacao);

                                escreveLog(logFolder, "CriaAdiantamento", string.Format("A referencia {0} do cliente {4} gerou o documento {1} {2}/{3} ",
                                    nib, objAdiantamento.Tipodoc, objAdiantamento.NumDoc, objAdiantamento.Serie,
                                    objAdiantamento.Entidade));

                                if (resultAdiantamento)
                                {
                                    try
                                    {
                                        resultTestouraria = GravaLigacaoBancos(objAdiantamento.TipoEntidade, objAdiantamento.Entidade, objAdiantamento.Moeda,
                                       objAdiantamento.ValorTotal > 0 ? objAdiantamento.ValorTotal : objAdiantamento.ValorTotal * -1,
                                       objAdiantamento.DataDoc, objAdiantamento.IDHistorico,
                                           nib, strDocTesouraria, strContaBanco, strRubrica);

                                        //actualiza a tdu_tautomatismo
                                        string query = String.Format(@"
                               update TDU_TAutomatismos set cdu_Documento='{0}/{1}/{2}',CDU_DataProc='{3}' 
                                where cdu_Nomeficheiro='{4}'", objAdiantamento.Tipodoc, objAdiantamento.NumDoc, objAdiantamento.Serie, objAdiantamento.DataDoc.ToString("yyyy-MM-dd HH:mm:ss"), nomeFich);
                                        ExecutaQuery(query);

                                        //actualiza historico
                                        query = String.Format(@"
                               update histórico set cdu_NrTransacao='{0}',cdu_NomeFichPgto='{1}' 
                                where tipodoc='{2}' and numdocint={3} and serie='{4}'", referenciaOperacao, nomeFich, objAdiantamento.Tipodoc, objAdiantamento.NumDoc, objAdiantamento.Serie);
                                        ExecutaQuery(query);
                                    }
                                    catch (Exception ex)
                                    {
                                        escreveErro(errorFolder, "ActualizaAdiant", nib + " - " + ex.Message);
                                    }

                                }

                            }
                        }
                        else
                        {
                            if (montante > 0)
                            {
                                CctBEPendente objAdiantamento = new CctBEPendente();

                                var resultAdiantamento = CriaDocumentoAdiantamento(objAdiantamento, strTipoEntidade, codClient, strDocAdiantamento, dataPgto, valorTotal,
                                    referenciaOperacao);

                                escreveLog(logFolder, "CriaAdiantamento", string.Format("A referencia {0} do cliente {4} gerou o documento {1} {2}/{3} ",
                                    nib, objAdiantamento.Tipodoc, objAdiantamento.NumDoc, objAdiantamento.Serie,
                                    objAdiantamento.Entidade));

                                if (resultAdiantamento)
                                {
                                    try
                                    {
                                        resultTestouraria = GravaLigacaoBancos(objAdiantamento.TipoEntidade, objAdiantamento.Entidade, objAdiantamento.Moeda,
                                       objAdiantamento.ValorTotal > 0 ? objAdiantamento.ValorTotal : objAdiantamento.ValorTotal * -1,
                                       objAdiantamento.DataDoc, objAdiantamento.IDHistorico,
                                           nib, strDocTesouraria, strContaBanco, strRubrica);

                                        string query = String.Format(@"
                               update TDU_TAutomatismos set cdu_Documento='{0}/{1}/{2}',CDU_DataProc='{3}' 
                                where cdu_Nomeficheiro='{4}'", objAdiantamento.Tipodoc, objAdiantamento.NumDoc, objAdiantamento.Serie, objAdiantamento.DataDoc.ToString("yyyy-MM-dd HH:mm:ss"), nomeFich);
                                        ExecutaQuery(query);

                                        query = String.Format(@"
                               update histórico set cdu_NrTransacao='{0}',cdu_NomeFichPgto='{1}' 
                                where tipodoc='{2}' and numdocint={3} and serie='{4}'", referenciaOperacao, nomeFich, objAdiantamento.Tipodoc, objAdiantamento.NumDoc, objAdiantamento.Serie);
                                        ExecutaQuery(query);
                                    }
                                    catch (Exception ex)
                                    {
                                        escreveErro(errorFolder, "ActAdiant", nib + " - " + ex.Message);
                                    }

                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        escreveErro(errorFolder, "pendEntAssoc", nib + "- " + ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                escreveErro(errorFolder, "FazPgtoNIB", nib + " - " + ex.Message);
            }
        }
       
        //FAZ PAGAMENTO  para uma entidade desconhecida
        private void FazPgtoNIB_EntDesc(DateTime dataPgto, string nib, string referenciaOperacao, double montante, string contaBancaria, string nomeFich)
        {
            try
            {
                string strFilial;
                string strModulo;
                string strTipoDoc;
                string strSerie;
                string strRubrica, strDocAdiantamento;
                int numdocInt;
                int numPrestacao;
                string strSql;
                string strTipoEntidade = "C";
                string strDocLiquidacao, strEntidade = "";
                int numTransferencia, cnt = 0;
                string strMovimentoBancario = "";
                string strContaBanco = "", tipoDoc = "", serie = "";
                string strIdTransBMEPS = "";
                string strDocTesouraria;
                DataTable dt = new DataTable();
                DateTime dtDataMovBMEPS;
                CctBEDocumentoLiq _docLiq = null; ;
                bool importado = false;
                string dataP = dataPgto.ToString("yyyy-MM-dd");
                double valorTotal = montante;
                double valor = 0;
                int numdoc = 0;
                bool resultTestouraria = false;
                string strClienteDesconhecido;
                DataTable dtEA = new DataTable();
                //VAI BUSCAR A RÚBRICA A USAR NO LANÇAMENTO DE TAXAS NA TESOURARIA
                strRubrica = GetParameter("RubricaTes");
                strClienteDesconhecido = GetParameter("ClienteDesconhecido");
                strDocTesouraria = GetParameter("DocTesouraria");
                strContaBanco = contaBancaria;
                strDocAdiantamento = GetParameter("DocAdiantamento");

                if (montante > 0)
                {
                    CctBEPendente objAdiantamento = new CctBEPendente();

                    var resultAdiantamento = CriaDocumentoAdiantamento(objAdiantamento, strTipoEntidade, strClienteDesconhecido, strDocAdiantamento, dataPgto, valorTotal,
                        referenciaOperacao);

                    escreveLog(logFolder, "CriaAdiantamento", string.Format("A referencia {0} do cliente {4} gerou o documento {1} {2}/{3} ",
                        nib, objAdiantamento.Tipodoc, objAdiantamento.NumDoc, objAdiantamento.Serie,
                        objAdiantamento.Entidade));

                    if (resultAdiantamento)
                    {
                        PriEngine.Engine.PagamentosRecebimentos.Historico.ActualizaValorAtributo(objAdiantamento.Tipodoc, "M", _docLiq.Serie, "000", Convert.ToInt32(objAdiantamento.NumDoc),
                         1, 1, "cdu_NrTransacao", referenciaOperacao);

                        PriEngine.Engine.PagamentosRecebimentos.Historico.ActualizaValorAtributo(objAdiantamento.Tipodoc, "M", _docLiq.Serie, "000", Convert.ToInt32(objAdiantamento.NumDoc),
                         1, 1, "cdu_NomeFichPgto", nomeFich);
                        strMovimentoBancario = GetParameter("RubricaTes");
                      
                        resultTestouraria = GravaLigacaoBancos(objAdiantamento.TipoEntidade, objAdiantamento.Entidade, objAdiantamento.Moeda,
                       objAdiantamento.ValorTotal > 0 ? objAdiantamento.ValorTotal : objAdiantamento.ValorTotal * -1,
                       objAdiantamento.DataDoc, objAdiantamento.IDHistorico,
                           nib, strDocTesouraria, strContaBanco, strMovimentoBancario);
                    }

                }

            }
            catch (Exception ex)
            {
                escreveErro(errorFolder, "FazPgtoNIB_EntDesc", nib + " - " + ex.Message);
                //throw new Exception(string.Format("<FazPgtoNIB>_ {0} {1} ", referenciaOperacao, ex.Message));
            }
        }

        private void AdicionaLinhasLiquidacao(string strDocLiquidacao, string strContaBanco, string strRubrica, string entidade, string nib, string referenciaOperacao, DataRow dr,DataTable dt, ref CctBEDocumentoLiq _docLiq, ref double valorTotal,bool entAssociada=false)
        {
            try
            {
                string strTipoEntidade, strFilial, strModulo, strTipoDoc, strSerie;
                Int32 numdocInt;
                DateTime dataDocLiq = DateTime.Now;
                string tipoDoc = DaString(dr["TipoDoc"]);
                Int32 numdoc = DaInt32(dr["NumdocInt"]);
                string serie = DaString(dr["Serie"]);
                int numPrestacao;
                int numTransferencia;

                double valor = 0;
                //verifica se existe documento
                if (tipoDoc.Length > 0 & numdoc > 0 & serie.Length > 0)
                {
                    // escreveLog(logFolder, nib, "Inicio do Pagamento na referencia - " + nib);

                    //pega o objecto pendente do documento
                    CctBEPendente _pendente = PriEngine.Engine.PagamentosRecebimentos.Pendentes.EditaID(dr["IdHistorico"].ToString());

                    strTipoEntidade = DaString(dr["TipoEntidade"]);
                    entidade = DaString(dr["Entidade"]);

                    strFilial = DaString(dr["Filial"]);
                    strModulo = DaString(dr["Modulo"]);
                    strTipoDoc = DaString(dr["TipoDoc"]);
                    strSerie = DaString(dr["Serie"]);
                     //numPrestacao = Convert.ToInt16(dr["NumPrestacao"]);
                     //numTransferencia = Convert.ToInt16(dr["NumTransferencia"]);
                    numdocInt = Convert.ToInt32(dr["NumdocInt"]);

                    if (_docLiq == null)
                        //preenche o cabecalho
                        _docLiq = CriaCabecalhoLiquidacaoViaNIB(strTipoEntidade, entidade, strDocLiquidacao, dataDocLiq,
                                nib, referenciaOperacao, true, strContaBanco, strRubrica);

                    if (_pendente.ValorPendente >= valorTotal)
                    {
                        valor = valorTotal;
                    }
                    else
                    {
                        valor = _pendente.ValorPendente;

                    }
                    if (valor > 0)
                    {
                        string strEstadoLiq = PriEngine.Engine.Vendas.TabVendas.DaValorAtributo(_pendente.Tipodoc, "Estado").ToString();

                        //Adiciona o documento de venda ao documento de liquidação
                        PriEngine.Engine.PagamentosRecebimentos.Liquidacoes.AdicionaLinha(_docLiq, _pendente.Filial
                            , _pendente.Modulo, _pendente.Tipodoc, _pendente.Serie,
                            _pendente.NumDocInt, _pendente.NumPrestacao, strEstadoLiq, _pendente.NumTransferencia,
                            ref valor);
                        valorTotal -= valor;
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private bool validaImportacao(string nomeFicheiro)
        {
            bool valida = false;
            DataTable dt = new DataTable();
            try
            {
                string query = string.Format(@"select tipodoc + ' ' + convert(nvarchar,numdoc) + '/' + serie as Documento from historico with(nolock)
                            where cdu_NomeFichPgto= '{0}' and tipodoc='RE' and anulado=0", nomeFicheiro);
                dt = ConsultaSQLDatatable(query);

                if (dt.Rows.Count<=0)
                {
                    valida = true;
                }
                else
                {
                    escreveLog(logFolder, "Pgto Existente", string.Format(@"Documento já se encontra integrado com o ficheiro  '{0}'",
                     nomeFicheiro));
                    valida = false;
                }

            }
            catch (Exception ex)
            {
                escreveErro(errorFolder, "validaImportacao", ex.Message);
            }

            return valida;

        }

        private bool validaImportacaoEntDesconhecida(string nomeFicheiro,string entidade)
        {
            bool valida = false;
            DataTable dt = new DataTable();
            try
            {
                string query = string.Format(@"select tipodoc + ' ' + convert(nvarchar,numdoc) + '/' + serie as Documento from historico with(nolock)
                            where cdu_NomeFichPgto= '{0}' and entidade='{1}' and anulado=0", nomeFicheiro,entidade);
                dt = ConsultaSQLDatatable(query);

                if (dt.Rows.Count <= 0)
                {
                    valida = true;
                }
                else
                {
                    escreveLog(logFolder, "Pgto Existente", string.Format(@"Documento já se encontra integrado com o ficheiro  '{0}'",
                     nomeFicheiro));
                    valida = false;
                }

            }
            catch (Exception ex)
            {
                escreveErro(errorFolder, "validaImportacao", ex.Message);
            }

            return valida;

        }
        public DataTable DaListaPagamentos()
        {

            DataTable dt = new DataTable();
            try
            {
                dt = ConsultaSQLDatatable(string.Format(@"select cdu_nomeFicheiro,cdu_ref,cdu_contabanc,cdu_valor,CDU_Obs
                     from TDU_TAutomatismos with(nolock)
                        where CDU_Documento is null and cdu_DataProc is null"));

                return dt;

            }
            catch (Exception ex)
            {
                escreveErro(errorFolder, "DaListaPagamentos", ex.Message);

                throw ex;
            }
        }

        public DataTable DaListaPendentes(string nib)
        {
            DataTable dt = new DataTable();
            try
            {
                //Consulta faturas pendentes referente ao NIB em causa
                dt = ConsultaSQLDatatable(string.Format(@"
                    select p.Entidade,p.TipoEntidade,
                        p.IdHistorico,p.Filial, p.Modulo,p.TipoDoc,p.Serie,NumdocInt,
                        NumPrestacao,NumTransferencia from Pendentes p with(nolock)
		                left join clientes c on c.Cliente=p.Entidade						
	                    where c.cdu_nib='{0}' and c.Cliente like 'cl0%'", nib));

                return dt;

            }
            catch (Exception ex)
            {
                escreveErro(errorFolder, "DaListaPendentes", ex.Message);

                throw ex;
            }
        }

        public string VerificaNib(string nib)
        {
            DataTable dt = new DataTable();
            string clienteNib = "";
            try
            {
                dt = ConsultaSQLDatatable(string.Format(@"select CDU_Nib from clientes c with(nolock)
                        where CDU_NIb='{0}'", nib));

                if (dt.Rows.Count > 0)
                {
                    clienteNib = DaString(dt.Rows[0][0]);
                }
                else
                {
                    escreveLog(logFolder, "VerificaNib", string.Format("O Nib {0} não está associado a nenhum cliente!", nib));
                }
                return clienteNib;

            }
            catch (Exception ex)
            {
                escreveErro(errorFolder, "VerificaNiB "+nib+" ", ex.Message);
                return "";
            }
        }

        public string DaNcontaPrimavera(string numcontaBancaria)
        {
            DataTable dt = new DataTable();
            string numConta = "";
            try
            {
                dt = ConsultaSQLDatatable(string.Format(@"select conta from contasBancarias c with(nolock)
                        where numConta='{0}'", numcontaBancaria));

                if (dt.Rows.Count > 0)
                {
                    numConta = DaString(dt.Rows[0][0]);
                }
                else
                {
                    escreveLog(logFolder, "VerificaNcontaPrimavera", string.Format("O numero de conta bancária não está associado a nenhum numero de conta no Primavera! " + numcontaBancaria));
                }
                return numConta;

            }
            catch (Exception ex)
            {
                escreveErro(errorFolder, "VerificaNcontaPrimavera", ex.Message);

                return "";
            }
        }


        public string DaCodCliente(string nib)
        {
            DataTable dt = new DataTable();
            string cliente = "";
            try
            {
                dt = ConsultaSQLDatatable(string.Format(@"select cliente from clientes c with(nolock)
                        where CDU_NIb='{0}'", nib));

                if (dt.Rows.Count > 0)
                {
                    cliente = DaString(dt.Rows[0][0]);
                }
                return cliente;

            }
            catch (Exception ex)
            {
                escreveErro(errorFolder, "DaCodCliente " + nib + " ", ex.Message);
                return "";
            }
        }

        public Boolean CriaDocumentoAdiantamento(CctBEPendente _docAdiantamento, string TipoEntidade,
    string Entidade, string TipoDoc, DateTime Data, double Valor, string referenciaOperacaoBIM)
        {
            try
            {

                string strAvisos = "";


                CctBELinhaPendente objLinhaPendente = new CctBELinhaPendente();

                _docAdiantamento.Tipodoc = TipoDoc;
                _docAdiantamento.Serie = PriEngine.Engine.Base.Series.DaSerieDefeito("M", TipoDoc, Data);
                _docAdiantamento.TipoEntidade = TipoEntidade;
                _docAdiantamento.Entidade = Entidade;
                _docAdiantamento.Moeda = PriEngine.Engine.Contexto.MoedaBase;
                _docAdiantamento.Utilizador = PriEngine.Engine.Contexto.UtilizadorActual;

                PriEngine.Engine.PagamentosRecebimentos.Pendentes.PreencheDadosRelacionados(_docAdiantamento);

                _docAdiantamento.DataDoc = Data;
                _docAdiantamento.DataIntroducao = Data;
                _docAdiantamento.DataVenc = Data;

                objLinhaPendente.Descricao = "Ref. Banc.: " + referenciaOperacaoBIM;
                objLinhaPendente.Incidencia = Valor;
                objLinhaPendente.Total = objLinhaPendente.Incidencia;
                objLinhaPendente.PercIvaDedutivel = 100;

                _docAdiantamento.ValorTotal = objLinhaPendente.Incidencia;
                _docAdiantamento.TotalIva = objLinhaPendente.ValorIva;
                _docAdiantamento.ValorPendente = objLinhaPendente.Incidencia;

             //   _docAdiantamento.CamposUtil["cdu_refOperacaoBIM"].Valor = referenciaOperacaoBIM;
                // _docAdiantamento.get_CamposUtil().get_Item("CDU_IDTransBMEPS").Valor = IdTransBMEPS;
                // _docAdiantamento.get_CamposUtil().get_Item("CDU_FicheiroBMEPS").Valor = FicheiroBMEPS;


                PriEngine.Engine.PagamentosRecebimentos.Pendentes.Actualiza(_docAdiantamento);

                PriEngine.Engine.PagamentosRecebimentos.Historico.ActualizaValorAtributoID(_docAdiantamento.IDHistorico, "cdu_NomeFichPgto", referenciaOperacaoBIM);
                // PriEngine.Engine.Comercial.Historico.ActualizaValorAtributoID(_docAdiantamento.get_IdHistorico(), "CDU_IDTransBMEPS", IdTransBMEPS);
                // PriEngine.Engine.Comercial.Historico.ActualizaValorAtributoID(_docAdiantamento.get_IdHistorico(), "CDU_FicheiroBMEPS", FicheiroBMEPS);

                return true;

            }
            catch (Exception ex)
            {
                escreveLog(referenciaOperacaoBIM, "<CriaDocumentoAdiantamento>_" + ex.Message);
                return false;
            }


        }
    }
}
