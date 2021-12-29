using System.Device.Gpio;

namespace main
{
    public class Motor
    {
        private readonly GpioController _GPIO;
        private readonly int _direction;
        private readonly int _pwm;
        private readonly int _enabled;
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

        public Motor(int o_direction, int o_pwm, int o_enabled, int stepsPerRound, int pitch)
        {
            _GPIO = new GpioController();

            _direction = o_direction;
            _pwm = o_pwm;
            _enabled = o_enabled;
            _stepsPerRound = stepsPerRound;
            _pitch = pitch;


            //Motor init
            _GPIO.OpenPin(o_direction, PinMode.Output);
            _GPIO.OpenPin(o_enabled, PinMode.Output);
            _GPIO.OpenPin(o_pwm, PinMode.Output);
            _GPIO.OpenPin(3, PinMode.InputPullDown);
            _GPIO.OpenPin(4, PinMode.InputPullDown);

            KeyEventHandler.DownArrowKeyPressed += DownKeyEventHandler;
            KeyEventHandler.UpArrowKeyPressed += UpKeyEventHandler;


        }

        public void setActive()
        {
            _GPIO.Write(_direction, PinValue.Low);
            _GPIO.Write(_enabled, PinValue.Low);


            while (true)
            {

                if (_GPIO.Read(4) == PinValue.High && !readLock)
                {
                    readLock = true;
                    if (dir)
                    {
                        _GPIO.Write(_direction, PinValue.Low);

                    }
                    else
                    {
                        _GPIO.Write(_direction, PinValue.High);

                    }

                    dir = !dir;
                }
                else if (_GPIO.Read(4) == PinValue.Low)
                {
                    readLock = false;
                }



                if (_GPIO.Read(3) == PinValue.Low)
                {
                    //Console.WriteLine("drinn");
                    //for (int i = 0; i < 10; i++)
                    //{
                    _GPIO.Write(_pwm, PinValue.Low);
                    System.Threading.Thread.Sleep(1);
                    _GPIO.Write(_pwm, PinValue.High);
                    System.Threading.Thread.Sleep(1);
                    //}
                }
                else
                {
                    Thread.Sleep(10);
                }
            }

            /*
            for (int i = 0; 0 < 1000; i++)
            {
                _GPIO.Write(_pwm, PinValue.Low);
                System.Threading.Thread.Sleep(5);
                _GPIO.Write(_pwm, PinValue.High);
                System.Threading.Thread.Sleep(5);
            }

            */


            _GPIO.Write(_enabled, PinValue.High);
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