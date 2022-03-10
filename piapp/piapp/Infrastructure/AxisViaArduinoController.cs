using piapp.Domain;
using System.Diagnostics;
using System.Text.Json;
using System.Timers;

namespace piapp.Infrastructure
{
    public class AxisViaArduinoController : IAxisController
    {
        private readonly int _motorStepsPerRound = 0;
        private readonly int _axisPitchPerRound = 0;

        private readonly SerialPortHandler _arduinoXAxis;
        private readonly SerialPortHandler _arduinoYAxis;

        private string _xAxisState = "done";
        private string _yAxisState = "done";

        private bool _limitSwich1XAxis = false;
        private bool _limitSwich2XAxis = false;
        private bool _limitSwich1YAxis = false;
        private bool _limitSwich2YAxis = false;

        private int _xAxisActualRelPositionInSteps = 0;
        private int _yAxisActualRelPositionInSteps = 0;


        private int _xAxisActualAbsPositionInSteps = 0;
        
        private int XAxisActualAbsPositionInSteps 
        {
            get
            { 
                return _xAxisActualAbsPositionInSteps; 
            }
            set
            {
                _xAxisActualAbsPositionInSteps = value;
                XAxisActualAbsPositionInMM = _xAxisActualAbsPositionInSteps / _motorStepsPerRound / _axisPitchPerRound;
            }
        }

        
        private int _yAxisActualAbsPositionInSteps = 0;


        private double _xAxisActualAbsPositionInMM;
        public double XAxisActualAbsPositionInMM
        {
            get
            {
                return _xAxisActualAbsPositionInMM;
            }
            set
            {
                _xAxisActualAbsPositionInMM = value;
                Debug.WriteLine("Position in mm: " + _xAxisActualAbsPositionInMM);
            }
        }

        public AxisViaArduinoController()
        {
            _arduinoXAxis = new SerialPortHandler("COM7", 460800);
            _arduinoYAxis = new SerialPortHandler("COM8", 460800);

            _arduinoXAxis.DataReceived += OnXAxisDataReceived;
            _arduinoYAxis.DataReceived += OnYAxisDataReceived;

            _motorStepsPerRound = 60;
            _axisPitchPerRound = 100;

            _arduinoXAxis.Write("init");
            _arduinoYAxis.Write("init");
        }

        public void ConnectControllers()
        {
            _arduinoXAxis.Connect();
            _arduinoYAxis.Connect();
        }

        public void DisconnectControllers()
        {
            _arduinoXAxis.Disconnect();
            _arduinoYAxis.Disconnect();
        }

        public bool Init(CancellationToken cancellationToken)
        {
            if (!InitXAxis(cancellationToken))
                return false;

            //if (! await InitYAxis(cancellationToken))
            //    return false;

            return true;
        }

        private bool InitXAxis(CancellationToken cancellationToken)
        {
            MoveXAxisInfinite("up", 300);
            while (!_limitSwich1XAxis)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StopAll();
                    return false;
                }
            }

            StopXAxis();
            Thread.Sleep(100);
            //await Task.Delay(100); // Achse stoppen lassen bevor weiter

            XAxisActualAbsPositionInSteps = 0;

            MoveXAxisInfinite("down", 300);
            while (!_limitSwich2XAxis)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StopAll();
                    return false;
                }
            }

            StopXAxis();
            Thread.Sleep(100);
            //await Task.Delay(100); // Achse stoppen lassen bevor weiter

            XAxisActualAbsPositionInSteps = (-_xAxisActualRelPositionInSteps / 2);

            var moveToPosition = new AxisStepParams { Direction = 1, Distance = Math.Abs(XAxisActualAbsPositionInSteps), Mode = "step", Speed = 300 };


            MoveXAxisOneStep(moveToPosition);

            while (_xAxisState != "done")
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StopAll();
                    return false;
                }
            }

            StopXAxis();
            Thread.Sleep(100);
            //await Task.Delay(100); // Achse stoppen lassen bevor weiter
            Debug.WriteLine("Position X-Achse: " + _xAxisActualAbsPositionInMM);
            return true;
        }

        
        private async Task<bool> InitYAxis(CancellationToken cancellationToken)
        {
            MoveYAxisInfinite("up", 300);
            while (!_limitSwich1YAxis)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StopAll();
                    return false;
                }
            }

            StopYAxis();
            await Task.Delay(100); // Achse stoppen lassen bevor weiter

            _yAxisActualAbsPositionInSteps = 0;

            MoveYAxisInfinite("down", 300);
            while (!_limitSwich2YAxis)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StopAll();
                    return false;
                }
            }

            StopYAxis();
            await Task.Delay(100); // Achse stoppen lassen bevor weiter

            _yAxisActualAbsPositionInSteps = (-_yAxisActualAbsPositionInSteps / 2);

            var moveToPosition = new AxisStepParams { Direction = 1, Distance = Math.Abs(_yAxisActualAbsPositionInSteps), Mode = "step", Speed = 300 };

            MoveYAxisOneStep(moveToPosition);

            while (_yAxisState != "done")
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StopAll();
                    return false;
                }
            }

            StopYAxis();
            await Task.Delay(100); // Achse stoppen lassen bevor weiter

            return true;
        }


        public void RunProcedure(Procedure procedure, CancellationToken cancellationToken)
        {
            StopAll();

            int numberOfSteps = 0;
            Stopwatch doneWaitStopwatch = Stopwatch.StartNew();

            while (numberOfSteps < procedure.Steps.Count())
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    StopAll();
                    break;
                }

                if (doneWaitStopwatch.ElapsedMilliseconds > 2000)
                {
                    StopAll();
                    throw new Exception($"Fertig nicht erhalten Status Y-Achse: {_yAxisState}, Status X-Achse: {_xAxisState}");
                }

                if (_xAxisState == "done" && _yAxisState == "done")
                {
                    doneWaitStopwatch.Restart();
                    _xAxisState = "";
                    _yAxisState = "";

                    MoveXAxisOneStep(procedure.Steps.ElementAt(numberOfSteps).XAxis);
                    MoveYAxisOneStep(procedure.Steps.ElementAt(numberOfSteps).YAxis);

                    numberOfSteps++;
                }
            }
        }

        public void MoveXAxisInfinite(string direction, int speed)
        {
            AxisStepParams step;

            if (direction == "up")
                step = new AxisStepParams { Direction = 1, Distance = 0, Mode = "direct", Speed = speed };
            else
                step = new AxisStepParams { Direction = 0, Distance = 0, Mode = "direct", Speed = speed };

            WriteXAxis(step);
        }

        public void MoveXAxisOneStep(AxisStepParams step)
        {
            WriteXAxis(step);
        }

        public void MoveYAxisInfinite(string direction, int speed)
        {
            AxisStepParams step;

            if (direction == "up")
                step = new AxisStepParams { Direction = 1, Distance = 0, Mode = "direct", Speed = speed };
            else
                step = new AxisStepParams { Direction = 0, Distance = 0, Mode = "direct", Speed = speed };

            WriteYAxis(step);
        }

        public void MoveYAxisOneStep(AxisStepParams step)
        {
            WriteYAxis(step);
        }

        public void StopXAxis()
        {
            _arduinoXAxis.Write("stop");
        }

        public void StopYAxis()
        {
            _arduinoYAxis.Write("stop");
        }

        public void StopAll()
        {
            _arduinoXAxis.Write("stop");
            _arduinoYAxis.Write("stop");
        }


        //Private Functions
        private void WriteXAxis(AxisStepParams step)
        {
            string jsonString = JsonSerializer.Serialize(step);

            try
            {
                _arduinoXAxis.Write(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Schreiben fehlgeschlagen: {ex.ToString()}");
            }
        }

        private void WriteYAxis(AxisStepParams step)
        {
            string jsonString = JsonSerializer.Serialize(step);

            try
            {
                _arduinoYAxis.Write(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Schreiben fehlgeschlagen: {ex.ToString()}");
            }
        }

        private void OnYAxisDataReceived(object? sender, string e)
        {
            //Sonderzeichen entfernen    
            string output = new string(e.Where(c => !char.IsControl(c)).ToArray());

            if (output == "fertig")
            {
                _yAxisState = "done";
                Console.WriteLine("Y-Achse fertig");
            }
        }

        private void OnXAxisDataReceived(object? sender, string e)
        {
            //Sonderzeichen entfernen    
            string output = new string(e.Where(c => !char.IsControl(c)).ToArray());

            if (output == "setup_fertig")
            {
                //handshakeDone = true;
                Debug.WriteLine("Handshake ok, Setup Arduino abgeschlossen");
                return;
            }

            if (output == "limitswitch_1_true")
            {
                _limitSwich1XAxis = true;
                Debug.WriteLine("Limitswitch 1 = true");

                return;
            }

            if (output == "limitswitch_1_false")
            {
                _limitSwich1XAxis = false;
                Debug.WriteLine("Limitswitch 1 = false");

                return;
            }

            if (output == "limitswitch_2_true")
            {
                _limitSwich2XAxis = true;
                Debug.WriteLine("Limitswitch 2 = true");

                return;
            }

            if (output == "limitswitch_2_false")
            {
                _limitSwich2XAxis = false;
                Debug.WriteLine("Limitswitch 2 = false");

                return;
            }

            if (output == "fertig")
            {
                _xAxisState = "done";
                XAxisActualAbsPositionInSteps += _xAxisActualRelPositionInSteps;
                _xAxisActualRelPositionInSteps = 0;
                Debug.WriteLine($"X-Achse fertig, aktuelle Position: {XAxisActualAbsPositionInSteps}");
                return;
            }

            if (int.TryParse(output, out _xAxisActualRelPositionInSteps))
                return;

            Debug.WriteLine($"Message konnte nicht erkannt werden: {output}");
        }
    }
}