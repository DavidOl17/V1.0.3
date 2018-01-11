using CajaDeBateo.BaseDeDatos;
using CajaDeBateo.ComunicacionArduino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;

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
                lblDato.Content = "Pase la tarjeta";
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

            Limpiar();
            List<String> Datos = baseDeDatos.ObtenerConCreditosDisponibles(Arg);
            if (Datos.Count == 1 && Datos.ElementAt(0) == ".")
            {
                MessageBox.Show("Error al realizar la consulta a la base de datos. Llame a soporte. ", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Datos.Count == 0)
            {
                MessageBox.Show("No se encontraron cargas de créditos para el usuario seleccionado.", "Sin resultados",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                Tabla = new DataTable();

                Tabla.Columns.Add(new DataColumn("Tipo", typeof(String)));
                Tabla.Columns.Add(new DataColumn("Fecha", typeof(String)));
                Tabla.Columns.Add(new DataColumn("Vencimiento", typeof(String)));
                Tabla.Columns.Add(new DataColumn("Creditos_Adquiridos", typeof(String)));
                Tabla.Columns.Add(new DataColumn("Creditos_Disponibles", typeof(String)));

                for (int i = 0; i < Datos.Count; i++)
                {
                    String Aux = Datos.ElementAt(i);
                    String[] ListaAux = Aux.Split(new[] { '|' }, StringSplitOptions.None);

                    DataRow Fila = Tabla.NewRow();
                    for (int j = 0; j < ListaAux.Count(); j++)
                    {
                        Fila[j] = ListaAux[j];
                    }
                    Tabla.Rows.Add(Fila);
                }
                DataGridActualizar.ItemsSource = Tabla.DefaultView;
                ContCalendarActualizar.Visibility = Visibility.Hidden;
                DataGridActualizar.Visibility = Visibility.Visible;
                //BtnGuardarActualizar.IsEnabled = false;
                IdActual = Arg;
            }
        }

        private void Limpiar()
        {
            DataGridActualizar.ItemsSource = new DataTable().DefaultView;
            DataGridActualizar.Visibility = Visibility.Visible;
            ContCalendarActualizar.Visibility = Visibility.Hidden;
            //BtnGuardarActualizar.IsEnabled = false;
            BtnBuscarActualizar.IsEnabled = true;
            IdActual = "";
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
            //BtnGuardarActualizar.IsEnabled = true;
            BtnCalendarAceptar.IsEnabled = false;

            /*Verificar si la fecha de vencimiento del elemento ubicado en la fila FilaActual es
            diferente a FechaSeleccionada. Si es así modificarlo en la base de datos*/
            //d.Rows[0].Field<string>(3);
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
