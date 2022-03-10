namespace piapp.Infrastructure
{
    public record MotorParams
    {
        public int Speed { get; set; }
        public int Steps { get; set; }
        public int StepsToDo { get; set; }
        public MotorMode Mode { get; set; }
    }
}
