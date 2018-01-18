using CajaDeBateo.BaseDeDatos;
using CajaDeBateo.ComunicacionArduino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Threading;

namespace CajaDeBateo.ControlDeUsuarios
{
    /// <summary>
    /// Interaction logic for Actualizar.xaml
    /// </summary>
    public partial class Actualizar : UserControl
    {
        private ArduinoComunication arduino;
        private Label aux;
        private bool primera = false;
        private DBConnect baseDeDatos;
        private bool ArduinoConectado;
        private String AuxPerm = "";
        private String AuxTem = "";
        private String IdActual;
        private DateTime FechaSeleccionada;
        private int FilaSeleccionada;
        private string data;
        private DataTable Tabla;

        public Actualizar(int puerto, string[] puertos)
        {
            InitializeComponent();
            FechaSeleccionada = new DateTime();

            CalendarActualizar.DisplayDateStart = DateTime.Today;
            try
            {
                baseDeDatos = new DBConnect();
                arduino = new ArduinoComunication(puerto, puertos);
                arduino.RespuestaRecivida += new EventHandler(Read);
                aux = lblDato;
                arduino.Write("2");
                lblDato.Content = "Pase la tarjeta";
                ArduinoConectado = true;
                this.IsVisibleChanged += Actualizar_IsVisibleChanged;
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

        private void Actualizar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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

        private void Read(object sender, EventArgs e)
        {
            data = (string)sender;
            lblDato.Dispatcher.Invoke(new Action(() => { lblDato.Content = data; }));
            Dispatcher.Invoke(delegate { ObtenerDatos(false); });
        }

        private void BtnCancelarActualizar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Win = (MainWindow)Window.GetWindow(this);
            Win.RegresarPantallaInicial();
        }

        private void BtnBuscarActualizar_Click(object sender, RoutedEventArgs e)
        {
            ObtenerDatos(false);
            if (ArduinoConectado)
            {
                BtnBuscarActualizar.IsEnabled = false;
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

        private void ObtenerDatos(bool Actualizar)
        {
            String Arg;
            if (Actualizar)
            {
                Arg = IdActual;
            }
            else
            {
                if (ArduinoConectado)
                    Arg = lblDato.Content.ToString();
                else
                    Arg = TlblDato.Text;
            }
            IdActual = Arg;
            Limpiar();
            Tabla = new DataTable();
            Tabla = baseDeDatos.ObtenerConCreditosDisponibles(Arg);
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
                DataGridActualizar.ItemsSource = Tabla.DefaultView;
                lblDato.Content = "Pase la tarjeta";
            }
        }

        private void Limpiar()
        {
            DataGridActualizar.ItemsSource = new DataTable().DefaultView;
            DataGridActualizar.Visibility = Visibility.Visible;
            ContCalendarActualizar.Visibility = Visibility.Hidden;
            //BtnGuardarActualizar.IsEnabled = false;
            BtnBuscarActualizar.IsEnabled = true;
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
            BtnBuscarActualizar.IsEnabled = (AuxTem.Length > 0);
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
                ObtenerDatos(false);
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow Row = sender as DataGridRow;
            FilaSeleccionada = Row.GetIndex();

            DataGridActualizar.Visibility = Visibility.Hidden;
            ContCalendarActualizar.Visibility = Visibility.Visible;
            CalendarActualizar.SelectedDate = null;
            BtnCalendarAceptar.IsEnabled = false;
        }

        private void BtnCalendarAceptar_Click(object sender, RoutedEventArgs e)
        {
            DataGridActualizar.Visibility = Visibility.Visible;
            ContCalendarActualizar.Visibility = Visibility.Hidden;
            FechaSeleccionada = CalendarActualizar.SelectedDate.Value;
            BtnCalendarAceptar.IsEnabled = false;
            baseDeDatos.ModificarFechaVencimiento(IdActual,
                Tabla.Rows[FilaSeleccionada].Field<String>(1),
                FechaSeleccionada.ToString("dd/MM/yyyy"),
                Tabla.Rows[FilaSeleccionada].Field<String>(0));
            ObtenerDatos(true);
        }

        private void BtnCalendarCancelar_Click(object sender, RoutedEventArgs e)
        {
            DataGridActualizar.Visibility = Visibility.Visible;
            ContCalendarActualizar.Visibility = Visibility.Hidden;
            FechaSeleccionada = new DateTime();
        }

        private void CalendarActualizar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!BtnCalendarAceptar.IsEnabled)
                BtnCalendarAceptar.IsEnabled = true;
        }
    }
}
