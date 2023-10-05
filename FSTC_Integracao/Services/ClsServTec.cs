using CmpBE100;
using ErpBS100;
using StdPlatBS100;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FSTC_Integracao.Services
{
    class ClsServTec: Geral
    {
        public StdBSInterfPub pso { get; set; }
        public ErpBS bso { get; set; }
        public ClsServTec(dynamic BSO, dynamic PSO)
        {
            this.bso = BSO;
            this.pso = PSO;
        }
        public void CriaDocCompra(List<TecnicoHoraTrab> listTecnicoHTrab)
        {
            try
            {

                var _doc = new CmpBEDocumentoCompra();
                DataTable dt = new DataTable();
                string query;
                int i = 0;
                foreach (var item in listTecnicoHTrab)
                {
                    //verifica se a entidade existe no sistema
                    query = String.Format(@"select terceiro 'Entidade' from outrosterceiros where terceiro='{0}'", item.Entidade);

                    dt = bso.ConsultaDataTable(query);

                    if (dt.Rows.Count > 0)
                    {
                        if (i==0)
                        {
                            _doc.Entidade = item.Entidade;
                        }
                        if (item.Tipodoc.Length>0)
                        {
                            _doc.Tipodoc = item.Tipodoc;
                            _doc.Serie = bso.Base.Series.DaSerieDefeito("C", _doc.Tipodoc);
                            _doc.NumDocExterno = item.NumDocExterno;
                            _doc.TipoEntidade = "R";
                            // _doc.CamposUtil["CDU_Ficheiro"].Valor = cdu_ficheiro;//dra.Name;
                            _doc.CamposUtil["cdu_DocSTP"].Valor = item.Documento;

                            bso.Compras.Documentos.PreencheDadosRelacionados(_doc);
                          //  _doc.Fluxo = "999";
                            _doc.Moeda = "MT";
                            _doc.DataDoc = Convert.ToDateTime(item.dataFecho);
                            _doc.DataVenc = Convert.ToDateTime(item.dataFecho);

                            bso.Compras.Documentos.AdicionaLinha(_doc, item.artigo);
                            _doc.Linhas.GetEdita(_doc.Linhas.NumItens).Descricao = item.descricao;
                            _doc.Linhas.GetEdita(_doc.Linhas.NumItens).CodIva = item.Iva;
                            _doc.Linhas.GetEdita(_doc.Linhas.NumItens).TaxaIva = (float)item.TaxaIva;
                            _doc.Linhas.GetEdita(_doc.Linhas.NumItens).Armazem = item.ArmazemSugestao;
                            _doc.Linhas.GetEdita(_doc.Linhas.NumItens).Localizacao =item.ArmazemLocalizacao;
                            _doc.Linhas.GetEdita(_doc.Linhas.NumItens).Quantidade = item.duracao;
                            _doc.Linhas.GetEdita(_doc.Linhas.NumItens).Unidade =item.Unidade;

                        }
                        else
                        {
                            pso.MensagensDialogos.MostraAviso("Deve configurar o tipo de documento na tabela parâmetros!", StdBSTipos.IconId.PRI_Exclama, "Erro");
                        }

                    }
                    else
                    {
                        pso.MensagensDialogos.MostraAviso("A entidade nao existe na tabela de Outro Terceiro!", StdBSTipos.IconId.PRI_Exclama, "Erro");
                    }
                    i += 1;
                }
                //Grava o documento de compra
                bso.Compras.Documentos.Actualiza(_doc);

                pso.MensagensDialogos.MostraAviso(String.Format(@"Foi gerado o documento '{0}' no módulo de compras!",_doc.Documento), StdBSTipos.IconId.PRI_Informativo);
            }
            catch (Exception ex)
            {
                pso.MensagensDialogos.MostraAviso(ex.Message, StdBSTipos.IconId.PRI_Exclama, "Erro");
            }
        }

    }
    public class TecnicoHoraTrab
    {
        public string descricao { get; set; }
        public string artigo { get; set; }
        public string dataFecho { get; set; }
        public double duracao { get; set; }
        public string Entidade { get; set; }
        public string Tipodoc { get; set; }
        public string NumDocExterno { get; set; }
        public string Iva { get; set; }
        public string Unidade { get; set; }
        public decimal TaxaIva { get; set; }
        public string Documento { get; set; }
        public string ArmazemLocalizacao { get; set; }
        public string ArmazemSugestao { get; set; }
    }
}
