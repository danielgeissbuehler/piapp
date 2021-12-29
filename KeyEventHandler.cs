using System.Device.Gpio;
using static System.Net.Mime.MediaTypeNames;

public static class KeyEventHandler
{
    public static event EventHandler LeftArrowKeyPressed;
    public static event EventHandler RightArrowKeyPressed;
    public static event EventHandler UpArrowKeyPressed;
    public static event EventHandler DownArrowKeyPressed;
    public static event EventHandler EscapeKeyPressed;

    public static void ReadKeys()
    {
        ConsoleKeyInfo key = new ConsoleKeyInfo();
        using GpioController gpio = new GpioController();

        while (!Console.KeyAvailable && key.Key != ConsoleKey.Escape)
        {
            key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    Console.WriteLine("UpArrow was pressed");
                    UpArrowKeyPressed?.Invoke(null, EventArgs.Empty);
                    break;
                case ConsoleKey.DownArrow:
                    Console.WriteLine("DownArrow was pressed");
                    DownArrowKeyPressed?.Invoke(null, EventArgs.Empty);
                    break;

                case ConsoleKey.RightArrow:
                    RightArrowKeyPressed?.Invoke(null, EventArgs.Empty);
                    break;

                case ConsoleKey.LeftArrow:
                    LeftArrowKeyPressed?.Invoke(null, EventArgs.Empty);
                    break;

                case ConsoleKey.Escape:
                    EscapeKeyPressed?.Invoke(null, EventArgs.Empty);
                    break;

                default:
                    if (Console.CapsLock && Console.NumberLock)
                    {
                        Console.WriteLine(key.KeyChar);
                    }
                    break;
            }
        }
    }

}