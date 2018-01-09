using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CajaDeBateo.ComunicacionArduino
{
    public class ArduinoComunication
    {
        SerialPort serialPort;
        public event EventHandler RespuestaRecivida;
        string message;
        bool sensorExisting = false;
        int porNum;
        string[] ports;
        public ArduinoComunication() { }
        public ArduinoComunication(int portNum, string[] ports)
        {
            this.porNum = portNum;
            this.ports = ports;
            initializeSensor();
        }
        public void Reset()
        {
            try
            {
                serialPort.Close();
                serialPort.Open();
            }
            catch(Exception)
            {

            }
        }
        public void AbrirComunicacion()
        {
            try
            {
                serialPort.Open();
            }
            catch (Exception)
            {

            }
        }
        public void CerrarComunicacion()
        {
            try
            {
                serialPort.Close();
            }
            catch (Exception)
            {

            }
        }
        public bool IsSensorConected
        {
            get { return sensorExisting; }
        }
        public void initializeSensor()
        {
            serialPort = new SerialPort();
            if (ports.Length <= 0)
            {
                sensorExisting = false;
                message = "Arduino no detectado";
                SensorNotFoundExceptio ex = new SensorNotFoundExceptio(message);
                throw ex;
            }
            else
            {
                sensorExisting = true;
                serialPort.PortName = ports[porNum];
                serialPort.BaudRate = 9600;
                serialPort.DtrEnable = true;

                try
                {
                    serialPort.Open();
                    serialPort.DataReceived+= new System.IO.Ports.SerialDataReceivedEventHandler(LeerDatos);
                }
                catch (Exception e)
                {
                    message = "No se pudo establecer comunicacion con el arduino." + e.Message;
                    SensorNotFoundExceptio ex = new SensorNotFoundExceptio(message);
                    throw ex;
                }
            }
        }

        private void LeerDatos(object sender, SerialDataReceivedEventArgs e)
        {
            string data;
            data = serialPort.ReadLine();
            RespuestaRecivida(data, null);
        }

        public string[] ExistingPorts()
        {
            //string[] ports = SerialPort.GetPortNames();
            string[] portnames = SerialPort.GetPortNames();
            if (portnames.Length > 0)
            {
                using (var searcher = new ManagementObjectSearcher
                    ("SELECT * FROM WIN32_SerialPort"))
                {

                    var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
                    var tList = (from n in portnames
                                 join p in ports on n equals p["DeviceID"].ToString()
                                 select n + " - " + p["Caption"]).ToList();
                    return tList.ToArray();


                }
            }
            else
            {
                message = "Arduino no detectado";
                SensorNotFoundExceptio ex = new SensorNotFoundExceptio(message);
                throw ex;
            }
        }
        public string[] ExistingPortsOnly()
        {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length <= 0)
            {
                sensorExisting = false;
                message = "Arduino no detectado";
                SensorNotFoundExceptio ex = new SensorNotFoundExceptio(message);
                throw ex;
            }
            else
            {
                return ports;
            }
        }
        public string getErrorMessage()
        {
            return message;
        }
        public void Dispose()
        {
            serialPort.Dispose();
        }
        public void Write(string data)
        {
            serialPort.Write(data);
        }
    }
}
