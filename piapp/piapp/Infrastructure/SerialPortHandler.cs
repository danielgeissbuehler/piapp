using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

public class SerialPortHandler : IDisposable
{
    private SerialPort _serialPort;
    string _data = string.Empty;

    public event EventHandler<string> DataReceived;

    public SerialPortHandler(string portName, int baudRate)
    {
        if(_serialPort == null)
        {
            _serialPort = new SerialPort(portName, baudRate);
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(OnSerialDataReceived);
            Connect();


            _serialPort.DiscardInBuffer();
        }
    }

    public void Connect()
    {
        if (!_serialPort.IsOpen)
            _serialPort.Open();
    }

    public void Disconnect()
    {
        _serialPort.DiscardInBuffer();
        _serialPort.Close();
    }

    private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            _data = _serialPort.ReadTo("#");
            DataReceived?.Invoke(this, _data);
        }
        catch (Exception ex)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
                
            }
                
            Console.WriteLine($"Fehler beim Lesen: {ex.Message}, Data: {_data}");
            _data = string.Empty;
        }
    }

    public void Write(string text)
    {
        try
        {
            text += "#"; //Stopbit (Arduino liest bist ein /n kommt)
            _serialPort.Write(text);
        }
        catch (IOException ex)
        {

            Console.WriteLine("Schreiben fehlgeschlagen: " + ex.Message);
        }
    }

    public void Dispose()
    {
        _serialPort.Close();
    }
}