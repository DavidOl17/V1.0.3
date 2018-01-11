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
using System.Windows.Controls;


namespace CajaDeBateo.BaseDeDatos
{
    public class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        //string message;

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
                String Val = e.Message;
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
            string query = "INSERT INTO tarjeta  VALUES(NULL, '"+fecha.ToString("dd/MM/yyyy") +"', 0)";

            //open connection
            if (this.OpenConnection())
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

        public bool AgregarCreditoMensual(string idTarjeta, int Creditos)
        {
            DateTime fecha = DateTime.Now;
            DateTime vencimiento = fecha;
            vencimiento=vencimiento.AddMonths(1);
            string query = "INSERT INTO creditos_mensuales VALUES(" + idTarjeta + "," +
                fecha.ToString("dd/MM/yyyy") + ", STR_TO_DATE(\'" + vencimiento.ToString("dd/MM/yyyy") +
                "\',\'%d/%m/%Y\')" + "," + Creditos.ToString() + "," + Creditos.ToString() + ")";

            //open connection
            if (this.OpenConnection())
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    String Mensaje = e.Message;
                    if(Mensaje.Contains("Duplicate"))
                    {
                        MessageBox.Show("Este usuario ya tiene los creditos mensuales.", "Creditos ya cargados", 
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (Mensaje.Contains("CONSTRAINT"))
                    {
                        MessageBox.Show("La tarjeta no se cuentra registrada. ", "Usuario inexistente",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        MessageBox.Show("Error en concección. Contacte a soporte." + Mensaje, "Error", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    
                    this.CloseConnection();
                    return false;
                }
                catch (Exception e)
                {
                    String Val = e.Message;
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
            string query = "INSERT INTO creditos_aderidos VALUES(" + idTarjeta + "," +
                fecha.ToString("dd/MM/yyyy HH:mm:ss") + ", STR_TO_DATE(\'" + 
                vencimiento.ToString("dd/MM/yyyy") + "\',\'%d/%m/%Y\')" + "," + 
                creditos.ToString() + "," + creditos.ToString() + ")";

            //open connection
            if (this.OpenConnection())
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
                    String Val = e.Message;
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

        public void ModificarFechaVencimiento(String id_tarjeta, String FechaAdicion,
            String NuevoVencimiento, String Tabla)
        {
            string query = "";

            if(Tabla == "Mensuales")
            {
                query = "UPDATE creditos_mensuales " +
                    "SET fecha_vencimiento = STR_TO_DATE(\'" + NuevoVencimiento + "\',\'%d/%m/%Y\') " +
                    "WHERE id_tarjeta = " + id_tarjeta + " AND fecha_adicion = \'" + FechaAdicion + "\'";
            }
            else if(Tabla == "Adicionales")
            {
                query = "UPDATE creditos_aderidos " +
                    "SET fecha_vencimiento = STR_TO_DATE(\'" + NuevoVencimiento + "\',\'%d/%m/%Y\') " +
                    "WHERE id_tarjeta = " + id_tarjeta + " AND fecha_adicion = \'" + FechaAdicion + "\'";
            }

            Console.WriteLine(query);
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    String Val = e.Message;
                    MessageBox.Show("Error al ejecutar la petición. Contacte a soporte. " + 
                        e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.CloseConnection();
                }
                this.CloseConnection();
            }
        }

        public int ActDesactTarjeta(String id_tarjeta, int Accion)
        {
            // 0 Correcto
            // 1 Tarjeta actualmente activada o desactivaao / no es necesario realizar accion
            // 2 Tarjeta no existen
            // 3 Error de acceso a base de datos
            string query = "SELECT status FROM tarjeta WHERE id_tarjeta = " + id_tarjeta;
            int status = -1;
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader Reader = cmd.ExecuteReader();
                int Contador = 0;
                while (Reader.Read())
                {
                    Contador++;
                    status = int.Parse(Reader[0].ToString());
                }

                if (Contador == 0)
                    return 2;

                if (status == Accion)
                    return 1;
                this.CloseConnection();
            }
            else
            {
                MessageBox.Show("Error al conectar a la base de datos. Contacte a soporte.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return 3;
            }

            query = "UPDATE tarjeta SET status = " + Accion.ToString() + " WHERE id_tarjeta = " + id_tarjeta;
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                return 0;
            }
            else
            {
                MessageBox.Show("Error al conectar a la base de datos. Contacte a soporte.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return 3;
            }
        }

        //Select statement
        public int ObtenerUltimoRegistro()
        {
            if(CountTarjetas()>0)
            {
                string query = "SELECT MAX(id_tarjeta) FROM tarjeta;";//Los dos 10´s se modifican por la informacin extraida del XML que sera de configiracion
                int id;
                //open connection
                if (this.OpenConnection())
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
            if (this.OpenConnection())
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
            DateTime Time = DateTime.Now;
            int year = Time.Year;
            int month = Time.Month;
            int day = Time.Day;
            int hour = Time.Hour;
            int minute = Time.Minute;
            int second = Time.Second;
            int millisecond = Time.Millisecond;

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + year + "-" + month + "-" + 
                day + "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
            try
            {
                //Save file to C:\ with the current date as a filename
                StreamWriter file = new StreamWriter(path); ;

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysqldump";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}","root", "", "localhost", "caja_bateo");
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
            catch (IOException e)
            {
                MessageBox.Show("Error al realizar el respaldo. " + e.Message, "Error", MessageBoxButton.OK,MessageBoxImage.Error);
            }
            catch(System.ComponentModel.Win32Exception e)
            {
                MessageBox.Show("Error al acceder a la base de datos. " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Error al recuperar datos del respaldo. " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<String> ObtenerCreditosActivos(String idTarjeta)
        {
            List<String> Datos = new List<string>();
            List<String> Aux = new List<string>()
            {
                "."
            };

            string query = "SELECT * FROM creditos_mensuales WHERE id_tarjeta = " + idTarjeta +
                " AND fecha_vencimiento > CURRENT_DATE() AND creditos_disponibles > 0" +
                " UNION " +
                "SELECT * FROM creditos_aderidos WHERE id_tarjeta = " + idTarjeta +
                " AND fecha_vencimiento > CURRENT_DATE() AND creditos_disponibles > 0";
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    String Tipo = "";
                    String ID = Reader[0].ToString();
                    String Adicion = Reader[1].ToString();
                    String Vencimiento = Reader[2].ToString();
                    String Aderidos = Reader[3].ToString();
                    String Disponibles = Reader[4].ToString();

                    Vencimiento = Vencimiento.Substring(0, 10);
                    int Index = Adicion.LastIndexOf(" ");
                    if (Index > 0)
                    {
                        Tipo = "Adicionales";
                        Adicion = Adicion.Substring(0, Index);
                    }
                    else
                    {
                        Tipo = "Mensuales";
                    }

                    String CadAux = Tipo + "|" + Adicion + "|" + Vencimiento + "|" + Aderidos + "|" + Disponibles;
                    Datos.Add(CadAux);
                }
                this.CloseConnection();
            }
            else
            {
                return Aux;
            }
            return Datos;
        }

        public List<String> ObtenerHistorial(String idTarjeta)
        {
            List<String> Datos = new List<string>();
            List<String> Aux = new List<string>()
            {
                "."
            };

            string query = "SELECT * FROM creditos_mensuales WHERE id_tarjeta = " + idTarjeta +
                " UNION " +
                "SELECT * FROM creditos_aderidos WHERE id_tarjeta = " + idTarjeta;
            Console.WriteLine(query);
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    String Tipo = "";
                    String ID = Reader[0].ToString();
                    String Adicion = Reader[1].ToString();
                    String Vencimiento = Reader[2].ToString();
                    String Aderidos = Reader[3].ToString();
                    String Disponibles = Reader[4].ToString();

                    Vencimiento = Vencimiento.Substring(0, 10);
                    int Index = Adicion.LastIndexOf(" ");
                    if (Index > 0)
                    {
                        Tipo = "Adicionales";
                        Adicion = Adicion.Substring(0, Index);
                    }
                    else
                    {
                        Tipo = "Mensuales";
                    }

                    String CadAux = Tipo + "|" + Adicion + "|" + Vencimiento + "|" + Aderidos + "|" + Disponibles;
                    Datos.Add(CadAux);
                }
                this.CloseConnection();
            }
            else
            {
                return Aux;
            }
            return Datos;
        }

        public List<String> ObtenerConCreditosDisponibles(String idTarjeta)
        {
            List<String> Datos = new List<string>();
            List<String> Aux = new List<string>()
            {
                "."
            };

            string query = "SELECT * FROM creditos_mensuales WHERE id_tarjeta = " + idTarjeta + " AND creditos_disponibles > 0" +
                " UNION " +
                "SELECT * FROM creditos_aderidos WHERE id_tarjeta = " + idTarjeta + " AND creditos_disponibles > 0";
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    String Tipo = "";
                    String ID = Reader[0].ToString();
                    String Adicion = Reader[1].ToString();
                    String Vencimiento = Reader[2].ToString();
                    String Aderidos = Reader[3].ToString();
                    String Disponibles = Reader[4].ToString();

                    Vencimiento = Vencimiento.Substring(0, 10);
                    int Index = Adicion.LastIndexOf(" ");
                    if (Index > 0)
                        Tipo = "Adicionales";
                    else
                        Tipo = "Mensuales";

                    String CadAux = Tipo + "|" + Adicion + "|" + Vencimiento + "|" + Aderidos + "|" + Disponibles;
                    Datos.Add(CadAux);
                }
                this.CloseConnection();
            }
            else
            {
                return Aux;
            }
            return Datos;
        }
    }

}
