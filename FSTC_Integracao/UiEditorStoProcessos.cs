using Primavera.Extensibility.TechnicalServices.Editors;
using Primavera.Extensibility.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using CmpBE100;
using System.Data;
using ErpBS100;
using FSTC_Integracao.Services;
using System.Windows;
using StdPlatBS100;

namespace FSTC_Integracao
{
    public class UiEditorStoProcessos : EditorStpProcessos
    {
        bool Fechado;

        public override void DepoisDeConfirmarIntervencao(ExtensibilityEventArgs e)
        {
            base.DepoisDeConfirmarIntervencao(e);
            try
            {
                Fechado = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override void DepoisDeGravar(string Filial, string TipoDoc, string Serie, int Numero, ExtensibilityEventArgs e)
        {
            base.DepoisDeGravar(Filial, TipoDoc, Serie, Numero, e);
            try
            {
                Geral geral = new Geral(BSO);

                string query,numdocExt;
                DataTable dt= new DataTable();
                double duracao, duracaoHora;
                decimal taxaIva;               
                string documento = TipoDoc + "/" + Convert.ToString(Numero) + "/" + Serie;
                string tipodocTes = geral.GetParameter("DocSerTec");
                List<TecnicoHoraTrab> listTecnicoHoraTrab = new List<TecnicoHoraTrab>();

                if (Fechado)
                {
                    query = String.Format(@"select t.Tecnico,t.CDU_Entidade,descricaoresp,duracao,AI.Artigo,A.ArmazemSugestao,A.UnidadeBase,A.LocalizacaoSugestao,A.Iva,Iva.taxa, Datahorafecho from STP_Processos p
                        left join  STP_Intervencoes I on p.id=i.ProcessoID
                        left join STP_Tecnicos T on T.Tecnico=I.Tecnico
                        left join STP_ArtigosIntervencao AI on AI.IntervencaoID=I.ID
                        left join Artigo a on a.Artigo=AI.Artigo
                        left join Iva on Iva.IVA=A.Iva
                        where T.CDU_TecExterno=1 and processoID in
                        (select id from STP_Processos where tipodoc='{0}' and serie='{1}' and numProcesso={2} and Datahorafecho is not null)
                        ", TipoDoc, Serie, Numero);
                    
                    dt = BSO.ConsultaDataTable(query);

                    if (dt.Rows.Count > 0)
                    {
                        ClsServTec clsServ = new ClsServTec(BSO,PSO);
                        //lista os atributos do tecnico
                        foreach (DataRow dr in dt.Rows)
                        {

                            TecnicoHoraTrab tecnicoHora = new TecnicoHoraTrab();
                            duracao = Convert.ToDouble(dr["duracao"]);
                            duracao = duracao / 60;
                            taxaIva = Convert.ToDecimal(geral.GetParameter("TaxaIva"));
                            taxaIva = taxaIva / 100;
                            numdocExt = TipoDoc + "/" + Numero + "/" + Serie;
                            tecnicoHora.artigo = geral.GetParameter("ArtigoSTP");
                            tecnicoHora.dataFecho = geral.DaString(dr["Datahorafecho"]);
                            tecnicoHora.descricao = geral.DaString(dr["descricaoresp"]);
                            tecnicoHora.duracao = duracao;
                            tecnicoHora.Entidade = geral.DaString(dr["CDU_Entidade"]);
                            tecnicoHora.NumDocExterno = numdocExt;
                            tecnicoHora.Tipodoc = tipodocTes;
                            tecnicoHora.ArmazemLocalizacao = geral.DaString(dr["LocalizacaoSugestao"]);
                            tecnicoHora.ArmazemSugestao = geral.DaString(dr["ArmazemSugestao"]);
                            tecnicoHora.Iva = geral.GetParameter("CodIva");
                            tecnicoHora.Unidade = geral.GetParameter("Unidade");
                            tecnicoHora.TaxaIva = taxaIva;
                            tecnicoHora.Documento = documento;
                            listTecnicoHoraTrab.Add(tecnicoHora);
                        }
                        bool valida = validaExistenciaDoc(TipoDoc, Numero, Serie);
                        if (valida)
                        {
                            //cria documento de compra
                            clsServ.CriaDocCompra(listTecnicoHoraTrab);           
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                PSO.MensagensDialogos.MostraAviso(ex.Message, StdBSTipos.IconId.PRI_Exclama, "Erro");
            }
        }
        private bool validaExistenciaDoc(string tipodoc,int numdoc,string serie)
        {
            bool valida = false;
            string documento = tipodoc + "/" + Convert.ToString(numdoc) + "/" + serie;
            DataTable dt = new DataTable();
            try
            {
                string query = string.Format(@"select tipodoc,numdoc,serie from cabecCompras with(nolock)
                            where cdu_DocSTP= '{0}'",documento);
               
                dt = BSO.ConsultaDataTable(query);

                if (dt.Rows.Count <= 0)
                {
                    valida = true;
                }
                else
                {
                    PSO.MensagensDialogos.MostraAviso(string.Format(@"Documento já se encontra integrado no modulo de compras, com o documento '{0}', 
                        por favor, faça a correção do mesmo no editor de compras!",
                     documento), StdBSTipos.IconId.PRI_Exclama, "Erro");
                }
            }
            catch (Exception ex)
            {
                PSO.MensagensDialogos.MostraAviso(ex.Message, StdBSTipos.IconId.PRI_Exclama, "Erro");
            }
            return valida;
        }

    }

}
