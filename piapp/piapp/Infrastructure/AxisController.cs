using piapp.Domain;
using System.Diagnostics;
using System.Text.Json;

namespace piapp.Infrastructure
{
    public class AxisController : IAxisController
    {
        private readonly double _motorStepsPerRound = 0;
        private readonly double _axisPitchPerRound = 0;

        private readonly SerialPortHandler _SerialPortToArduino;

        private bool _limitSwich_1 = false;
        private bool _limitSwich_2 = false;

        private int _absolutePositionInSteps = 0;
        private int _relativePositionInSteps = 0;

        public event EventHandler PositionChanged;
        public AxisState State { get; private set; }
        private double _position = 0;
        public double Position
        {
            get
            {
                return _position;
            }
            private set
            {
                _position = Math.Round(value, 2);
            }
        }

        
        public AxisController(string serialPort, int baudRate, int stepsPerRound, int axisPitchPerRound)
        {
            _motorStepsPerRound = stepsPerRound;
            _axisPitchPerRound = axisPitchPerRound;

            _SerialPortToArduino = new SerialPortHandler(serialPort, baudRate);
            _SerialPortToArduino.DataReceived += OnXAxisDataReceived;

            ConnectController();
            _SerialPortToArduino.Write("init#");
        }

        public void ConnectController()
        {
            _SerialPortToArduino.Connect();
        }

        public void DisconnectController()
        {
            _SerialPortToArduino.Disconnect();
        }

        public bool Init(CancellationToken cancellationToken)
        {
            if (!InitAxis(cancellationToken))
                return false;

            return true;
        }

        private bool InitAxis(CancellationToken cancellationToken)
        {
            MoveAxisInfinite("up", 300);
            while (!_limitSwich_1)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StopAxis();
                    return false;
                }
            }

            StopAxis();
            Thread.Sleep(100);
            //await Task.Delay(100); // Achse stoppen lassen bevor weiter

            MoveAxisInfinite("down", 300);
            while (!_limitSwich_2)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StopAxis();
                    return false;
                }
            }

            StopAxis();
            Thread.Sleep(100);
            //await Task.Delay(100); // Achse stoppen lassen bevor weiter

            var moveToPosition = new AxisCommand { Direction = 1, Distance = Math.Abs(_absolutePositionInSteps / 2), Mode = "step", Speed = 300 };

            MoveAxisToPosition(moveToPosition);
            while (State != AxisState.Idle)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StopAxis();
                    return false;
                }
            }

            StopAxis();
            Thread.Sleep(100);
            //await Task.Delay(100); // Achse stoppen lassen bevor weiter
            Debug.WriteLine("Position X-Achse: " + Position);
            return true;
        }

        public void MoveAxisInfinite(string direction, int speed)
        {
            AxisCommand command;
            State = AxisState.RunningInfinite;

            if (direction == "up")
                command = new AxisCommand { Direction = 1, Distance = 0, Mode = "direct", Speed = speed };
            else
                command = new AxisCommand { Direction = 0, Distance = 0, Mode = "direct", Speed = speed };

            WriteCommandToController(command);
        }

        public void MoveAxisToPosition(AxisCommand command)
        {
            State = AxisState.RunningToPosition;
            WriteCommandToController(command);
        }

        public void StopAxis()
        {
            _SerialPortToArduino.Write("stop");
        }


        //Private Functions
        private void WriteCommandToController(AxisCommand command)
        {
            string jsonString = JsonSerializer.Serialize(command);

            try
            {
                _SerialPortToArduino.Write(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Schreiben fehlgeschlagen: {ex.ToString()}");
            }
        }

        private void OnXAxisDataReceived(object? sender, string e)
        {
            string output = new string(e.Where(c => !char.IsControl(c)).ToArray());

            switch (output)
            {
                case "setup_fertig":
                    break;

                case "limitswitch_1_true":
                    _limitSwich_1 = true;
                    break;

                case "limitswitch_1_false":
                    _limitSwich_1 = false;
                    break;

                case "limitswitch_2_true":
                    _limitSwich_2 = true;
                    break;

                case "limitswitch_2_false":
                    _limitSwich_2 = false;
                    break;

                case "fertig":
                    State = AxisState.Idle;
                    UpdatePosition(_relativePositionInSteps);
                    Debug.WriteLine($"X-Achse fertig, aktuelle Position: { _absolutePositionInSteps } ");
                    break;

                default:
                    break;
            }

            if (int.TryParse(output, out int relPosInSteps))
            {
                UpdatePosition(relPosInSteps);
            }

            return;
        }

        private void UpdatePosition(int relativePositionInSteps)
        {
            if (relativePositionInSteps != 0)
            {
                _relativePositionInSteps = relativePositionInSteps;

                if (State == AxisState.Idle)
                {
                    _absolutePositionInSteps += _relativePositionInSteps;
                    _relativePositionInSteps = 0;
                }

                Position = ((_absolutePositionInSteps + _relativePositionInSteps) / _motorStepsPerRound) / _axisPitchPerRound;
                PositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}