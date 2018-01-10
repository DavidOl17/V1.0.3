using CajaDeBateo.BaseDeDatos;
using CajaDeBateo.ComunicacionArduino;
using CajaDeBateo.ControlDeUsuarios;
using CajaDeBateo.Menu;
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
using System.IO;

namespace CajaDeBateo
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Button[] principal,tarjeta,creditos,agregarCreditos,configuracion;
        ControladorMenu principalC, tarjetaC, creditosC, agregarCreditosC, configuracionC;
        ControladorMenus controlador;

        private Control controlDeVista;
        int puertoSeleccionado;
        string[] puertos;
        private int CreditosMensuales;
        private int CreditosMensualesPorDefecto = 15;
        private String ConfigPath;

        private void Reset(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.F1)
            {
                //ardC.Reset();
                stkUSerControlContainer.Children.Remove(controlDeVista);
                controlDeVista = new PantallaInicial();
                stkUSerControlContainer.Children.Add(controlDeVista);
            }
        }

        public void RegresarPantallaInicial()
        {
            stkUSerControlContainer.Children.Remove(controlDeVista);
            controlDeVista = new PantallaInicial();
            stkUSerControlContainer.Children.Add(controlDeVista);
        }

        public MainWindow()
        {
            InitializeComponent();
            InicializaBotones();
            controlDeVista = new PantallaInicial();
            stkUSerControlContainer.Children.Add(controlDeVista);
            SeleccionaArduino sA = new SeleccionaArduino();
            sA.ShowDialog();
            puertoSeleccionado = sA.Index;
            puertos = sA.EP;
            //ardC.RespuestaRecivida += new EventHandler(Recepcion);
            //String Path = System.Reflection.Assembly.GetExecutingAssembly().Location + "\\Config";
            ConfigPath = AppDomain.CurrentDomain.BaseDirectory;
            ConfigPath.Substring(0, ConfigPath.LastIndexOf("\\"));
            ConfigPath = ConfigPath + "Config";
            bool LecturaCorrecta = true;
            if(File.Exists(ConfigPath))
            {
                StreamReader In = new StreamReader(ConfigPath);
                String Cad = In.ReadLine();
                String[] Valores = Cad.Split(new char[] { '=' });
                if (Valores.Count() == 2)
                {
                    int Valor = -1;
                    if (Valores[0] == "CreditosMensuales" && int.TryParse(Valores[1], out Valor))
                    {
                        CreditosMensuales = Valor;
                    }
                    else
                    {
                        LecturaCorrecta = false;
                    }
                }
                else
                {
                    LecturaCorrecta = false;
                }
                In.Close();
            }
            else
            {
                LecturaCorrecta = false;
            }

            if(!LecturaCorrecta)
            {
                if (File.Exists(ConfigPath))
                {
                    File.Delete(ConfigPath);
                }
                StreamWriter Out = new StreamWriter(ConfigPath);
                Out.WriteLine("CreditosMensuales=" + CreditosMensualesPorDefecto.ToString());
                Out.Close();
                CreditosMensuales = CreditosMensualesPorDefecto;
            }
        }

        

        //private void Recepcion(object sender, EventArgs e)
        //{
        //    string data = (string)sender;
        //    MessageBox.Show(data);
        //}

        void InicializaBotones()
        {
            principal = new Button[3];
            principal[0] = btnTarjetas;
            principal[1] = btnCreditos;
            principal[2] = btnConfiguracion;
            principalC = new ControladorMenu(ref principal, 3);

            tarjeta = new Button[3];
            tarjeta[0] = btnCrear;
            tarjeta[1] = btnActivar;
            tarjeta[2] = btnDesactivar;
            tarjetaC = new ControladorMenu(ref tarjeta, 3);

            creditos = new Button[4];
            creditos[0] = btnAgregarCreditos;
            creditos[1] = btnVerCreditos;
            creditos[2] = btnVerCreditosCompletos;
            creditos[3] = btnActualizarCreditos;
            creditosC = new ControladorMenu(ref creditos, 4);

            agregarCreditos = new Button[2];
            agregarCreditos[0] = btnAgregarMensual;
            agregarCreditos[1] = btnMasCreditos;
            agregarCreditosC = new ControladorMenu(ref agregarCreditos, 2);

            configuracion = new Button[3];
            configuracion[0] = btnConfigurarCreditosMensuales;
            configuracion[1] = btnAcercaNosotros;
            configuracion[2] = btnRespaldoBD;
            configuracionC = new ControladorMenu(ref configuracion, 3);

            //Configuracion de eventos
            btnTarjetas.Click += new RoutedEventHandler(btnClick);
            btnCreditos.Click += new RoutedEventHandler(btnClick);
            btnConfiguracion.Click += new RoutedEventHandler(btnClick);
            btnCrear.Click += new RoutedEventHandler(btnClick);
            btnActivar.Click += new RoutedEventHandler(btnClick);
            btnDesactivar.Click += new RoutedEventHandler(btnClick);
            btnAgregarCreditos.Click += new RoutedEventHandler(btnClick);
            btnVerCreditos.Click += new RoutedEventHandler(btnClick);
            btnVerCreditosCompletos.Click += new RoutedEventHandler(btnClick);
            btnActualizarCreditos.Click += new RoutedEventHandler(btnClick);
            btnAgregarMensual.Click += new RoutedEventHandler(btnClick);
            btnMasCreditos.Click += new RoutedEventHandler(btnClick);
            btnRegresar.Click += new RoutedEventHandler(btnClick);
            btnAcercaNosotros.Click += new RoutedEventHandler(btnClick);
            btnConfigurarCreditosMensuales.Click += new RoutedEventHandler(btnClick);
            btnRespaldoBD.Click += new RoutedEventHandler(btnClick);
            controlador = new ControladorMenus(ref principalC, ref tarjetaC, ref creditosC, ref agregarCreditosC, ref configuracionC);
            
        }

        private void btnClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement feSource = e.Source as FrameworkElement;
            switch(feSource.Name)
            {
                case "btnTarjetas":
                    controlador.setTarjetas();
                break;
                case "btnCreditos":
                    controlador.setCreditos();
                    break;
                case "btnConfiguracion":
                    controlador.setConfiguracion();
                    break;
                case "btnRegresar":
                    RegresarPantallaInicial();
                    controlador.setBack();
                    break;
                case "btnAgregarCreditos":
                    controlador.setAgregarCreditos();
                    break;
                case "btnCrear":
                    stkUSerControlContainer.Children.Remove(controlDeVista);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    controlDeVista = new CrearTarjeta(puertoSeleccionado,puertos);
                    stkUSerControlContainer.Children.Add(controlDeVista);
                    break;
                case "btnActivar":
                    stkUSerControlContainer.Children.Remove(controlDeVista);
                    controlDeVista = new ActivarTarjeta(puertoSeleccionado,puertos);
                    stkUSerControlContainer.Children.Add(controlDeVista);
                    break;
                case "btnDesactivar":
                    //stkUSerControlContainer.Children.Remove(controlDeVista);
                    //controlDeVista = new DesactivarTarjeta(ref ardC);
                    //stkUSerControlContainer.Children.Add(controlDeVista);
                    break;
                case "btnAgregarMensual":
                    stkUSerControlContainer.Children.Remove(controlDeVista);
                    controlDeVista = new CargarCreditosMensuales(puertoSeleccionado,puertos);
                    stkUSerControlContainer.Children.Add(controlDeVista);
                    break;
                case "btnMasCreditos":
                    stkUSerControlContainer.Children.Remove(controlDeVista);
                    controlDeVista = new CargarCreditosAdicionales(puertoSeleccionado,puertos);
                    stkUSerControlContainer.Children.Add(controlDeVista);
                    break;
                case "btnRespaldoBD":
                    DBConnect.Backup();
                    break;
                case "btnConfigurarCreditosMensuales":
                    stkUSerControlContainer.Children.Remove(controlDeVista);
                    controlDeVista = new Configuracion(CreditosMensuales, ConfigPath);
                    stkUSerControlContainer.Children.Add(controlDeVista);
                    break;
            }
        }
    }
}
