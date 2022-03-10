namespace piapp.Domain
{
    public record Procedure
    {
        public Procedure()
        {
            Steps = new List<Step>();
            Name = string.Empty;
            UserName = string.Empty;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public IEnumerable<Step> Steps { get; set; }
    }
}