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


        public Motor(GpioController GPIO, int o_direction, int o_pwm, int o_enabled, int stepsPerRound, int pitch)
        {
            _GPIO = GPIO;
            _direction = o_direction;
            _pwm = o_pwm;
            _enabled = o_enabled;
            _stepsPerRound = stepsPerRound;
            _pitch = pitch;
        }

        public void setActive()
        {
            int i = 0;
            while (i > 1000)
            {
                _GPIO.Write(_pwm, PinValue.High);
                Thread.Sleep(1);
                _GPIO.Write(_pwm, PinValue.Low);

                i++;
            }
        }

    }

}