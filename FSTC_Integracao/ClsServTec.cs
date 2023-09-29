using Primavera.Extensibility.TechnicalServices.Editors;
using Primavera.Extensibility.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;

namespace FSTC_Integracao
{
    public class ClsServTec : EditorStpPedidos
    {
        public override void AntesDeGravar(ref bool Cancel, ExtensibilityEventArgs e)
        {
            base.AntesDeGravar(ref Cancel, e);
            try
            {

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override void DepoisDeGravar(string Filial, string TipoDocumento, string Serie, int Documento, ExtensibilityEventArgs e)
        {
            base.DepoisDeGravar(Filial, TipoDocumento, Serie, Documento, e);
            try
            {

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
