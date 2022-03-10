namespace piapp.Domain
{
    public record AxisStepParams
    {
        public string Mode { get; set; }
        public int Direction { get; set; }
        public int Speed { get; set; }
        public int Distance { get; set; }
    }
}
