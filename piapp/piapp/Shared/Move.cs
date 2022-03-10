using piapp.Data;
using piapp.Domain;
using piapp.Infrastructure;

namespace piapp.Shared
{
    public partial class Move
    {
        private const int MAX_SPEED = 500;
        private const int MIN_SPEED = 1000;
        private int Speed = 500;
        private string Error = string.Empty;

        private bool _upButtonPressed = false;
        private bool _downButtonPressed = false;
        private bool _leftButtonPressed = false;
        private bool _rightButtonPressed = false;
        private bool _processing = false;
        private bool _connected = true;

        private double XPositon = 0;

        private CancellationTokenSource _cancellationTokenSource;


        public Move()
        {
        }

        private void _axisController_XAxisPositionChanged(object? sender, PositionChangedEventArgs e)
        {
            XPositon = e.Position;
        }

        public void ArrowUpOnClick()
        {
            _axisController.MoveXAxisInfinite("up", Speed);
            _upButtonPressed = true;        
        }

        public void ArrowUpOnRelease()
        {
            _axisController.StopXAxis();
            _upButtonPressed=false;
        }

        public void ArrowDownOnClick()
        {
            _axisController.MoveXAxisInfinite("down", Speed);
            _downButtonPressed = true;
        }

        public void ArrowDownOnRelease()
        {
            _axisController.StopXAxis();
            _downButtonPressed = false;
        }

        public void ArrowRightOnClick()
        {
            _axisController.MoveYAxisInfinite("up", Speed);
            _rightButtonPressed = true;
        }

        public void ArrowRightOnRelease()
        {
            _axisController.StopYAxis();
            _rightButtonPressed = false;
        }

        public void ArrowLeftOnClick()
        {
            _axisController.MoveYAxisInfinite("down", Speed);
            _leftButtonPressed = true;
        }

        public void ArrowLeftOnRelease()
        {
            _axisController.StopYAxis();
            _leftButtonPressed = false;
        }

        public async Task InitPlatform()
        {
            _processing = true;

            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                await Task.Run(() => _axisController.Init(_cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {

                Error = ex.Message;
            }

            _processing = false;
        }

        public async Task SartProcessClick()
        {
            _processing = true;

            Procedure procedure = RandomProcedureFactory.GetRandomProcedure(10, 10, 200, 20, 2000, 2000);

            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                await Task.Run(() => _axisController.RunProcedure(procedure, _cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {

                Error = ex.Message;
            }
            
            _processing = false;
        }

        public void Connect()
        {
            _axisController.ConnectControllers();
            _connected = true;
        }

        public void Disconnect()
        {
            _axisController.DisconnectControllers();
            _connected = false;
        }

        public void StopProcess()
        {
            _cancellationTokenSource.Cancel();
            _axisController.StopAll();
            _processing = false;
        }

        public void CloseError()
        {
            Error = string.Empty;
        }
    }
}
