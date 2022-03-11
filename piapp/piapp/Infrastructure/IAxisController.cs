using piapp.Domain;

namespace piapp.Infrastructure
{
    public interface IAxisController
    {
        public AxisState State { get; }
        public double Position { get; }
        public event EventHandler PositionChanged;
        public void ConnectController();
        public void DisconnectController();
        public bool Init(CancellationToken cancellationToken);
        public void MoveAxisInfinite(string direction, int speed);
        public void MoveAxisToPosition(AxisCommand step);
        public void StopAxis();
    }
}