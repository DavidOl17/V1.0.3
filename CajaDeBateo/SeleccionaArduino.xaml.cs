using CajaDeBateo.ComunicacionArduino;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace CajaDeBateo
{
    /// <summary>
    /// Lógica de interacción para SeleccionaArduino.xaml
    /// </summary>
    public partial class SeleccionaArduino : Window
    {
        ArduinoComunication configuration = new ArduinoComunication();
        string[] eP;
        int index;
        public SeleccionaArduino()
        {
            InitializeComponent();
            cmbPuertos.SelectedIndex = 0;
            try
            {
                eP = configuration.ExistingPortsOnly();
                ObservableCollection<string> list = new ObservableCollection<string>(eP);
                cmbPuertos.ItemsSource = list;
            }
            catch (SensorNotFoundExceptio e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK);
            }
            catch(Exception)
            {
                throw new SensorNotFoundExceptio("Ningun arduino detextado");
            }
        }

        public string[] EP { get => eP;}
        public int Index { get => index; }

        private void click(object sender, RoutedEventArgs e)
        {
            if(!cmbPuertos.Items.Count.Equals(0))
            {
                index = cmbPuertos.SelectedIndex;
                this.Close();
            }
        }
    }
}
