using System.Device.Gpio;

public class InputEventHandler
{
    private bool[] lockArr = { };
    public void GpioRead()
    {
        while (true)
        {
            Thread.Sleep(1000);
        }
    }
    private bool Read(int input)
    {
        using var _GPIO = new GpioController();

        if (_GPIO.Read(input) == PinValue.High)
        {
            lockArr[input] = true;
            return true;
        }
        else
        {
            lockArr[input] = false;
            return false;
        }
    }

}