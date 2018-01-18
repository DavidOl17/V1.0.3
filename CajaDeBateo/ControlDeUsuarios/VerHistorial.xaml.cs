using CajaDeBateo.BaseDeDatos;
using CajaDeBateo.ComunicacionArduino;
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
using System.Data;
using System.Threading;

namespace CajaDeBateo.ControlDeUsuarios
{
    /// <summary>
    /// Interaction logic for VerHistorial.xaml
    /// </summary>
    public partial class VerHistorial : UserControl
    {
        private ArduinoComunication arduino;
        private Label aux;
        private bool primera = false;
        private DBConnect baseDeDatos;
        private bool ArduinoConectado;
        private String AuxPerm = "";
        private String AuxTem = "";

        public VerHistorial(int puerto, string[] puertos)
        {
            InitializeComponent();
            try
            {
                baseDeDatos = new DBConnect();
                arduino = new ArduinoComunication(puerto, puertos);
                arduino.RespuestaRecivida += new EventHandler(Read);
                aux = lblDato;
                arduino.Write("2");
                lblDato.Content = "Pase la tarjeta";
                ArduinoConectado = true;
                this.IsVisibleChanged += VerHistorial_IsVisibleChanged;
            }
            catch (SensorNotFoundExceptio e)
            {
                MessageBox.Show("Error. Conección con lectora/escritora no encontrada." + e.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ArduinoConectado = false;
            }
            catch (NullReferenceException e)
            {
                MessageBox.Show("Error. Conección con lectora/escritora no encontrada." + e.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ArduinoConectado = false;
            }
            finally
            {
                Thread.Sleep(2000);
                try
                {
                    arduino.Write("2");
                }
                catch (Exception e)
                {
                    String Valor = e.Message;
                }
            }

            if (!ArduinoConectado)
            {
                lblDato.Visibility = Visibility.Hidden;
                TlblDato.Visibility = Visibility.Visible;
            }
        }

        private void VerHistorial_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!primera)
            {
                primera = true;
            }
            else
            {
                try
                {
                    arduino.CerrarComunicacion();
                }
                catch (NullReferenceException ex)
                {
                    String Val = ex.Message;
                }
            }
        }

        string data;
        private void Read(object sender, EventArgs e)
        {
            data = (string)sender;
            lblDato.Dispatcher.Invoke(new Action(() => { lblDato.Content = data; }));
            Dispatcher.Invoke(ObtenerDatos);
        }

        private void BtnCancelarVerHistorial_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Win = (MainWindow)Window.GetWindow(this);
            Win.RegresarPantallaInicial();
        }

        private void BtnBuscarVerHistorial_Click(object sender, RoutedEventArgs e)
        {
            ObtenerDatos();
            if (ArduinoConectado)
            {
                BtnBuscarVerHistorial.IsEnabled = false;
                try
                {
                    arduino.Write("2");
                }
                catch (NullReferenceException ex)
                {
                    String Val = ex.Message;
                }
            }
        }

        void ObtenerDatos()
        {
            String Arg;
            if (ArduinoConectado)
                Arg = lblDato.Content.ToString();
            else
                Arg = TlblDato.Text;

            DataTable Tabla = new DataTable();
            Tabla = baseDeDatos.ObtenerHistorial(Arg);
            if (Tabla == null)
            {
                MessageBox.Show("Error al realizar la consulta a la base de datos. Llame a soporte. ", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Tabla.Rows.Count == 0)
            {
                MessageBox.Show("No se encontraron cargas de créditos para el usuario seleccionado.", "Sin resultados",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (Tabla.Rows.Count == 1 && Tabla.Rows[0][0].ToString() == "")
            {
                MessageBox.Show("El usuario no se encuentra registrado.", "Usuario inexistente",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                DataGridHistorial.ItemsSource = Tabla.DefaultView;
                lblDato.Content = "Pase la tarjeta";
                for (int i = 0; i < DataGridHistorial.Columns.Count; i++)
                {
                    //DataGridHistorial.Columns[i].Width = 20'*';
                }
            }
        }

        private void TlblDato_TextChanged(object sender, TextChangedEventArgs e)
        {
            AuxTem = TlblDato.Text;
            if (EsNumCorrecto(AuxTem) || TlblDato.Text.Length == 0)
            {
                AuxPerm = AuxTem;
            }
            else
            {
                TlblDato.Text = AuxPerm;
                TlblDato.CaretIndex = TlblDato.Text.Length;
            }
            BtnBuscarVerHistorial.IsEnabled = (AuxTem.Length > 0);
        }

        private bool EsNumCorrecto(String Cadena)
        {
            int n = 0;
            bool EsNumero = int.TryParse(Cadena, out n);
            return (EsNumero && n > 0 && n <= 2147483647);
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ObtenerDatos();
            }
        }
    }
}
