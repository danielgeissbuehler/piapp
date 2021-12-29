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

while (true)
{
    if (gpio.Read(O_LED) == PinValue.High)
    {
        gpio.Write(O_LED, PinValue.Low);
    }
    else
    {
        gpio.Write(O_LED, PinValue.High);
    }
    Thread.Sleep(2000);

    Console.WriteLine("blink");
}




