import { ArtifactBonus } from './clients';
import { IArtifactBonus } from './clients';
import { IArtifactBonus as IArtifactBonus1 } from './clients';

export abstract class Raid {
    static sets: { [index: string]: { name: string, setSize: number, bonuses: ArtifactBonus[] } } = {
    'None': { 'name': '', 'setSize': 1, 'bonuses': [] },
    'Hp': { 'name': 'Life', 'setSize': 2, 'bonuses': [Raid.percBonus('Health', 0.15)] },
    'AttackPower': { 'name': 'Offense', 'setSize': 2, 'bonuses': [Raid.percBonus('Attack', 0.15)]  },
    'Defense': { 'name': 'Defense', 'setSize': 2, 'bonuses': [Raid.percBonus('Defense', 0.15)] },
    'AttackSpeed': { 'name': 'Speed', 'setSize': 2, 'bonuses': [Raid.percBonus('Speed', 0.12)] },
    'CriticalChance': { 'name': 'Critical Rate', 'setSize': 2, 'bonuses': [Raid.percBonus('CriticalChance', 0.15)] },
    'CriticalDamage': { 'name': 'Critical Damage', 'setSize': 2, 'bonuses': [Raid.percBonus('CriticalDamage', 0.15)] },
    'Accuracy': { 'name': 'Accuracy', 'setSize': 2, 'bonuses': [Raid.absBonus('Accuracy', 40)] },
    'Resistance': { 'name': 'Resistance', 'setSize': 2, 'bonuses': [Raid.absBonus('Resistance', 40)] },
    'LifeDrain': { 'name': 'Lifesteal', 'setSize': 4, 'bonuses': [] },
    'DamageIncreaseOnHpDecrease': { 'name': 'Fury', 'setSize': 4, 'bonuses': [] },
    'SleepChance': { 'name': 'Daze', 'setSize': 4, 'bonuses': [] },
    'BlockHealChance': { 'name': 'Cursed', 'setSize': 4, 'bonuses': [] },
    'FreezeRateOnDamageReceived': { 'name': 'Frost', 'setSize': 4, 'bonuses': [] },
    'Stamina': { 'name': '', 'setSize': 4, 'bonuses': [] },
    'Heal': { 'name': 'Regeneration', 'setSize': 4, 'bonuses': [] },
    'BlockDebuff': { 'name': 'Immunity', 'setSize': 4, 'bonuses': [] },
    'Shield': { 'name': 'Shield', 'setSize': 4, 'bonuses': [] },
    'GetExtraTurn': { 'name': 'Relentless', 'setSize': 4, 'bonuses': [] },
    'IgnoreDefense': { 'name': 'Savage', 'setSize': 4, 'bonuses': [] },
    'DecreaseMaxHp': { 'name': 'Destroy', 'setSize': 4, 'bonuses': [] },
    'StunChance': { 'name': 'Stun', 'setSize': 4, 'bonuses': [] },
    'DotRate': { 'name': 'Toxic', 'setSize': 4, 'bonuses': [] },
    'ProvokeChance': { 'name': 'Taunting', 'setSize': 4, 'bonuses': [] },
    'Counterattack': { 'name': 'Retaliation', 'setSize': 4, 'bonuses': [] },
    'CounterattackOnCrit': { 'name': 'Avenging', 'setSize': 4, 'bonuses': [] },
    'AoeDamageDecrease': { 'name': 'Stalwart', 'setSize': 4, 'bonuses': [] },
    'CooldownReductionChance': { 'name': 'Reflex', 'setSize': 4, 'bonuses': [] },
    'CriticalHealMultiplier': { 'name': 'Curing', 'setSize': 4, 'bonuses': [] },
    'AttackPowerAndIgnoreDefense': { 'name': 'Cruel', 'setSize': 2, 'bonuses': [Raid.percBonus('Attack',0.15)] },
    'HpAndHeal': { 'name': 'Immortal', 'setSize': 4, 'bonuses': [Raid.percBonus('Health',0.15)] },
    'ShieldAndAttackPower': { 'name': 'Divine Offense', 'setSize': 2, 'bonuses': [Raid.percBonus('Attack',0.15)] },
    'ShieldAndCriticalChance': { 'name': 'Divine Critical Rate', 'setSize': 2, 'bonuses': [Raid.percBonus('CritcalChance',0.12)] },
    'ShieldAndHp': { 'name': 'Divine Life', 'setSize': 2, 'bonuses': [Raid.percBonus('Health',0.15)] },
    'ShieldAndSpeed': { 'name': 'Divine Speed', 'setSize': 2, 'bonuses': [Raid.percBonus('Speed',0.12)] },
    'UnkillableAndSpdAndCrDmg': { 'name': '', 'setSize': 4, 'bonuses': [Raid.percBonus('Speed',0.18),Raid.percBonus('CriticalDamage',0.3)] },
    'BlockReflectDebuffAndHpAndDef': { 'name': '', 'setSize': 4, 'bonuses': [Raid.percBonus('Health',0.2),Raid.percBonus('Defense',0.2)] },
    'HpAndDefence': { 'name': '', 'setSize': 2, 'bonuses': [Raid.percBonus('Health',0.1),Raid.percBonus('Defense',0.1)] },
    'AccuracyAndSpeed': { 'name': '', 'setSize': 2, 'bonuses': [Raid.absBonus('Accuracy',40),Raid.percBonus('Speed',0.05)] },
    'IgnoreCooldown': { 'name': '', 'setSize': 4, 'bonuses': [] },
    'RemoveDebuff': { 'name': '', 'setSize': 4, 'bonuses': [] }
  }

  
    static percBonus(kind: string, value: number): ArtifactBonus {
      return new ArtifactBonus(<IArtifactBonus>{
        'kind': kind,
        'isAbsolute': false,
        'value': value,
        'level': 0,
        'enhancement': 0
      });
    }

    static absBonus(kind: string, value: number): ArtifactBonus {
      return new ArtifactBonus(<IArtifactBonus1>{
        'kind': kind,
        'isAbsolute': true,
        'value': value,
        'level': 0,
        'enhancement': 0
      });
    }


  static fraction = {
    'BannerLords': 'BL',
    'HighElves': 'HI',
    'SacredOrder': 'SO',
    'CovenOfMagi': 'CM',
    'OgrynTribes': 'OT',
    'LizardMen': 'LZ',
    'Skinwalkers': 'SW',
    'Orcs': 'OR',
    'Demonspawn': 'DS',
    'UndeadHordes': 'UH',
    'DarkElves': 'DE',
    'KnightsRevenant': 'KR',
    'Barbarians': 'BA',
    'NyresanElves': 'NE',
    'AssassinsGuild': 'AG',
    'Dwarves': 'DW'
  }

  static role = {
    'Attack': 'A',
    'Defense': 'D',
    'Health': 'H',
    'Support': 'S',
    'Evolve': 'E',
    'Xp': 'X'
  }

  static element = {
    'Magic': 'M',
    'Force': 'F',
    'Spirit': 'S',
    'Void': 'V'
  }

  static statProperty = {
    'Health': 'health',
    'Attack': 'attack',
    'Defense': 'defense',
    'Accuracy': 'accuracy',
    'Resistance': 'resistance',
    'Speed': 'speed',
    'CriticalChance': 'criticalChance',
    'CriticalDamage': 'criticalDamage'
  }

  static statAbbr = {
    'Health': 'HP',
    'Attack': 'Atk',
    'Defense': 'Def',
    'Accuracy': 'Acc',
    'Resistance': 'Res',
    'Speed': 'Spd',
    'CriticalChance': 'C.Rate',
    'CriticalDamage': 'C.Dmg'
  }

  static kind = {
    'Helmet': 'HE',
    'Chest': 'CH',
    'Gloves': 'GO',
    'Boots': 'BO',
    'Weapon': 'WE',
    'Shield': 'SH',
    'Ring': 'RI',
    'Cloak': 'CL',
    'Banner': 'BA'
  };

  static rank = {
    'One': '1',
    'Two': '2',
    'Three': '3',
    'Four': '4',
    'Five': '5',
    'Six': '6',
  }

  static rarity = {
    'Common': 'C',
    'Uncommon': 'U',
    'Rare': 'R',
    'Epic': 'E',
    'Legendary': 'L',
  }

}
