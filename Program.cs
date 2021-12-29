using System.Device.Gpio;


using GpioController gpio = new GpioController();

const int O_LED = 2;
const int O_M1_DIRECTION = 19;
const int O_M1_PWM = 13;
const int O_M1_ENABLED = 26;
const int M1_STEPS_PER_ROUND = 400;
const int M1_PITCH = 10;


gpio.OpenPin(O_LED, PinMode.Output);
gpio.Write(O_LED, PinValue.Low);

//Motor init
gpio.OpenPin(O_M1_DIRECTION, PinMode.Output);
gpio.OpenPin(O_M1_ENABLED, PinMode.Output);
gpio.OpenPin(O_M1_PWM, PinMode.Output);

gpio.Write(O_M1_DIRECTION, PinValue.Low);
gpio.Write(O_M1_ENABLED, PinValue.Low);

for (int i = 0; i < 1000; i++)
{
    i++;
    gpio.Write(O_M1_PWM, PinValue.High);
    System.Threading.Thread.Sleep(10);
    gpio.Write(O_M1_PWM, PinValue.Low);
}

gpio.Write(O_M1_ENABLED, PinValue.High);
gpio.Write(O_LED, PinValue.High);