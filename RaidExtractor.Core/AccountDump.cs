using System.Collections.Generic;

namespace RaidExtractor.Core
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class AccountDump
    {
        [Newtonsoft.Json.JsonProperty("artifacts", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Artifact> Artifacts { get; set; }

        [Newtonsoft.Json.JsonProperty("heroes", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Hero> Heroes { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Artifact
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        public int Id { get; set; }

        [Newtonsoft.Json.JsonProperty("sellPrice", Required = Newtonsoft.Json.Required.Always)]
        public int SellPrice { get; set; }

        [Newtonsoft.Json.JsonProperty("price", Required = Newtonsoft.Json.Required.Always)]
        public int Price { get; set; }

        [Newtonsoft.Json.JsonProperty("level", Required = Newtonsoft.Json.Required.Always)]
        public int Level { get; set; }

        [Newtonsoft.Json.JsonProperty("isActivated", Required = Newtonsoft.Json.Required.Always)]
        public bool IsActivated { get; set; }

        [Newtonsoft.Json.JsonProperty("kind", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Kind { get; set; }

        [Newtonsoft.Json.JsonProperty("rank", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Rank { get; set; }

        [Newtonsoft.Json.JsonProperty("rarity", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Rarity { get; set; }

        [Newtonsoft.Json.JsonProperty("setKind", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string SetKind { get; set; }

        [Newtonsoft.Json.JsonProperty("isSeen", Required = Newtonsoft.Json.Required.Always)]
        public bool IsSeen { get; set; }

        [Newtonsoft.Json.JsonProperty("failedUpgrades", Required = Newtonsoft.Json.Required.Always)]
        public int FailedUpgrades { get; set; }

        [Newtonsoft.Json.JsonProperty("primaryBonus", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ArtifactBonus PrimaryBonus { get; set; }

        [Newtonsoft.Json.JsonProperty("secondaryBonuses", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<ArtifactBonus> SecondaryBonuses { get; set; }

        [Newtonsoft.Json.JsonProperty("requiredFraction", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string RequiredFraction { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ArtifactBonus
    {
        [Newtonsoft.Json.JsonProperty("kind", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Kind { get; set; }

        [Newtonsoft.Json.JsonProperty("isAbsolute", Required = Newtonsoft.Json.Required.Always)]
        public bool IsAbsolute { get; set; }

        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.Always)]
        public float Value { get; set; }

        [Newtonsoft.Json.JsonProperty("enhancement", Required = Newtonsoft.Json.Required.Always)]
        public float Enhancement { get; set; }

        [Newtonsoft.Json.JsonProperty("level", Required = Newtonsoft.Json.Required.Always)]
        public int Level { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Hero
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        public int Id { get; set; }

        [Newtonsoft.Json.JsonProperty("typeId", Required = Newtonsoft.Json.Required.Always)]
        public int TypeId { get; set; }

        [Newtonsoft.Json.JsonProperty("grade", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Grade { get; set; }

        [Newtonsoft.Json.JsonProperty("level", Required = Newtonsoft.Json.Required.Always)]
        public int Level { get; set; }

        [Newtonsoft.Json.JsonProperty("experience", Required = Newtonsoft.Json.Required.Always)]
        public int Experience { get; set; }

        [Newtonsoft.Json.JsonProperty("fullExperience", Required = Newtonsoft.Json.Required.Always)]
        public int FullExperience { get; set; }

        [Newtonsoft.Json.JsonProperty("locked", Required = Newtonsoft.Json.Required.Always)]
        public bool Locked { get; set; }

        [Newtonsoft.Json.JsonProperty("inStorage", Required = Newtonsoft.Json.Required.Always)]
        public bool InStorage { get; set; }

        [Newtonsoft.Json.JsonProperty("artifacts", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<int> Artifacts { get; set; }

        [Newtonsoft.Json.JsonProperty("fraction", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Fraction { get; set; }

        [Newtonsoft.Json.JsonProperty("rarity", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Rarity { get; set; }

        [Newtonsoft.Json.JsonProperty("role", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Role { get; set; }

        [Newtonsoft.Json.JsonProperty("element", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Element { get; set; }

        [Newtonsoft.Json.JsonProperty("awakenLevel", Required = Newtonsoft.Json.Required.Always)]
        public int AwakenLevel { get; set; }

        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("health", Required = Newtonsoft.Json.Required.Always)]
        public float Health { get; set; }

        [Newtonsoft.Json.JsonProperty("accuracy", Required = Newtonsoft.Json.Required.Always)]
        public float Accuracy { get; set; }

        [Newtonsoft.Json.JsonProperty("attack", Required = Newtonsoft.Json.Required.Always)]
        public float Attack { get; set; }

        [Newtonsoft.Json.JsonProperty("defense", Required = Newtonsoft.Json.Required.Always)]
        public float Defense { get; set; }

        [Newtonsoft.Json.JsonProperty("criticalChance", Required = Newtonsoft.Json.Required.Always)]
        public float CriticalChance { get; set; }

        [Newtonsoft.Json.JsonProperty("criticalDamage", Required = Newtonsoft.Json.Required.Always)]
        public float CriticalDamage { get; set; }

        [Newtonsoft.Json.JsonProperty("criticalHeal", Required = Newtonsoft.Json.Required.Always)]
        public float CriticalHeal { get; set; }

        [Newtonsoft.Json.JsonProperty("resistance", Required = Newtonsoft.Json.Required.Always)]
        public float Resistance { get; set; }

        [Newtonsoft.Json.JsonProperty("speed", Required = Newtonsoft.Json.Required.Always)]
        public float Speed { get; set; }

        [Newtonsoft.Json.JsonProperty("masteries", Required = Newtonsoft.Json.Required.Always)]
        public List<int> Masteries { get; set; }
    }
}
