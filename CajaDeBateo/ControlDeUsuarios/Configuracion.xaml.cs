using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace CajaDeBateo.ControlDeUsuarios
{
    /// <summary>
    /// Interaction logic for Configuracion.xaml
    /// </summary>
    public partial class Configuracion : UserControl
    {
        private int CreditosMensuales;
        private String AuxPerm = "";
        private String AuxTem = "";
        private String ConfigPath;

        public Configuracion(int CreditMensLeidos, String ConfigFilePath)
        {
            InitializeComponent();
            CreditosMensuales = CreditMensLeidos;
            ConfigPath = ConfigFilePath;
            ConfiguracionCantidadCreditosMensuales.Text = CreditosMensuales.ToString();
        }

        private void BtnCancelarConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Win = (MainWindow)Window.GetWindow(this);
            Win.RegresarPantallaInicial();
        }

        private void BtnGuardarConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(ConfigPath))
            {
                File.Delete(ConfigPath);
            }
            CreditosMensuales = int.Parse(ConfiguracionCantidadCreditosMensuales.Text);
            String CadOut = "CreditosMensuales=" + CreditosMensuales;
            StreamWriter Out = new StreamWriter(ConfigPath);
            Out.WriteLine(CadOut);
            Out.Close();
            MessageBox.Show("Configuración guardada con éxito.", "Exito", MessageBoxButton.OK);
        }

        private void ConfiguracionCantidadCreditosMensuales_TextChanged(object sender, TextChangedEventArgs e)
        {
            AuxTem = ConfiguracionCantidadCreditosMensuales.Text;
            if (EsNumCorrecto(AuxTem) || ConfiguracionCantidadCreditosMensuales.Text.Length == 0)
            {
                AuxPerm = AuxTem;
            }
            else
            {
                ConfiguracionCantidadCreditosMensuales.Text = AuxPerm;
                ConfiguracionCantidadCreditosMensuales.CaretIndex = ConfiguracionCantidadCreditosMensuales.Text.Length;
            }

            BtnGuardarConfiguracion.IsEnabled = (ConfiguracionCantidadCreditosMensuales.Text.Length > 0);
        }

        private bool EsNumCorrecto(String Cadena)
        {
            int n = 0;
            bool EsNumero = int.TryParse(Cadena, out n);
            return (EsNumero && n > 0 && n <= 2147483647);
        }
    }
}
