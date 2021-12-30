using System.Device.Gpio;

namespace main
{
    public class Motor
    {
        private readonly GpioController _GPIO;
        private readonly int _o_direction;
        private readonly int _o_pwm;
        private readonly int _o_enabled;
        private readonly int _i_directionChange;
        private readonly int _i_run;
        private readonly int _stepsPerRound;
        private readonly int _pitch;
        private readonly decimal _startSpeed;
        private readonly decimal _endSpeed;
        private readonly int _rampUpSteps;
        private readonly int _rampDownSteps;
        private readonly decimal _increaseSpeedPerStep;
        private readonly decimal _decreaseSpeedPerStep;
        private bool readLock = false;
        private bool dir = false;

        public Motor(int o_direction, int o_pwm, int o_enabled, int i_directionChange, int i_run, int stepsPerRound, int pitch)
        {
            _GPIO = new GpioController();

            _o_direction = o_direction;
            _o_pwm = o_pwm;
            _o_enabled = o_enabled;
            _i_directionChange = i_directionChange;
            _i_run = i_run;
            _stepsPerRound = stepsPerRound;
            _pitch = pitch;


            //Motor init
            _GPIO.OpenPin(_o_direction, PinMode.Output);
            _GPIO.OpenPin(_o_enabled, PinMode.Output);
            _GPIO.OpenPin(_o_pwm, PinMode.Output);
            _GPIO.OpenPin(_i_directionChange, PinMode.InputPullDown);
            _GPIO.OpenPin(_i_run, PinMode.InputPullDown);
        }

        public void setActive()
        {
            _GPIO.Write(_o_direction, PinValue.Low);
            _GPIO.Write(_o_enabled, PinValue.Low);

            while (true)
            {
                if (_GPIO.Read(_i_directionChange) == PinValue.High && !readLock)
                {
                    readLock = true;
                    if (dir)
                    {
                        _GPIO.Write(_o_direction, PinValue.Low);

                    }
                    else
                    {
                        _GPIO.Write(_o_direction, PinValue.High);

                    }

                    dir = !dir;
                }
                else if (_GPIO.Read(_i_directionChange) == PinValue.Low)
                {
                    readLock = false;
                }


                if (_GPIO.Read(_i_run) == PinValue.High)
                {
                    _GPIO.Write(_o_pwm, PinValue.Low);
                    System.Threading.Thread.Sleep(5);
                    _GPIO.Write(_o_pwm, PinValue.High);
                    System.Threading.Thread.Sleep(5);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }

            _GPIO.Write(_o_enabled, PinValue.High);
            _GPIO.Dispose();
        }


        public void UpKeyEventHandler(object sender, EventArgs e)
        {

        }

        public void DownKeyEventHandler(object sender, EventArgs e)
        {

        }
    }

}