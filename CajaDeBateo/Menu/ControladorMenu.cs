using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CajaDeBateo.Menu
{
    class ControladorMenu
    {
        Button[] botones;
        int numBotones;
        bool visible;
        public ControladorMenu(ref Button[] botones, int numBotones)
        {
            this.botones = botones;
            this.numBotones = numBotones;
        }

        public bool Visible { get => visible; }

        public void SetVisible(bool visible)
        {
            this.visible = visible;
            if(visible)
            {
                for (int i = 0; i < numBotones; i++)
                    botones[i].Visibility = System.Windows.Visibility.Visible;
            }
            else
                for (int i = 0; i < numBotones; i++)
                    botones[i].Visibility = System.Windows.Visibility.Hidden;

        }
    }
}
