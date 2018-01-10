using CajaDeBateo.BaseDeDatos;
using CajaDeBateo.ComunicacionArduino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Lógica de interacción para EscribirDebug.xaml
    /// </summary>
    public partial class CrearTarjeta : UserControl
    {
        //Random random = new Random();
        int id;
        ArduinoComunication arduino;
        bool escribirID = false;
        bool leer = false;
        bool guardado = false;
        DBConnect baseDeDatos;
        bool primera = false;
        public CrearTarjeta(int puerto, string[] puertos)
        {
            InitializeComponent();
            escribirID = false;
            leer = false;
            this.IsVisibleChanged += CrearTarjeta_IsVisibleChanged;
            baseDeDatos = new DBConnect();
            //arduino.Reset();
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
            try
            {
                arduino.RespuestaRecivida += new EventHandler(Read);
                //id = random.Next(0, 50000);
                Inicia();
            }
            catch (NullReferenceException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CrearTarjeta_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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
                catch(NullReferenceException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Read(object sender, EventArgs e)
        {
            string data = (string)sender;
            if (!escribirID)
            {
                lblDato.Dispatcher.Invoke(new Action(() => { lblDato.Content = "Enviando dato"; }));
                arduino.Write(id.ToString() + "#");
                escribirID = true;
            }
            else if (!leer)
            {
                lblDato.Dispatcher.Invoke(new Action(() => { lblDato.Content = "Pase tarjeta"; }));
                leer = true;
                guardado = true;
            }
            else if(guardado)
            {
                if(baseDeDatos.AgregarTarjeta())
                    MessageBox.Show("Tarjeta creada", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Error en la creacion", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                guardado = false;
                Inicia();
                //BtnCancelarCrearTarjeta.Dispatcher.Invoke(new Action(() => { BtnCancelarCrearTarjeta.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); }));
            }
        }

        private void BtnCancelarCrearTarjeta_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Win = (MainWindow)Window.GetWindow(this);
            Win.RegresarPantallaInicial();
        }

        private void click_crarTarjeta(object sender, RoutedEventArgs e)
        {
            TransfeririInfoATarjeta();
        }
        private void TransfeririInfoATarjeta()
        {
            Thread.Sleep(1300);
            BtnActivarCrearTarjeta.IsEnabled = false;
            arduino.Write("1");
        }
        void Inicia()
        {
            BtnActivarCrearTarjeta.Dispatcher.Invoke(new Action(() => { BtnActivarCrearTarjeta.IsEnabled = true; }));
            escribirID = false;
            leer = false;
            guardado = false;
            lblDato.Dispatcher.Invoke(new Action(() => { lblDato.Content = ""; }));
            id = baseDeDatos.ObtenerUltimoRegistro();
            tbkID.Dispatcher.Invoke(new Action(() => { tbkID.Text = "ID Usuario: " + id.ToString(); }));
        }
    }
}
