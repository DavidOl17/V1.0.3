using CajaDeBateo.ComunicacionArduino;
using CajaDeBateo.BaseDeDatos;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace CajaDeBateo.ControlDeUsuarios
{
    /// <summary>
    /// Lógica de interacción para Leer.xaml
    /// </summary>
    public partial class ActDesactTarjeta : UserControl
    {
        ArduinoComunication arduino;
        Label aux;
        bool primera = false;
        private DBConnect baseDeDatos;
        int TipoDeAccion;

        public ActDesactTarjeta(int puerto, string[] puertos, int Tipo)
        {
            InitializeComponent();
            baseDeDatos = new DBConnect();
            switch (Tipo)
            {
                case 0:
                    lblEncabezado.Content = "Desactivación de Tarjeta";
                    lblPregunta.Content = "¿Desactivar Tarjeta?";
                    lblBoton.Content = "Desactivar";
                    break;
                case 1:
                    lblEncabezado.Content = "Activación de Tarjeta";
                    lblPregunta.Content = "¿Activar Tarjeta?";
                    lblBoton.Content = "Activar";
                    break;
            }
            TipoDeAccion = Tipo;

            try
            {
                arduino = new ArduinoComunication(puerto, puertos);
                arduino.RespuestaRecivida += new EventHandler(Read);
                aux = lblDato;
                arduino.Write("2");
                lblDato.Content = "Pase la tarjeta";
            }
            catch (SensorNotFoundExceptio e)
            {
                MessageBox.Show("Error. Conección con lectora/escritora no encontrada." + e.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NullReferenceException e)
            {
                MessageBox.Show("Error. Conección con lectora/escritora no encontrada." + e.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.IsVisibleChanged += ActDesactTarjeta_IsVisibleChanged;
        }

        private void ActDesactTarjeta_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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
        }

        private void BtnCancelarActDesactTarjeta_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Win = (MainWindow)Window.GetWindow(this);
            Win.RegresarPantallaInicial();
        }

        private void BtnAccionActivarTarjeta_Click(object sender, RoutedEventArgs e)
        {
            String Arg = lblDato.Content.ToString();
            int Resp = baseDeDatos.ActDesactTarjeta(Arg, TipoDeAccion);
            String Aux = "";
            if (TipoDeAccion == 0)
                Aux = "activada";
            else
                Aux = "desactivada";

            if (Resp == 1)
            {
                MessageBox.Show("Esta tarjeta se encuentra actualmente " + Aux + ".", "Tarjeta activada",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (Resp == 2)
            {
                MessageBox.Show("Esta tarjeta no se ha dado de alta.", "Tarjeta inexistente",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (Resp == 0)
            {
                MessageBox.Show("Tarjeta " + Aux + " con éxito.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
