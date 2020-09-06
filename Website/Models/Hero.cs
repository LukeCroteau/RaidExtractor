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
        public string Fraction { get; set; }
        public string Rarity { get; set; }
        public string Role { get; set; }
        public string Element { get; set; }
        public int AwakenLevel { get; set; }
        public string Name { get; set; }
        public float Health { get; set; }
        public float Accuracy { get; set; }
        public float Attack { get; set; }
        public float Defense { get; set; }
        public float CriticalChance { get; set; }
        public float CriticalDamage { get; set; }
        public float CriticalHeal { get; set; }
        public float Resistance { get; set; }
        public float Speed { get; set; }
    }
}
