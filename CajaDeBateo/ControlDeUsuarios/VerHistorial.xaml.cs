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

namespace CajaDeBateo.ControlDeUsuarios
{
    /// <summary>
    /// Interaction logic for VerHistorial.xaml
    /// </summary>
    public partial class VerHistorial : UserControl
    {
        ArduinoComunication arduino;
        Label aux;
        bool primera = false;
        DBConnect baseDeDatos;

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
            this.IsVisibleChanged += VerHistorial_IsVisibleChanged;
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
        }

        private void BtnCancelarVerHistorial_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Win = (MainWindow)Window.GetWindow(this);
            Win.RegresarPantallaInicial();
        }

        private void BtnBuscarVerHistorial_Click(object sender, RoutedEventArgs e)
        {
            List<String> Datos = baseDeDatos.ObtenerHistorial(lblDato.Content.ToString());
            if (Datos.Count == 0)
            {
                MessageBox.Show("No se encontraron cargas de créditos para el usuario seleccionado.", "Sin resultados",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                DataTable Tabla = new DataTable();

                Tabla.Columns.Add(new DataColumn("Tipo", typeof(String)));
                Tabla.Columns.Add(new DataColumn("Fecha", typeof(String)));
                Tabla.Columns.Add(new DataColumn("Vencimiento", typeof(String)));
                Tabla.Columns.Add(new DataColumn("Créditos Adquiridos", typeof(String)));
                Tabla.Columns.Add(new DataColumn("Créditos Disponibles", typeof(String)));

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
                DataGridHistorial.ItemsSource = Tabla.DefaultView;
            }

            lblDato.Content = "Pase la tarjeta";
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
}
