using System.Device.Gpio;
using main;

const int O_LED = 2;
const int O_M1_DIRECTION = 19;
const int O_M1_PWM = 13;
const int O_M1_ENABLED = 26;
const int M1_STEPS_PER_ROUND = 400;
const int M1_PITCH = 10;
const int I_M1_LEFT = 4;

using GpioController gpio = new GpioController();

gpio.OpenPin(O_LED, PinMode.Output);

/*
gpio.OpenPin(27, PinMode.InputPullDown);
while (true)
{
    if (gpio.Read(27) == PinValue.Low)
    {
        gpio.Write(O_LED, PinValue.Low);
    }
    else
    {
        gpio.Write(O_LED, PinValue.High);
    }
}
*/



//var taskKeys = new Task(KeyEventHandler.ReadKeys);
//taskKeys.Start();

var motor1 = new Motor(O_M1_DIRECTION, O_M1_PWM, O_M1_ENABLED, M1_STEPS_PER_ROUND, M1_PITCH);
var motor2 = new Motor(20, 14, 25, M1_STEPS_PER_ROUND, M1_PITCH);

gpio.Write(O_LED, PinValue.Low);

Thread tmotor1 = new Thread(motor1.setActive);
tmotor1.Start();

Thread tmotor2 = new Thread(motor1.setActive);
tmotor2.Start();


tmotor2.Join(); //Warten bis thread 2 abgeschlossen ist

gpio.Write(O_LED, PinValue.High);


//var tasks = new[] { taskKeys };
//Task.WaitAll(tasks);
