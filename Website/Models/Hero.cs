namespace Website.Models
{
    public class Hero
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string Grade { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int FullExperience { get; set; }
        public bool Locked { get; set; }
        public bool InStorage { get; set; }
        public int[] Artifacts { get; set; }
    }
}