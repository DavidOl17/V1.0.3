﻿using CajaDeBateo.BaseDeDatos;
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
    /// Interaction logic for CargarCreditosMensuales.xaml
    /// </summary>
    public partial class CargarCreditosMensuales : UserControl
    {
        ArduinoComunication arduino;
        bool primera = false;
        DBConnect baseDeDatos;
        int id;
        private int CreditosMensuales;

        public CargarCreditosMensuales(int puerto, string[] puertos, int ValCreditosMensuales)
        {
            InitializeComponent();
            CreditosMensuales = ValCreditosMensuales;
            baseDeDatos = new DBConnect();
            BtnGuardarCargarCreditosMensuales.IsEnabled = false;
            try
            {
                arduino = new ArduinoComunication(puerto, puertos);
                arduino.RespuestaRecivida += new EventHandler(Read);
                arduino.Write("2");
                IDUusuarioCargarCreditosMensuales.Content = "Pase la tarjeta";
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
            //arduino.Reset();
            this.IsVisibleChanged += ActivarTarjeta_IsVisibleChanged;
        }

        private void ActivarTarjeta_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!primera)
                primera = true;
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

        private void Read(object sender, EventArgs e)
        {
            string data = (string)sender;
            id = int.Parse(data);
            tbkIDUsuarios.Dispatcher.Invoke(new Action(() => { tbkIDUsuarios.Text ="ID Usuario: "; }));
            IDUusuarioCargarCreditosMensuales.Dispatcher.Invoke(new Action(() => { IDUusuarioCargarCreditosMensuales.Content = data; }));
            BtnGuardarCargarCreditosMensuales.Dispatcher.Invoke(new Action(() => { BtnGuardarCargarCreditosMensuales.IsEnabled = true; }));
        }

        private void BtnCancelarCargarCreditosMensuales_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Win = (MainWindow)Window.GetWindow(this);
            Win.RegresarPantallaInicial();
        }

        private void click_Guardar(object sender, RoutedEventArgs e)
        {
            if(baseDeDatos.AgregarCreditoMensual(id.ToString(), CreditosMensuales))
            {
                MessageBox.Show("Creditos agregados", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            IDUusuarioCargarCreditosMensuales.Content = "Pase la tarjeta";
            BtnGuardarCargarCreditosMensuales.IsEnabled = false;
            tbkIDUsuarios.Text = "ID Usuarios: ";
            try
            {
                arduino.Write("2");
            }
            catch(NullReferenceException ex)
            {
                String Val = ex.Message;
            }
        }
    }
}
