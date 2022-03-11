using piapp.Infrastructure;

namespace piapp.Domain
{
    public record PositionCommand
    {
        public int Id { get; set; }

        public AxisCommand XAxis { get; set; } = new AxisCommand();

        public AxisCommand YAxis { get; set; } = new AxisCommand();
    }
}