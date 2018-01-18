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
using System.Data;


namespace CajaDeBateo.BaseDeDatos
{
    public class DBConnect
    {
        private String Verificador = "<>73WZK4TRG9CFXX4FYCG5NHA3FXVW7D<>";

        MySqlConnection connection;
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
            //server = "192.168.100.27"; password = "12345"; uid = "caja";
            server = "localhost"; password = ""; uid = "root";
            database = "caja_bateo";
            
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + 
                "UID=" + uid + ";" + "PASSWORD=" + password + ";";

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
                        MessageBox.Show("No se pudo conectar al servidor. LLame a soporte", 
                            "Error de conexion",MessageBoxButton.OK,MessageBoxImage.Error);
                        break;

                    case 1045:
                        MessageBox.Show("Nombre de usuario o contraseña incorrectas",
                            "Error de datos",MessageBoxButton.OK,MessageBoxImage.Error);
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
            string query = "INSERT INTO creditos_mensuales VALUES(" + idTarjeta + ",'" +
                fecha.ToString("dd/MM/yyyy") + "',STR_TO_DATE(\'" + vencimiento.ToString("dd/MM/yyyy") +
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
            string query = "INSERT INTO creditos_aderidos VALUES(" + idTarjeta + ",'" +
                fecha.ToString("dd/MM/yyyy HH:mm:ss") + "',STR_TO_DATE(\'" + 
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
        public void Backup()
        {
            DateTime Time = DateTime.Now;
            String NombreArchivo = Time.Year + "-" + Time.Month + "-" + Time.Day + " " + 
                Time.Hour + "-" + Time.Minute + "-" + Time.Second;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".cbku";
            dlg.Filter = "(*.bakcb)|*.bakcb";
            dlg.FileName = NombreArchivo;
            bool result = dlg.ShowDialog() ?? default(bool);

            if (result)
            {
                string q1 = "SELECT * FROM tarjeta";
                string q2 = "SELECT * FROM creditos_aderidos";
                string q3 = "SELECT * FROM creditos_mensuales";
                if(File.Exists(dlg.FileName))
                  File.Delete(dlg.FileName);
                StreamWriter Out = new StreamWriter(dlg.FileName);

                Out.WriteLine(Verificador);
                Out.WriteLine("//Tostatronic A.C. de C.V. no se hace responsable por daños o perdidas causados a sus datos " +
                    "provocados por la modificación de este archivo");

                if (this.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(q1, connection);
                    MySqlDataReader Reader = cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        String Aux = "INSERT INTO tarjeta VALUES (" + Reader[0].ToString() +
                            ",'" + Reader[1].ToString() + "'," + Reader[2].ToString() + ")";
                        Out.WriteLine(Aux);
                        int Auto = int.Parse(Reader[0].ToString()) + 1;
                    }
                    this.CloseConnection();

                    this.OpenConnection();
                    cmd = new MySqlCommand(q2, connection);
                    Reader = cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        String Vencimiento = Reader[2].ToString();
                        Vencimiento = Vencimiento.Substring(0, Vencimiento.IndexOf(" "));
                        String Aux = "INSERT INTO creditos_aderidos VALUES (" + Reader[0].ToString() +
                            ",'" + Reader[1].ToString() + "'," + "STR_TO_DATE('" + Vencimiento +
                            "','%d/%m/%Y')," + Reader[3].ToString() + "," + Reader[4].ToString() + ")";
                        Out.WriteLine(Aux);
                    }
                    this.CloseConnection();

                    this.OpenConnection();
                    cmd = new MySqlCommand(q3, connection);
                    Reader = cmd.ExecuteReader();
                    while (Reader.Read())
                    {
                        String Vencimiento = Reader[2].ToString();
                        Vencimiento = Vencimiento.Substring(0, Vencimiento.IndexOf(" "));
                        String Aux = "INSERT INTO creditos_mensuales VALUES (" + Reader[0].ToString() +
                            ",'" + Reader[1].ToString() + "'," + "STR_TO_DATE('" + Vencimiento +
                            "','%d/%m/%Y')," + Reader[3].ToString() + "," + Reader[4].ToString() + ")";
                        Out.WriteLine(Aux);
                    }
                    this.CloseConnection();
                    Out.WriteLine(Verificador);
                }
                else
                {
                    MessageBox.Show("Error al realizar el respaldo. Contacte a soporte. ", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Out.Close();
                MessageBox.Show("Respaldo realizado con éxito. ", "Completado", MessageBoxButton.OK);
            }
        }

        //Restore
        public void Restore()
        {
            MessageBoxResult Boxresult = MessageBox.Show("¿Reemplazar datos actuales con guardados en respaldo?", "Confirmación", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (Boxresult == MessageBoxResult.Yes)
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".cbku";
                dlg.Filter = "(*.bakcb)|*.bakcb";
                bool result = dlg.ShowDialog() ?? default(bool);
                bool ArchivoCorrecto;

                string q1 = "DROP TABLE IF EXISTS creditos_mensuales, creditos_aderidos,tarjeta";
                string q2 = "CREATE TABLE tarjeta (id_tarjeta int(11) NOT NULL, fecha_creacion varchar(20) NOT NULL," +
                    "status int(11) NOT NULL)";
                string q3 = "CREATE TABLE creditos_aderidos (id_tarjeta int(11) NOT NULL,fecha_adicion varchar(25) NOT NULL," +
                    "fecha_vencimiento date NOT NULL,creditos_aderidos int(11) NOT NULL,creditos_disponibles int(11) NOT NULL)";
                string q4 = "CREATE TABLE creditos_mensuales (id_tarjeta int(11) NOT NULL,fecha_adicion varchar(15) NOT NULL," +
                    "fecha_vencimiento date NOT NULL,creditos_aderidos int(11) NOT NULL,creditos_disponibles int(11) NOT NULL)";

                if (result)
                {
                    if (this.OpenConnection())
                    {
                        StreamReader In = new StreamReader(dlg.FileName);
                        bool IdentI = false;
                        bool IdentF = false;
                        bool PrimerasLinea = true;
                        string Aux;
                        ArchivoCorrecto = true;

                        Aux = In.ReadLine();
                        if (Aux == null || Aux != Verificador)
                        {
                            MessageBox.Show("Archivo no compatible o desconocido.", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            ArchivoCorrecto = false;
                        }
                        else
                        {
                            IdentI = true;
                        }
                        MySqlCommand cmd;
                        if (ArchivoCorrecto)
                        {
                            cmd = new MySqlCommand(q1, connection);
                            cmd.ExecuteNonQuery();
                            this.CloseConnection();

                            this.OpenConnection();
                            cmd = new MySqlCommand(q2, connection);
                            cmd.ExecuteNonQuery();
                            this.CloseConnection();

                            this.OpenConnection();
                            cmd = new MySqlCommand(q3, connection);
                            cmd.ExecuteNonQuery();
                            this.CloseConnection();

                            this.OpenConnection();
                            cmd = new MySqlCommand(q4, connection);
                            cmd.ExecuteNonQuery();
                            this.CloseConnection();
                        }
                        while (!In.EndOfStream && ArchivoCorrecto)
                        {
                            Aux = In.ReadLine();
                            if (Aux == Verificador && !IdentF)
                            {
                                IdentF = true;
                                break;
                            }

                            if (PrimerasLinea && IdentI)
                            {
                                PrimerasLinea = false;
                            }
                            else
                            {
                                this.OpenConnection();
                                cmd = new MySqlCommand(Aux, connection);
                                if (cmd.ExecuteNonQuery() < 1)
                                {
                                    Boxresult = MessageBox.Show("Error al realizar consulta. Archivo posiblemente corrupto." +
                                        "¿Desea continuar?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                    if (Boxresult == MessageBoxResult.No)
                                        break;
                                }
                                this.CloseConnection();
                            }
                        }
                        if (!IdentF)
                        {
                            MessageBox.Show("Archivo incompleto. Posible pérdida de datos. ", "Error",
                                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        if (ArchivoCorrecto)
                        {
                            this.OpenConnection();
                            Aux = "ALTER TABLE tarjeta ADD PRIMARY KEY(id_tarjeta)";
                            cmd = new MySqlCommand(Aux, connection);
                            cmd.ExecuteNonQuery();
                            this.CloseConnection();

                            this.OpenConnection();
                            Aux = "ALTER TABLE tarjeta MODIFY id_tarjeta int(11) NOT NULL AUTO_INCREMENT";
                            cmd = new MySqlCommand(Aux, connection);
                            cmd.ExecuteNonQuery();
                            this.CloseConnection();

                            this.OpenConnection();
                            Aux = "ALTER TABLE creditos_mensuales ADD PRIMARY KEY(id_tarjeta,fecha_adicion)";
                            cmd = new MySqlCommand(Aux, connection);
                            cmd.ExecuteNonQuery();
                            this.CloseConnection();

                            this.OpenConnection();
                            Aux = "ALTER TABLE creditos_mensuales ADD CONSTRAINT tarjeta_mensual_res FOREIGN KEY(id_tarjeta)" +
                                " REFERENCES tarjeta (id_tarjeta) ON DELETE CASCADE ON UPDATE CASCADE";
                            cmd = new MySqlCommand(Aux, connection);
                            cmd.ExecuteNonQuery();
                            this.CloseConnection();

                            this.OpenConnection();
                            Aux = "ALTER TABLE creditos_aderidos ADD PRIMARY KEY (id_tarjeta,fecha_adicion)";
                            cmd = new MySqlCommand(Aux, connection);
                            cmd.ExecuteNonQuery();
                            this.CloseConnection();

                            this.OpenConnection();
                            Aux = "ALTER TABLE creditos_aderidos ADD CONSTRAINT tarjeta_adicionales_res FOREIGN KEY(id_tarjeta) " +
                                "REFERENCES tarjeta (id_tarjeta) ON DELETE CASCADE ON UPDATE CASCADE";
                            cmd = new MySqlCommand(Aux, connection);
                            cmd.ExecuteNonQuery();
                            this.CloseConnection();

                            In.Close();
                            MessageBox.Show("Recuperación completada. ", "Completado", MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error al iniciar conección. Consulte a soporte. ", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public DataTable ObtenerCreditosActivos(String idTarjeta)
        {
            string query = "SELECT * FROM creditos_mensuales WHERE id_tarjeta = " + idTarjeta +
                " AND fecha_vencimiento > CURRENT_DATE() AND creditos_disponibles > 0" +
                " UNION " +
                "SELECT * FROM creditos_aderidos WHERE id_tarjeta = " + idTarjeta +
                " AND fecha_vencimiento > CURRENT_DATE() AND creditos_disponibles > 0";

            string querycom = "SELECT * FROM tarjeta WHERE id_tarjeta = " + idTarjeta;
            DataTable Tabla = new DataTable();
            Tabla.Columns.Add(new DataColumn("Tipo", typeof(String)));
            Tabla.Columns.Add(new DataColumn("Fecha", typeof(String)));
            Tabla.Columns.Add(new DataColumn("Vencimiento", typeof(String)));
            Tabla.Columns.Add(new DataColumn("Créditos Adquiridos", typeof(String)));
            Tabla.Columns.Add(new DataColumn("Créditos Disponibles", typeof(String)));

            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(querycom, connection);
                MySqlDataReader Reader = cmd.ExecuteReader();
                int cont = 0;
                while(Reader.Read())
                {
                    cont++;
                }
                if (cont == 0)
                {
                    DataRow Fila = Tabla.NewRow();
                    Tabla.Rows.Add(Fila);
                    return Tabla;
                }
            }
            else
            {
                return null;
            }
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

                    DataRow Fila = Tabla.NewRow();
                    Tabla.Rows.Add(Fila);
                    Fila[0] = Tipo;
                    Fila[1] = Adicion;
                    Fila[2] = Vencimiento;
                    Fila[3] = Aderidos;
                    Fila[4] = Disponibles;
                }
                this.CloseConnection();
            }
            else
            {
                return null;
            }
            return Tabla;
        }

        public DataTable ObtenerHistorial(String idTarjeta)
        {
            string query = "SELECT * FROM creditos_mensuales WHERE id_tarjeta = " + idTarjeta +
                " UNION " +
                "SELECT * FROM creditos_aderidos WHERE id_tarjeta = " + idTarjeta;
            string querycom = "SELECT * FROM tarjeta WHERE id_tarjeta = " + idTarjeta;
            DataTable Tabla = new DataTable();
            Tabla.Columns.Add(new DataColumn("Tipo", typeof(String)));
            Tabla.Columns.Add(new DataColumn("Fecha", typeof(String)));
            Tabla.Columns.Add(new DataColumn("Vencimiento", typeof(String)));
            Tabla.Columns.Add(new DataColumn("Créditos Adquiridos", typeof(String)));
            Tabla.Columns.Add(new DataColumn("Créditos Disponibles", typeof(String)));
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(querycom, connection);
                MySqlDataReader Reader = cmd.ExecuteReader();
                int cont = 0;
                while (Reader.Read())
                {
                    cont++;
                }
                if (cont == 0)
                {
                    DataRow Fila = Tabla.NewRow();
                    Tabla.Rows.Add(Fila);
                    return Tabla;
                }
            }
            else
            {
                return null;
            }
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

                    DataRow Fila = Tabla.NewRow();
                    Tabla.Rows.Add(Fila);
                    Fila[0] = Tipo;
                    Fila[1] = Adicion;
                    Fila[2] = Vencimiento;
                    Fila[3] = Aderidos;
                    Fila[4] = Disponibles;
                }
                this.CloseConnection();
            }
            else
            {
                return null;
            }
            return Tabla;
        }

        public DataTable ObtenerConCreditosDisponibles(String idTarjeta)
        {
            string query = "SELECT * FROM creditos_mensuales WHERE id_tarjeta = " + idTarjeta + " AND creditos_disponibles > 0" +
                " UNION " +
                "SELECT * FROM creditos_aderidos WHERE id_tarjeta = " + idTarjeta + " AND creditos_disponibles > 0";
            string querycom = "SELECT * FROM tarjeta WHERE id_tarjeta = " + idTarjeta;
            DataTable Tabla = new DataTable();
            Tabla.Columns.Add(new DataColumn("Tipo", typeof(String)));
            Tabla.Columns.Add(new DataColumn("Fecha", typeof(String)));
            Tabla.Columns.Add(new DataColumn("Vencimiento", typeof(String)));
            Tabla.Columns.Add(new DataColumn("Créditos Adquiridos", typeof(String)));
            Tabla.Columns.Add(new DataColumn("Créditos Disponibles", typeof(String)));
            if (this.OpenConnection())
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(querycom, connection);
                    MySqlDataReader Reader = cmd.ExecuteReader();
                    int cont = 0;
                    while (Reader.Read())
                    {
                        cont++;
                    }
                    if (cont == 0)
                    {
                        DataRow Fila = Tabla.NewRow();
                        Tabla.Rows.Add(Fila);
                        return Tabla;
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                return null;
            }
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
                    }
                    else
                    {
                        Tipo = "Mensuales";
                    }

                    DataRow Fila = Tabla.NewRow();
                    Tabla.Rows.Add(Fila);
                    Fila[0] = Tipo;
                    Fila[1] = Adicion;
                    Fila[2] = Vencimiento;
                    Fila[3] = Aderidos;
                    Fila[4] = Disponibles;
                }
                this.CloseConnection();
            }
            else
            {
                return null;
            }
            return Tabla;
        }
    }

}
