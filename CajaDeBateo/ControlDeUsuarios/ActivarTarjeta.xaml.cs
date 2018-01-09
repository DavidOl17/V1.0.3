using CajaDeBateo.ComunicacionArduino;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace CajaDeBateo.ControlDeUsuarios
{
    /// <summary>
    /// Lógica de interacción para Leer.xaml
    /// </summary>
    public partial class ActivarTarjeta : UserControl
    {
        ArduinoComunication arduino;
        //string info;
        Label aux;
        bool primera = false;
        public ActivarTarjeta(int puerto, string[] puertos, MainWindow Win)
        {
            InitializeComponent();
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
            //arduino.Reset();
            try
            {
                arduino.RespuestaRecivida += new EventHandler(Read);
                aux = lblDato;
                arduino.Write("2");
                lblDato.Content = "Pase la tarjeta";
            }
            catch (NullReferenceException e)
            {
                String Mensaje = "Conexión a Lector/Escritor no detectada. " + e.Message;
                MessageBox.Show(Mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //Win.RegresarPantallaInicial();
            }
            this.IsVisibleChanged += ActivarTarjeta_IsVisibleChanged;
        }

        private void ActivarTarjeta_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!primera)
                primera = true;
            else
                arduino.CerrarComunicacion();
        }

        string data;
        private void Read(object sender, EventArgs e)
        {
            data = (string)sender;
            lblDato.Dispatcher.Invoke(new Action(() => { lblDato.Content = data; }));
        }

        private void BtnCancelarActivarTarjeta_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Win = (MainWindow)Window.GetWindow(this);
            Win.RegresarPantallaInicial();
        }
    }
}
