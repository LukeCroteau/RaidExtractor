namespace Website.Models
{
    public class SkillType
    {
        public int Id { get; set; }
        public int Revision { get; set; }
        public Translation Name { get; set; }
        public Translation Description { get; set; }
        public SkillGroup Group { get; set; }
        public int Cooldown { get; set; }
        public bool ReduceCooldownProhibited { get; set; }
        public bool IsHidden { get; set; }
        public bool ShowDamageScale { get; set; }
        public Visibility Visibility { get; set; }
        public EffectType[] Effects { get; set; }
        public SkillTargets Targets { get; set; }
        public SkillBonus[] SkillLevelBonuses { get; set; }
        public HeroesSetInfo HeroesSetInfo { get; set; }
        public bool EnableVisualizationForPassiveSkill { get; set; }
    }

    public enum SkillBonusType : int
        {
            Attack = 0,
            Health = 1,
            EffectChance = 2,
            CooltimeTurn = 3,
            ShieldCreation = 4,
        }
        public enum SkillGroup : int
        {
            Active = 0,
            Passive = 1,
        }
        public enum SkillTargets : int
        {
            Producer = 0,
            AliveAllies = 1,
            AliveEnemies = 2,
            DeadAllies = 3,
            DeadEnemies = 4,
            AllAllies = 5,
            AllEnemies = 6,
            AliveAlliesExceptProducer = 7,
        }
        public enum Visibility : int
        {
            Visible = 0,
            HiddenOnHud = 1,
            HiddenOnHudWithVisualization = 2,
        }


        public class Translation
        {
            public string Key { get; set; }
            public string DefaultValue { get; set; }
        }

        public class HeroesSetInfo
        {
            public object[] TypeIds { get; set; }
            public int IgnoreDeath { get; set; }
        }

        public enum EffectGroup : int
        {
            Active = 0,
            Passive = 1,
        }


        public class EffectType
        {
            public int Id { get; set; }
            public EffectKindId KindId { get; set; }
            public EffectGroup Group { get; set; }
            public TargetParameters TargetParams { get; set; }
            public bool IsEffectDescription { get; set; }
            public bool ConsidersDead { get; set; }
            public bool LeaveThroughDeath { get; set; }
            public bool DoesntSetSkillOnCooldown { get; set; }
            public bool IgnoresCooldown { get; set; }
            public bool IsUnique { get; set; }
            public bool IterationChanceRolling { get; set; }
            public EffectRelation Relation { get; set; }
            public string Condition { get; set; }
            public int Count { get; set; }
            public int StackCount { get; set; }
            public string MultiplierFormula { get; set; }
            public string ValueCap { get; set; }
            public bool PersistsThroughRounds { get; set; }
            public HealParams HealParams { get; set; }
            public BlockEffectParams BlockEffectParams { get; set; }
            public long Chance { get; set; }
            public ApplyStatusEffectParams ApplyStatusEffectParams { get; set; }
            public ApplyMode ApplyInstantEffectMode { get; set; }
            public ActivatesKillParams ActivateSkillParams { get; set; }
            public ChangeSkillCooldownParams ChangeSkillCooldownParams { get; set; }
            public PassiveBonusParams PassiveBonusParams { get; set; }
            public ChangeEffectTargetParams ChangeEffectTargetParams { get; set; }
            public UnapplyStatusEffectParams UnapplyStatusEffectParams { get; set; }
            public DamageParams DamageParams { get; set; }
            public TeamAttackParams TeamAttackParams { get; set; }
            public ChangeEffectLifetimeParams ChangeEffectLifetimeParams { get; set; }
        }

        public enum EffectTargetType : int
        {
            Target = 0,
            Producer = 1,
            RelationTarget = 2,
            RelationProducer = 3,
            Owner = 4,
            RandomAlly = 5,
            RandomEnemy = 6,
            AllAllies = 7,
            AllEnemies = 8,
            AllDeadAllies = 9,
            RandomDeadAlly = 13,
            RandomDeadEnemy = 14,
            MostInjuredAlly = 19,
            MostInjuredEnemy = 20,
            Boss = 22,
            RandomRevivableAlly = 25,
            OwnerAllies = 26,
            AllHeroes = 29,
            ActiveHero = 31,
            AllyWithLowestMaxHp = 32,
            HeroCausedRelationUnapply = 33,
            HeroThatKilledProducer = 34,
            RelationTargetDuplicates = 35,
            AllyWithLowestStamina = 36,
            AllyWithHighestStamina = 37,
            EnemyWithLowestStamina = 38,
            EnemyWithHighestStamina = 39,
        }

        public enum UnapplyEffectSource : int
        {
            Death = 1,
            Revive = 2,
            RoundClear = 3,
            Effect = 4,
            Replacing = 5,
            Expiring = 6,
        }
        public enum TargetExclusion : int
        {
            Target = 0,
            Producer = 1,
            RelationTarget = 2,
            RelationProducer = 3,
        }


        public class TargetParameters
        {
            public EffectTargetType TargetType { get; set; }
            public bool Exclusive { get; set; }
            public bool FirstHitInSelected { get; set; }
            public TargetExclusion? Exclusion { get; set; }
        }

        public class EffectRelation
        {
            public EffectKindId[] EffectKindIds { get; set; }
            public BattlePhaseId Phase { get; set; }
            public bool ActivateOnGlancingHit { get; set; }
            public EffectKindId EffectTypeId { get; set; }
        }

        public enum BattlePhaseId : int
        {
            Unknown = 0,
            BattleStarted = 10,
            BattleFinished = 11,
            BeforeTurnStarted = 20,
            AfterTurnStarted = 21,
            TurnFinished = 22,
            RoundStarted = 30,
            RoundFinished = 31,
            BeforeEffectProcessed = 40,
            BeforeEffectProcessedOnTarget = 41,
            BeforeEffectAppliedOnTarget = 42,
            AfterEffectAppliedOnTarget = 43,
            AfterEffectProcessedOnTarget = 44,
            AfterEffectProcessed = 45,
            EffectExpired = 46,
            BeforeEffectChanceRolling = 47,
            AfterEffectChanceRolling = 48,
            TargetContextHasJustBeenCreated = 49,
            BeforeDamageCalculated = 50,
            AfterDamageCalculated = 51,
            BeforeDamageDealt = 52,
            AfterDamageDealt = 53,
            BlockDamageProcessing = 54,
            AfterDamageContextCreated = 55,
            BeforeHealDealt = 60,
            AfterHealDealt = 61,
            AllHeroesDeathProcessed = 70,
            HeroDead = 71,
            AfterSkillEffectsProcessed = 72,
            AfterHeroSummoned = 80,
            BeforeAppliedEffectsUpdate = 100,
            BeforeSkillQueued = 110,
            BeforeSkillProcessed = 111,
            AfterSkillUsed = 112,
            AfterAllSkillsUsed = 113,
            AfterStatusEffectToApplySelected = 120,
            CancelEffectProcessing = 130,
            BeforeEffectUnappliedFromHero = 140,
            AfterEffectUnappliedFromHero = 141,
        }
        
	public enum EffectKindId : int
	{
		Revive = 0,
		Heal = 1000,
		StartOfStatusBuff = 2000,
		BlockDamage = 2001,
		BlockDebuff = 2002,
		ContinuousHeal = 2003,
		Shield = 2004,
		StatusCounterAttack = 2005,
		ReviveOnDeath = 2006,
		ShareDamage = 2007,
		Unkillable = 2008,
		DamageCounter = 2009,
		ReflectDamage = 2010,
		HitCounterShield = 2012,
		Invisible = 2013,
		ReduceDamageTaken = 2014,
		StatusIncreaseAttack = 2101,
		StatusIncreaseDefence = 2102,
		StatusIncreaseSpeed = 2103,
		StatusIncreaseCriticalChance = 2104,
		StatusChangeDamageMultiplier = 2105,
		StatusIncreaseAccuracy = 2106,
		StatusIncreaseCriticalDamage = 2107,
		EndOfStatusBuff = 2999,
		StartOfStatusDebuff = 3000,
		Freeze = 3001,
		Provoke = 3002,
		Sleep = 3003,
		Stun = 3004,
		BlockHeal = 3005,
		BlockSkill = 3006,
		ContinuousDamage = 3007,
		BlockBuffs = 3008,
		TimeBomb = 3009,
		IncreaseDamageTaken = 3010,
		BlockRevive = 3011,
		Mark = 3012,
		LifeDrainOnDamage = 3013,
		AoEContinuousDamage = 3014,
		Fear = 3015,
		IncreasePoisoning = 3016,
		StatusReduceAttack = 3101,
		StatusReduceDefence = 3102,
		StatusReduceSpeed = 3103,
		StatusReduceCriticalChance = 3104,
		StatusReduceAccuracy = 3105,
		StatusReduceCriticalDamage = 3106,
		EndOfStatusDebuff = 3999,
		ApplyBuff = 4000,
		IncreaseStamina = 4001,
		TransferDebuff = 4002,
		RemoveDebuff = 4003,
		ActivateSkill = 4004,
		ShowHiddenSkill = 4005,
		TeamAttack = 4006,
		ExtraTurn = 4007,
		LifeShare = 4008,
		ReduceCooldown = 4009,
		ReduceDebuffLifetime = 4010,
		IncreaseBuffLifetime = 4011,
		PassiveCounterAttack = 4012,
		PassiveIncreaseStats = 4013,
		PassiveBlockDebuff = 4014,
		PassiveBonus = 4015,
		MultiplyBuff = 4016,
		PassiveReflectDamage = 4017,
		PassiveShareDamage = 4018,
		EndOfInstantBuff = 4999,
		ApplyDebuff = 5000,
		ReduceStamina = 5001,
		StealBuff = 5002,
		RemoveBuff = 5003,
		IncreaseCooldown = 5004,
		ReduceBuffLifetime = 5005,
		PassiveDebuffLifetime = 5006,
		SwapHealth = 5007,
		IncreaseDebuffLifetime = 5008,
		DestroyHp = 5009,
		Detonate = 5010,
		MultiplyDebuff = 5011,
		EndOfInstantDebuff = 5999,
		Damage = 6000,
		HitTypeModifier = 7000,
		ChangeDefenceModifier = 7001,
		ChangeEffectAccuracy = 7002,
		MultiplyEffectChance = 7003,
		ChangeDamageMultiplier = 7004,
		IgnoreProtectionEffects = 7005,
		ChangeCalculatedDamage = 7006,
		ChangeEffectRepeatCount = 7007,
		Summon = 8000,
		ChangeEffectTarget = 9000,
		CancelEffect = 9001,
		EffectDurationModifier = 10000,
		CheckTargetForCondition = 11000,
		EffectContainer = 11001,
	}

        public class HealParams
        {
            public bool CanBeCritical { get; set; }
        }

        public class BlockEffectParams
        {
            public int[] EffectKindIds { get; set; }
        }

        public class ApplyStatusEffectParams
        {
            public StatusEffectInfo[] StatusEffectInfos { get; set; }
        }

        public class StatusEffectInfo
        {
            public int TypeId { get; set; }
            public int Duration { get; set; }
            public bool IsProtected { get; set; }
            public bool IgnoreEffectsLimit { get; set; }
            public ApplyMode ApplyMode { get; set; }
        }

        public class ActivatesKillParams
        {
            public int SkillIndex { get; set; }
            public ActivateSkillOwner SkillOwner { get; set; }
        }

        public enum ActivateSkillOwner : int
        {
            Producer = 0,
            Target = 1,
        }

        public class ChangeSkillCooldownParams
        {
            public int Turns { get; set; }
            public SkillToChange SkillToChange { get; set; }
        }

        public enum SkillToChange : int
        {
            Random = 1,
            ByIndex = 2,
            SkillOfCurrentContext = 3,
            All = 4,
        }

        public class PassiveBonusParams
        {
            public PassiveBonus Bonus { get; set; }
        }

        public enum PassiveBonus : int
        {
            Heal = 1,
            ShieldCreation = 2,
            StaminaRecovery = 3,
            ArtifactSetStats = 4,
        }

        public class ChangeEffectTargetParams
        {
            public bool OverrideApplyMode { get; set; }
            public ApplyMode ApplyMode { get; set; }
        }
        public enum ApplyMode : int
        {
            Unresistable = 0,
            Guaranteed = 1,
        }

        public class UnapplyStatusEffectParams
        {
            public int Count { get; set; }
            public UnapplyEffectMode UnapplyMode { get; set; }
        }

        public enum UnapplyEffectMode : int
        {
            Selected = 0,
            AllExceptSelected = 1,
        }

        public class DamageParams
        {
            public int DefenceModifier { get; set; }
            public bool IgnoreDamageReduction { get; set; }
            public bool IgnoreBlockDamage { get; set; }
            public bool IgnoreShield { get; set; }
            public bool IgnoreUnkillable { get; set; }
            public bool IsFixed { get; set; }
            public bool DoesNotCountAsHit { get; set; }
            public int IncreaseCriticalHitChance { get; set; }
        }

        public class TeamAttackParams
        {
            public int TeammatesCount { get; set; }
            public bool ExcludeProducerFromAttack { get; set; }
        }

        public class ChangeEffectLifetimeParams
        {
            public AppliedEffectType Type { get; set; }
            public int Turns { get; set; }
            public int Count { get; set; }
        }

        public class SkillBonus
        {
            public SkillBonusType SkillBonusType { get; set; }
            public long Value { get; set; }
        }

        public enum AppliedEffectType : int
        {
            Buff = 0,
            Debuff = 1,
        }
}
