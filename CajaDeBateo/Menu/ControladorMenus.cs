using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CajaDeBateo.Menu
{
    class ControladorMenus
    {
        ControladorMenu principal, tarjetas, creditos, agregarCreditos, configuracion;
        public ControladorMenus(ref ControladorMenu principal, ref ControladorMenu tarjetas, ref ControladorMenu creditos,
            ref ControladorMenu agregarCreditos, ref ControladorMenu configuracion)
        {
            this.principal = principal;
            this.tarjetas = tarjetas;
            this.creditos = creditos;
            this.agregarCreditos = agregarCreditos;
            this.configuracion = configuracion;
            setPrincipal();
        }
        public void setPrincipal()
        {
            principal.SetVisible(true);
            tarjetas.SetVisible(false);
            creditos.SetVisible(false);
            agregarCreditos.SetVisible(false);
            configuracion.SetVisible(false);
        }
        public void setTarjetas()
        {
            principal.SetVisible(false);
            tarjetas.SetVisible(true);
            creditos.SetVisible(false);
            agregarCreditos.SetVisible(false);
            configuracion.SetVisible(false);
        }
        public void setCreditos()
        {
            principal.SetVisible(false);
            tarjetas.SetVisible(false);
            creditos.SetVisible(true);
            agregarCreditos.SetVisible(false);
            configuracion.SetVisible(false);
        }
        public void setAgregarCreditos()
        {
            principal.SetVisible(false);
            tarjetas.SetVisible(false);
            creditos.SetVisible(false);
            agregarCreditos.SetVisible(true);
            configuracion.SetVisible(false);
        }
        public void setConfiguracion()
        {
            principal.SetVisible(false);
            tarjetas.SetVisible(false);
            creditos.SetVisible(false);
            agregarCreditos.SetVisible(false);
            configuracion.SetVisible(true);
        }
        public void setBack()
        {
            if (agregarCreditos.Visible)
                setCreditos();
            else
                setPrincipal();
            
        }
    }
}
