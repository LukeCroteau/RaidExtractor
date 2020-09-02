namespace Website.Models
{
    public class ArtifactBonus
    {
        public string Kind { get; set; }
        public bool IsAbsolute { get; set; }
        public float Value { get; set; }
        public float Enhancement { get; set; }
        public int Level { get; set; }
    }
}