using piapp.Infrastructure;

namespace piapp.Domain
{
    public class PlatformController
    {
        private IAxisController _axisController;

        public PlatformController(IAxisController axisController)
        {
            _axisController = axisController;
        }

        public void StartProcedure(Procedure procedure)
        {
            foreach(Step step in procedure.Steps)
            {
                var xAxis = step.XAxis;
                var yAxis = step.YAxis;

                MoveDistance(false, xAxis);
                MoveDistance(true, yAxis);
            }
        }

        public void Move(bool axis, string direction, int speed)
        {
            if (axis)
            {
               _axisController.MoveXAxisInfinite(direction, speed);
                return;
            }

            _axisController.MoveYAxisInfinite(direction, speed);
        }

        public void MoveDistance(bool axis, AxisStepParams axisStepParams)
        {
            if (axis)
            {
                _axisController.MoveYAxisOneStep(axisStepParams);
                return;
            }

            _axisController.MoveYAxisOneStep(axisStepParams);
        }

        public void Stop()
        {
            _axisController.StopAll();
        }
    }
}