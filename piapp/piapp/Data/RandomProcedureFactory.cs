using piapp.Domain;

namespace piapp.Data
{
    public static class RandomProcedureFactory
    {
        private static int _actualXPosition;
        private static int _actualYPosition;
        private static int _maxDistanceFromCenter;
        private static Random _random = new Random();

        public static Procedure GetRandomProcedure(int numberOfSteps, int maxSpeed, int minSpeed, int minDistancePerStep, int maxDistancePerStep, int maxDistanceFromCenter)
        {
            _actualXPosition = 0;
            _actualYPosition = 0;
            _maxDistanceFromCenter = maxDistanceFromCenter;

            var procedure = new Procedure();
            procedure.Id = 1;
            procedure.Name = "random";
            procedure.UserId = 0;
            procedure.UserName = "admin";
            procedure.Steps = CreateRandomSteps(numberOfSteps, maxSpeed, minSpeed, maxDistancePerStep, minDistancePerStep);

            return procedure;
        }

        private static List<PositionCommand> CreateRandomSteps(int numberOfSteps, int maxSpeed, int minSpeed, int maxDistancePerStep, int minDistancePerStep)
        {
            var steps = new List<PositionCommand>();

            for (int i = 0; i < numberOfSteps; i++)
            {
                var xAxis = GetRandomAxisParams(maxSpeed, minSpeed, maxDistancePerStep, minDistancePerStep);
                var yAxis = GetRandomAxisParams(maxSpeed, minSpeed, maxDistancePerStep, minDistancePerStep);

                var step = new PositionCommand { Id = i, XAxis = xAxis, YAxis = yAxis };
                
                //step.XAxis.Distance = xAxis.Distance > _maxDistanceFromCenter? xAxis.Distance : _actualXPosition + Math.Abs(_actualXPosition - _maxDistanceFromCenter);
                //step.YAxis.Distance = yAxis.Distance > _maxDistanceFromCenter ? yAxis.Distance : _maxDistanceFromCenter - _actualXPosition;

                _actualXPosition += xAxis.Distance;
                _actualYPosition += yAxis.Distance;

                steps.Add(step);
            }

            return steps;
        }

        private static AxisCommand GetRandomAxisParams(int maxSpeed, int minSpeed, int maxDistancePerStep, int minDistancePerStep)
        {
            return new AxisCommand
            {
                Mode = "step",
                Direction = GetRandomDirection(),
                Distance = GetRandomDistance(maxDistancePerStep, minDistancePerStep),
                Speed = GetRandomSpeed(minSpeed, maxSpeed)
            };
        }

        private static int GetRandomDirection()
        {
            return _random.Next(0, 2);
        }

        private static int GetRandomDistance(int maxDistancePerStep, int minDistancePerStep)
        {
            return _random.Next(minDistancePerStep, maxDistancePerStep);
        }

        private static int GetRandomSpeed(int maxSpeed, int minSpeed)
        {
            return _random.Next(minSpeed, maxSpeed);
        }
    }
}
