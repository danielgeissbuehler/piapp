using System.Device.Gpio;

namespace piapp.Infrastructure
{
    public sealed class GPIO : GpioController
    {
        private static GPIO instance;

        private static readonly object padlock = new object();

        public GPIO()
        {}

        public static GPIO GetGPIO
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GPIO();
                    }
                    return instance;
                }
            }
        }
    }
}
