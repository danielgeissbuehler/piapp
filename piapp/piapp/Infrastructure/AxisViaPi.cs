using piapp.Infrastructure;
using System.Device.Gpio;

namespace piapp.Domain
{
    public class AxisViaPi
    {
        private GPIO _GPIO;

        private int _o_direction;
        private int _o_pwm;
        private int _o_enabled;

        private MotorParams _motorParams;

        public AxisViaPi(int o_direction, int o_pwm, int o_enabled)
        {
            _GPIO = GPIO.GetGPIO;

            _o_direction = o_direction;
            _o_pwm = o_pwm;
            _o_enabled = o_enabled;
            _GPIO.OpenPin(_o_direction, PinMode.Output);
            _GPIO.OpenPin(_o_enabled, PinMode.Output);
            _GPIO.OpenPin(_o_pwm, PinMode.Output);

            _motorParams = new MotorParams
            {
                Speed = 50,
                Steps = 0,
                StepsToDo = 0,
                Mode = MotorMode.Infinite
            };

            StartLoop();
        }

        private void StartLoop()
        {
            while (true)
            {
                switch (_motorParams.Mode)
                {
                    case MotorMode.Stopped:
                        break;
                    case MotorMode.Infinite:
                        TurnOneStep();
                        break;
                    case MotorMode.NumberOfSteps:
                        TurnNumberOfSteps();
                        break;
                    default:
                        break;
                }
            }
        }

        private void TurnNumberOfSteps()
        {
            if (_motorParams.Steps < _motorParams.StepsToDo)
            {
                TurnOneStep();
                _motorParams.Steps++;
            }
        }

        private void TurnOneStep()
        {
            _GPIO.Write(_o_pwm, PinValue.Low);
            System.Threading.Thread.Sleep(_motorParams.Speed / 2);
            _GPIO.Write(_o_pwm, PinValue.High);
            System.Threading.Thread.Sleep(_motorParams.Speed / 2);
        }

        public void Stop()
        {
            _motorParams.Mode = MotorMode.Stopped;
        }

        public void TurnLeft()
        {
            _GPIO.Write(_o_direction, PinValue.Low);
            _motorParams.Mode = MotorMode.Infinite;
        }

        public void TurnLeft(int mm, int speed)
        {
            _GPIO.Write(_o_direction, PinValue.Low);
            _motorParams.Mode = MotorMode.NumberOfSteps;
            _motorParams.StepsToDo = mm;
            _motorParams.Steps = 0;
        }

        public void MoveYAxisInfinite(bool direction, int speed)
        {
            _motorParams.Mode = MotorMode.Infinite;
            _motorParams.Speed = speed;

            if (direction)
            {
                _GPIO.Write(_o_direction, PinValue.High);
                return;
            }

            _GPIO.Write(_o_direction, PinValue.Low); 
        }

        public void MoveYAxisDinstance(AxisStepParams axisStepParams)
        {
            _motorParams.Mode = MotorMode.NumberOfSteps;
            _motorParams.Speed = axisStepParams.Speed;
            _motorParams.StepsToDo = axisStepParams.Distance;

            if (axisStepParams.Direction  == 1)
            {
                _GPIO.Write(_o_direction, PinValue.High);
                return;
            }

            _GPIO.Write(_o_direction, PinValue.Low);
        }

        public void MoveXAxisInfninite(bool direction, int speed)
        {
            _motorParams.Mode = MotorMode.Infinite;
            _motorParams.Speed = speed;

            if (direction)
            {
                _GPIO.Write(_o_direction, PinValue.High);
                return;
            }

            _GPIO.Write(_o_direction, PinValue.Low);
        }

        public void MoveXAxisOneStep(AxisStepParams axisStepParams)
        {
            _motorParams.Mode = MotorMode.NumberOfSteps;
            _motorParams.Speed = axisStepParams.Speed;
            _motorParams.StepsToDo = axisStepParams.Distance;

            if (axisStepParams.Direction == 1)
            {
                _GPIO.Write(_o_direction, PinValue.High);
                return;
            }

            _GPIO.Write(_o_direction, PinValue.Low);
        }
    }
}
