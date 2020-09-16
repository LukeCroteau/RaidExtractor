import { ArtifactBonus } from './clients';
import { IArtifactBonus } from './clients';
import { IArtifactBonus as IArtifactBonus1 } from './clients';

export abstract class Raid {
    static sets: { [index: string]: { name: string, set: number, bonuses: ArtifactBonus[] } } = {
    'None': { 'name': '', 'set': 1, 'bonuses': [] },
    'Hp': { 'name': 'Life', 'set': 2, 'bonuses': [Raid.percBonus('Health', 0.15)] },
    'AttackPower': { 'name': 'Offense', 'set': 2, 'bonuses': [Raid.percBonus('Attack', 0.15)]  },
    'Defense': { 'name': 'Defense', 'set': 2, 'bonuses': [Raid.percBonus('Defense', 0.15)] },
    'AttackSpeed': { 'name': 'Speed', 'set': 2, 'bonuses': [Raid.percBonus('Speed', 0.12)] },
    'CriticalChance': { 'name': 'Critical Rate', 'set': 2, 'bonuses': [Raid.percBonus('CriticalChance', 0.15)] },
    'CriticalDamage': { 'name': 'Critical Damage', 'set': 2, 'bonuses': [Raid.percBonus('CriticalDamage', 0.15)] },
    'Accuracy': { 'name': 'Accuracy', 'set': 2, 'bonuses': [Raid.percBonus('Accuracy', 40)] },
    'Resistance': { 'name': 'Resistance', 'set': 2, 'bonuses': [Raid.percBonus('Resistance', 40)] },
    'LifeDrain': { 'name': 'Lifesteal', 'set': 4, 'bonuses': [] },
    'DamageIncreaseOnHpDecrease': { 'name': 'Fury', 'set': 4, 'bonuses': [] },
    'SleepChance': { 'name': 'Daze', 'set': 4, 'bonuses': [] },
    'BlockHealChance': { 'name': 'Cursed', 'set': 4, 'bonuses': [] },
    'FreezeRateOnDamageReceived': { 'name': 'Frost', 'set': 4, 'bonuses': [] },
    'Stamina': { 'name': '', 'set': 4, 'bonuses': [] },
    'Heal': { 'name': 'Regeneration', 'set': 4, 'bonuses': [] },
    'BlockDebuff': { 'name': 'Immunity', 'set': 4, 'bonuses': [] },
    'Shield': { 'name': 'Shield', 'set': 4, 'bonuses': [] },
    'GetExtraTurn': { 'name': 'Relentless', 'set': 4, 'bonuses': [] },
    'IgnoreDefense': { 'name': 'Savage', 'set': 4, 'bonuses': [] },
    'DecreaseMaxHp': { 'name': 'Destroy', 'set': 4, 'bonuses': [] },
    'StunChance': { 'name': 'Stun', 'set': 4, 'bonuses': [] },
    'DotRate': { 'name': 'Toxic', 'set': 4, 'bonuses': [] },
    'ProvokeChance': { 'name': 'Taunting', 'set': 4, 'bonuses': [] },
    'Counterattack': { 'name': 'Retaliation', 'set': 4, 'bonuses': [] },
    'CounterattackOnCrit': { 'name': 'Avenging', 'set': 4, 'bonuses': [] },
    'AoeDamageDecrease': { 'name': 'Stalwart', 'set': 4, 'bonuses': [] },
    'CooldownReductionChance': { 'name': 'Reflex', 'set': 4, 'bonuses': [] },
    'CriticalHealMultiplier': { 'name': 'Curing', 'set': 4, 'bonuses': [] },
    'AttackPowerAndIgnoreDefense': { 'name': 'Cruel', 'set': 2, 'bonuses': [Raid.percBonus('Attack',0.15)] },
    'HpAndHeal': { 'name': 'Immortal', 'set': 4, 'bonuses': [Raid.percBonus('Health',0.15)] },
    'ShieldAndAttackPower': { 'name': 'Divine Offense', 'set': 2, 'bonuses': [Raid.percBonus('Attack',0.15)] },
    'ShieldAndCriticalChance': { 'name': 'Divine Critical Rate', 'set': 2, 'bonuses': [Raid.percBonus('CritcalChance',0.12)] },
    'ShieldAndHp': { 'name': 'Divine Life', 'set': 2, 'bonuses': [Raid.percBonus('Health',0.15)] },
    'ShieldAndSpeed': { 'name': 'Divine Speed', 'set': 2, 'bonuses': [Raid.percBonus('Speed',0.12)] },
    'UnkillableAndSpdAndCrDmg': { 'name': '', 'set': 4, 'bonuses': [Raid.percBonus('Speed',0.18),Raid.percBonus('CriticalDamage',0.3)] },
    'BlockReflectDebuffAndHpAndDef': { 'name': '', 'set': 4, 'bonuses': [Raid.percBonus('Health',0.2),Raid.percBonus('Defense',0.2)] },
    'HpAndDefence': { 'name': '', 'set': 2, 'bonuses': [Raid.percBonus('Health',0.1),Raid.percBonus('Defense',0.1)] },
    'AccuracyAndSpeed': { 'name': '', 'set': 2, 'bonuses': [Raid.absBonus('Accuracy',40),Raid.percBonus('Speed',0.05)] },
    'IgnoreCooldown': { 'name': '', 'set': 4, 'bonuses': [] },
    'RemoveDebuff': { 'name': '', 'set': 4, 'bonuses': [] }
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
