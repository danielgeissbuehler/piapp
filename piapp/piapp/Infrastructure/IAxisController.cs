using piapp.Domain;

namespace piapp.Infrastructure
{
    public interface IAxisController
    {
        public void ConnectControllers();
        public void DisconnectControllers();
        public Task<bool> Init(CancellationToken cancellationToken);
        public void RunProcedure(Procedure procedure, CancellationToken cancellationToken);
        public void MoveYAxisInfinite(string direction, int speed);
        public void MoveYAxisOneStep(AxisStepParams step);
        public void MoveXAxisInfinite(string direction, int speed);
        public void MoveXAxisOneStep(AxisStepParams step);
        public void StopXAxis();
        public void StopYAxis();
        public void StopAll();
    }
}