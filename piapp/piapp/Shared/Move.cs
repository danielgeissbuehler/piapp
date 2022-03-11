using piapp.Data;
using piapp.Domain;
using System.Diagnostics;

namespace piapp.Shared
{
    public partial class Move : IDisposable
    {
        private const int MAX_SPEED = 20;
        private const int MIN_SPEED = 500;
        private int Speed = 250;
        private string Error = string.Empty;

        private bool _upButtonPressed = false;
        private bool _downButtonPressed = false;
        private bool _leftButtonPressed = false;
        private bool _rightButtonPressed = false;
        private bool _processing = false;
        private bool _connected = true;
        private Stopwatch _timer = Stopwatch.StartNew();

        private CancellationTokenSource _cancellationTokenSource;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _plattformController.XAxis.PositionChanged += XAxis_PositionChanged;
            _plattformController.YAxis.PositionChanged += YAxis_PositionChanged;
        }

        void IDisposable.Dispose()
        {
            _plattformController.XAxis.PositionChanged -= XAxis_PositionChanged;
            _plattformController.YAxis.PositionChanged -= YAxis_PositionChanged;
        }

        //Update View wenn Wert geändert hat
        private void XAxis_PositionChanged(object? sender, EventArgs e)
        {
            if(_timer.ElapsedMilliseconds > 500)
            {
                InvokeAsync(StateHasChanged);
                _timer.Restart();
            }  
        }

        //Update View wenn Wert geändert hat
        private void YAxis_PositionChanged(object? sender, EventArgs e)
        {
            if (_timer.ElapsedMilliseconds > 100)
            {
                InvokeAsync(StateHasChanged);
                _timer.Restart();
            }
        }

        public void ArrowUpOnClick()
        { 
            _plattformController.MoveXAxisInfinite("up", Speed);
            _upButtonPressed = true;
        }

        public void ArrowUpOnRelease()
        {
            _plattformController.XAxis.StopAxis();
            _upButtonPressed=false;
        }

        public void ArrowDownOnClick()
        {
            _plattformController.MoveXAxisInfinite("down", Speed);
            _downButtonPressed = true;
        }

        public void ArrowDownOnRelease()
        {
            _plattformController.XAxis.StopAxis();
            _downButtonPressed = false;
        }

        public void ArrowRightOnClick()
        {
            _plattformController.MoveYAxisInfinite("up", Speed);
            _rightButtonPressed = true;
        }

        public void ArrowRightOnRelease()
        {
            _plattformController.YAxis.StopAxis();
            _rightButtonPressed = false;
        }

        public void ArrowLeftOnClick()
        {
            _plattformController.MoveYAxisInfinite("down", Speed);
            _leftButtonPressed = true;
        }

        public void ArrowLeftOnRelease()
        {
            _plattformController.YAxis.StopAxis();
            _leftButtonPressed = false;
        }

        public async Task InitPlatform()
        {
            _processing = true;

            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                await Task.Run(() => _plattformController.Init(_cancellationTokenSource.Token)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                Error = ex.Message;
            }

            _processing = false;
        }

        public async Task StartProcess()
        {
            _processing = true;

            Procedure procedure = RandomProcedureFactory.GetRandomProcedure(10, 10, 200, 20, 2000, 2000);

            try
            {
                _cancellationTokenSource = new CancellationTokenSource();

                await Task.Run(() => _plattformController.RunProcedure(procedure, _cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            _processing = false;
        }

        public void Connect()
        {
            _plattformController.ConnectController();
            _connected = true;
        }

        public void Disconnect()
        {
            _plattformController.DisconnectController();
            _connected = false;
        }

        public void StopProcess()
        {
            _cancellationTokenSource.Cancel();
            _plattformController.StopAll();
            _processing = false;
        }

        public void CloseError()
        {
            Error = string.Empty;
        }
    }
}
