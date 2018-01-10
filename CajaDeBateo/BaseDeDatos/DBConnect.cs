using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace CajaDeBateo.BaseDeDatos
{
    public class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        string message;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "caja_bateo";
            uid = "root";
            password = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("No se pudo conectar al servidor. LLame a soporte", "Error de conexion",MessageBoxButton.OK,MessageBoxImage.Error);
                        break;

                    case 1045:
                        MessageBox.Show("Nombre de usuario o contraseña incorrectas","Error de datos",MessageBoxButton.OK,MessageBoxImage.Error);
                        break;
                }
                return false;
            }
            catch(System.InvalidOperationException e)
            {
                connection.Close();
                connection.Open();
                return true;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error",MessageBoxButton.OK,MessageBoxImage.Error);
                return false;
            }
        }

        //Insert statement
        public bool AgregarTarjeta()
        {
            DateTime fecha = DateTime.Now;
            string query = "INSERT INTO tarjeta  VALUES(NULL, '"+fecha.ToString("dd/MM/yyyy") +"', 1)";

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
                return true;
            }
            return false;
        }
        public bool AgregarCreditoMensual(string idTarjeta)
        {
            DateTime fecha = DateTime.Now;
            DateTime vencimiento = fecha;
            vencimiento=vencimiento.AddMonths(1);
            string query = "INSERT INTO creditos_mensuales  VALUES(" + idTarjeta + ", '" + fecha.ToString("dd/MM/yyyy") + "', '" + vencimiento.ToString("dd/MM/yyyy") + "', 10,10)";//Los dos 10´s se modifican por la informacin extraida del XML que sera de configiracion

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                try
                {
                    cmd.ExecuteNonQuery();
                }catch(Exception e)
                {
                    MessageBox.Show("Este usuario ya tiene los creditos mensuales", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.CloseConnection();
                    return false;
                }
                

                //close connection
                this.CloseConnection();
                return true;
            }
            return false;
        }
        public bool AgregarCreditoAdicion(string idTarjeta, int creditos)
        {
            DateTime fecha = DateTime.Now;
            DateTime vencimiento = fecha;
            vencimiento = vencimiento.AddMonths(1);
            string query = "INSERT INTO creditos_aderidos  VALUES(" + idTarjeta + ", '" + fecha.ToString() + "','" + vencimiento.ToString("dd / MM / yyyy") + "', " + creditos+ "," + creditos + ")";//Los dos 10´s se modifican por la informacin extraida del XML que sera de configiracion

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Este usuario ya tiene los creditos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.CloseConnection();
                    return false;
                }

                //close connection
                this.CloseConnection();
                return true;
            }
            return false;
        }
        //Update statement
        public void ModificarFechaVencimiento()
        {
        }

        //Delete statement
        public void ActivarTarjeta()
        {
        }

        public void DesactivarTarjeta()
        {
        }

        //Select statement
        public int ObtenerUltimoRegistro()
        {
            if(CountTarjetas()>0)
            {
                string query = "SELECT MAX(id_tarjeta) FROM tarjeta;";//Los dos 10´s se modifican por la informacin extraida del XML que sera de configiracion
                int id;
                //open connection
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        id = (int)dataReader.GetInt32(0);
                        id++;
                        return id;
                    }
                    dataReader.Close();
                    this.CloseConnection();
                }
            }
            return 10000;
        }

        //Count statement
        public int CountTarjetas()
        {
            string query = "SELECT Count(*) FROM tarjeta";
            int Count = -1;

            //Open Connection
            if (this.OpenConnection() == true)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                this.CloseConnection();

                return Count;
            }
            else
            {
                return Count;
            }
        }

        //Backup
        public static void Backup()
        {
            try
            {
                DateTime Time = DateTime.Now;
                int year = Time.Year;
                int month = Time.Month;
                int day = Time.Day;
                int hour = Time.Hour;
                int minute = Time.Minute;
                int second = Time.Second;
                int millisecond = Time.Millisecond;

                //Save file to C:\ with the current date as a filename
                string path;
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                path += "\\"+ year + "-" + month + "-" + day +
            "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
                StreamWriter file = new StreamWriter(path);


                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysqldump";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}",
                    "root", "", "localhost", "caja_bateo");
                psi.UseShellExecute = false;

                Process process = Process.Start(psi);

                string output;
                output = process.StandardOutput.ReadToEnd();
                file.WriteLine(output);
                process.WaitForExit();
                file.Close();
                process.Close();
                MessageBox.Show("Respaldo exitoso","Succes",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error , unable to backup!" + ex.Message);
            }
            catch(System.ComponentModel.Win32Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Restore
        public void Restore(string path)
        {
            try
            {
                //Read file from C:\
                StreamReader file = new StreamReader(path);
                string input = file.ReadToEnd();
                file.Close();

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysql";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = false;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}",
                    uid, password, server, database);
                psi.UseShellExecute = false;


                Process process = Process.Start(psi);
                process.StandardInput.WriteLine(input);
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error , unable to Restore!");
            }
        }
    }
}
