using CajaDeBateo.BaseDeDatos;
using CajaDeBateo.ComunicacionArduino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace CajaDeBateo.ControlDeUsuarios
{
    /// <summary>
    /// Interaction logic for CargarCreditosAdicionales.xaml
    /// </summary>
    public partial class CargarCreditosAdicionales : UserControl
    {
        private String AuxPerm = "";
        private String AuxTem = "";
        ArduinoComunication arduino;
        bool primera = false;
        DBConnect baseDeDatos;
        int id;
        public CargarCreditosAdicionales(int puerto, string[] puertos)
        {
            InitializeComponent();
            baseDeDatos = new DBConnect();
            DesactivaControles();
            try
            {
                arduino = new ArduinoComunication(puerto, puertos);
            }
            catch (SensorNotFoundExceptio e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Thread.Sleep(1500);
            }
            try
            {
                arduino.RespuestaRecivida += new EventHandler(Read);
                arduino.Write("2");
                IDUusuarioCargarCreditosAdicionales.Content = "Pase la tarjeta";
            }
            catch (NullReferenceException e)
            {
                String Mensaje = "Conexión a Lector/Escritor no detectada. " + e.Message;
                MessageBox.Show(Mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //Win.RegresarPantallaInicial();
            }
            this.IsVisibleChanged += ActivarTarjeta_IsVisibleChanged;
        }

        private void Read(object sender, EventArgs e)
        {
            string data = (string)sender;
            id = int.Parse(data);
            ActivaControles(data);
        }

        private void ActivarTarjeta_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!primera)
                primera = true;
            else
                arduino.CerrarComunicacion();
        }

        private void BtnCancelarCreditosAdicionales_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Win = (MainWindow)Window.GetWindow(this);
            Win.RegresarPantallaInicial();
        }

        private void CantidadCreditosAdicionales_TextChanged(object sender, TextChangedEventArgs e)
        {
            AuxTem = CantidadCreditosAdicionales.Text;
            if (EsNumCorrecto(AuxTem) || CantidadCreditosAdicionales.Text.Length == 0)
            {
                AuxPerm = AuxTem;
            }
            else
            {
                CantidadCreditosAdicionales.Text = AuxPerm;
                CantidadCreditosAdicionales.CaretIndex = CantidadCreditosAdicionales.Text.Length;
            }

            BtnGuardarCargarCreditosAdicionales.IsEnabled = (CantidadCreditosAdicionales.Text.Length > 0);
        }

        private bool EsNumCorrecto(String Cadena)
        {
            int n = 0;
            bool EsNumero = int.TryParse(Cadena, out n);
            return (EsNumero && n > 0 && n <= 2147483647);
        }

        private void BtnGuardarCargarCreditosAdicionales_Click(object sender, RoutedEventArgs e)
        {
            int creditos = int.Parse(CantidadCreditosAdicionales.Text);
            if(baseDeDatos.AgregarCreditoAdicion(id.ToString(),creditos))
            {
                MessageBox.Show("Creditos agregados", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Error al guardar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            DesactivaControles();
            arduino.Write("2");
        }
        void DesactivaControles()
        {
            CantidadCreditosAdicionales.Text = "";
            BtnGuardarCargarCreditosAdicionales.IsEnabled = false;
            CantidadCreditosAdicionales.IsEnabled = false;
            IDUusuarioCargarCreditosAdicionales.Content = "Pase la tarjeta";
        }
        void ActivaControles(string data)
        {
            BtnGuardarCargarCreditosAdicionales.Dispatcher.Invoke(new Action(() => { BtnGuardarCargarCreditosAdicionales.IsEnabled = true; }));
            CantidadCreditosAdicionales.Dispatcher.Invoke(new Action(() => { CantidadCreditosAdicionales.IsEnabled = true; }));
            IDUusuarioCargarCreditosAdicionales.Dispatcher.Invoke(new Action(() => { IDUusuarioCargarCreditosAdicionales.Content = data; }));
        }
    }
}
