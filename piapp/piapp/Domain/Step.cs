using piapp.Infrastructure;

namespace piapp.Domain
{
    public record Step
    {
        public int Id { get; set; }

        public AxisStepParams XAxis { get; set; } = new AxisStepParams();

        public AxisStepParams YAxis { get; set; } = new AxisStepParams();
    }
}