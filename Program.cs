using System.Device.Gpio;
using main;

//Led PIN
const int O_LED = 2;

//Motor1 PIN
const int O_M1_ENABLED = 26;
const int O_M1_DIRECTION = 19;
const int O_M1_PWM = 13;

const int I_M1_DIRECTION_CHANGE = 4;
const int I_M1_RUN = 22;


//Motor2 PIN
const int O_M2_ENABLED = 21;
const int O_M2_DIRECTION = 20;
const int O_M2_PWM = 12;

const int I_M2_DIRECTION_CHANGE = 27;
const int I_M2_RUN = 17;

//Motordaten
const int M_STEPS_PER_ROUND = 400;
const int M_PITCH = 10;

using GpioController gpio = new GpioController();

gpio.OpenPin(O_LED, PinMode.Output);

/*
gpio.OpenPin(22, PinMode.InputPullDown);
while (true)
{
    if (gpio.Read(22) == PinValue.Low)
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

var motor1 = new Motor(O_M1_DIRECTION, O_M1_PWM, O_M1_ENABLED, I_M1_DIRECTION_CHANGE, I_M1_RUN, M_STEPS_PER_ROUND, M_PITCH);
var motor2 = new Motor(O_M2_DIRECTION, O_M2_PWM, O_M2_ENABLED, I_M2_DIRECTION_CHANGE, I_M2_RUN, M_STEPS_PER_ROUND, M_PITCH);

gpio.Write(O_LED, PinValue.Low);

Thread tmotor1 = new Thread(motor1.setActive);
tmotor1.Start();

Thread tmotor2 = new Thread(motor2.setActive);
tmotor2.Start();


tmotor1.Join();
tmotor2.Join(); //Warten bis thread 2 abgeschlossen ist


gpio.Write(O_LED, PinValue.High);


//var tasks = new[] { taskKeys };
//Task.WaitAll(tasks);