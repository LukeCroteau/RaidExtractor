namespace Website.Models
{
    public class Artifact
    {
        public int Id { get; set; }
        public int SellPrice { get; set; }
        public int Price { get; set; }
        public int Level { get; set; }
        public bool IsActivated { get; set; }
        public string Kind { get; set; }
        public string Rank { get; set; }
        public string Rarity { get; set; }
        public string SetKind { get; set; }
        public bool IsSeen { get; set; }
        public int FailedUpgrades { get; set; }
        public ArtifactBonus PrimaryBonus { get; set; }
        public ArtifactBonus[] SecondaryBonuses { get; set; }
        public string RequiredFraction { get; set; }
    }
}