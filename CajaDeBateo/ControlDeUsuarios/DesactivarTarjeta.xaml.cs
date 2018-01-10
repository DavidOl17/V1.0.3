using CajaDeBateo.ComunicacionArduino;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CajaDeBateo.ControlDeUsuarios
{
    /// <summary>
    /// Interaction logic for DesactivarTarjeta.xaml
    /// </summary>
    public partial class DesactivarTarjeta : UserControl
    {
        ArduinoComunication arduino;
        //string info;
        Label aux;
        public DesactivarTarjeta(ref ArduinoComunication arduino)
        {
            InitializeComponent();
            this.arduino = arduino;
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
            }
        }
        string data;
        private void Read(object sender, EventArgs e)
        {
            data = (string)sender;
            lblDato.Dispatcher.Invoke(new Action(() => { lblDato.Content = data; }));
        }

        private void BtnCancelarDesactivarTarjeta_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Win = (MainWindow)Window.GetWindow(this);
            Win.RegresarPantallaInicial();
        }
    }
}
