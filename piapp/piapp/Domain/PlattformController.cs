using piapp.Domain;
using System.Diagnostics;
using System.Text.Json;
using System.Timers;

namespace piapp.Infrastructure
{
    public class PlattformController
    {
        public IAxisController XAxis { get; private set; }
        public IAxisController YAxis { get; private set; }

        public PlattformController()
        {
            XAxis = new AxisController("COM7", 460800, 60, 100);
            YAxis = new AxisController("COM8", 460800, 60, 100);
        }

        public void ConnectController()
        {
            XAxis.ConnectController();
            YAxis.ConnectController();
        }

        public void DisconnectController()
        {
            XAxis.DisconnectController();
            YAxis.DisconnectController();
        }

        public bool Init(CancellationToken cancellationToken)
        {
            var ok = XAxis.Init(cancellationToken);
            //ok = _yAxis.Init(cancellationToken);

            return ok;
        }

        public void RunProcedure(Procedure procedure, CancellationToken cancellationToken)
        {
            StopAll();

            int numberOfSteps = 0;
            Stopwatch doneWaitStopwatch = Stopwatch.StartNew();

            while (numberOfSteps < procedure.Steps.Count())
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    StopAll();
                    break;
                }

                if (doneWaitStopwatch.ElapsedMilliseconds > 2000)
                {
                    StopAll();
                    throw new Exception($"Achse reagiert nicht: /nStatus X-Achse: {XAxis.State} /nStatus Y-Achse: {YAxis.State}");
                }

                if (XAxis.State == AxisState.Idle && YAxis.State == AxisState.Idle)
                {
                    doneWaitStopwatch.Restart();

                    MoveToPositon(procedure.Steps.ElementAt(numberOfSteps));

                    numberOfSteps++;
                }
            }
        }

        public void MoveXAxisInfinite(string direction, int speed)
        {
            XAxis.MoveAxisInfinite(direction, speed);
        }

        public void MoveYAxisInfinite(string direction, int speed)
        {
            YAxis.MoveAxisInfinite(direction, speed);
        }

        public void MoveToPositon(PositionCommand step)
        {
            XAxis.MoveAxisToPosition(step.XAxis);
            YAxis.MoveAxisToPosition(step.YAxis);
        }

        public void StopAll()
        {
            XAxis.StopAxis();
            YAxis.StopAxis();
        }
    }
}