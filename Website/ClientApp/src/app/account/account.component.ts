import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { AccountService } from '../shared/account.service';
import { AccountDump } from '../shared/clients';
import { Artifact } from '../shared/clients';
import { Hero } from '../shared/clients';
import { ArtifactBonus } from '../shared/clients';
import { IArtifactBonus } from '../shared/clients';

@Component({
  selector: 'account-dump',
  templateUrl: './account.component.html',
})
export class AccountComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private accountService: AccountService
  ) { }

  account: AccountDump;

  ngOnInit(): void {
    this.route.params.subscribe((params: ParamMap) => {
      if ('key' in params) {
        this.accountService.get(params['key']).subscribe(dump => {
          if (!dump) {
            return;
          }

          this.account = dump;
        });
      } else {
        this.router.navigate(['/']);
      }
    });
  }

  kindMap = {
    'Helmet': 'HE',
    'Chest': 'CH',
    'Gloves': 'GO',
    'Boots': 'BO',
    'Weapon': 'WE',
    'Shield': 'SH',
    'Ring': 'RI',
    'Cloak': 'CL',
    'Banner': 'BA'
  }

  rankMap = {
    'One': '1',
    'Two': '2',
    'Three': '3',
    'Four': '4',
    'Five': '5',
    'Six': '6',
  }

  rarityMap = {
    'Common': 'C',
    'Uncommon': 'U',
    'Rare': 'R',
    'Epic': 'E',
    'Legendary': 'L',
  }

  getArtifactType(artifact: Artifact): string {
    return `${this.kindMap[artifact.kind]}-${this.rankMap[artifact.rank]}${this.rarityMap[artifact.rarity]}`;
  }

  getArtifactSet(artifact: Artifact): string {
    return this.setMap[artifact.setKind].name;
  }

  getArtifactStat(artifact: Artifact, stat: string, isAbsolute: boolean) {
    if (artifact.primaryBonus.kind === stat && artifact.primaryBonus.isAbsolute === isAbsolute) {
      return this.statToString(artifact.primaryBonus.value, artifact.primaryBonus.isAbsolute);
    }
    if (!artifact.secondaryBonuses) {
      return '';
    }
    for (var bonus of artifact.secondaryBonuses) {
      if (bonus.kind === stat && bonus.isAbsolute === isAbsolute) {
        return this.statToString(bonus.value, bonus.isAbsolute);
      }
    }
    return '';
  }

  statToString(value: number, isAbsolute: boolean) {
    if (isAbsolute) return value.toString();
    value *= 100;
    return `${Math.round(value)}%`;
  }

  fractionMap = {
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

  roleMap = {
    'Attack': 'A',
    'Defense': 'D',
    'Health': 'H',
    'Support': 'S',
    'Evolve': 'E',
    'Xp': 'X'
  }

  elementMap = {
    'Magic': 'M',
    'Force': 'F',
    'Spirit': 'S',
    'Void': 'V'
  }

  getHeroType(hero: Hero): string {
    return `${this.fractionMap[hero.fraction]}-${this.rarityMap[hero.rarity]}${this.roleMap[hero.role]}${this.elementMap[hero.element]}`;
  }

  getArtifactById(artifactId: number): Artifact {
    for (let artifact of this.account.artifacts) {
      if (artifact.id === artifactId) {
        return artifact;
      }
    }
    return null;
  }

  percBonus(kind: string, value: number): ArtifactBonus {
    return new ArtifactBonus(<IArtifactBonus>{
      'kind': kind,
      'isAbsolute': false,
      'value': value,
      'level': 0,
      'enhancement': 0
    });
  }

  absBonus(kind: string, value: number): ArtifactBonus {
    return new ArtifactBonus(<IArtifactBonus>{
      'kind': kind,
      'isAbsolute': true,
      'value': value,
      'level': 0,
      'enhancement': 0
    });
  }

  setMap = {
    'None': { 'name': '' },
    'Hp': { 'name': 'Life', 'set': 2, 'bonuses': [this.percBonus('Health', 0.15)] },
    'AttackPower': { 'name': 'Offense', 'set': 2, 'bonuses': [this.percBonus('Attack', 0.15)]  },
    'Defense': { 'name': 'Defense', 'set': 2, 'bonuses': [this.percBonus('Defense', 0.15)] },
    'AttackSpeed': { 'name': 'Speed', 'set': 2, 'bonuses': [this.percBonus('Speed', 0.12)] },
    'CriticalChance': { 'name': 'Critical Rate', 'set': 2, 'bonuses': [this.percBonus('CriticalChance', 0.15)] },
    'CriticalDamage': { 'name': 'Critical Damage', 'set': 2, 'bonuses': [this.percBonus('CriticalDamage', 0.15)] },
    'Accuracy': { 'name': 'Accuracy', 'set': 2, 'bonuses': [this.percBonus('Accuracy', 40)] },
    'Resistance': { 'name': 'Resistance', 'set': 2, 'bonuses': [this.percBonus('Resistance', 40)] },
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
    'AttackPowerAndIgnoreDefense': { 'name': 'Cruel', 'set': 2, 'bonuses': [this.percBonus('Attack',0.15)] },
    'HpAndHeal': { 'name': 'Immortal', 'set': 4, 'bonuses': [this.percBonus('Health',0.15)] },
    'ShieldAndAttackPower': { 'name': 'Divine Offense', 'set': 2, 'bonuses': [this.percBonus('Attack',0.15)] },
    'ShieldAndCriticalChance': { 'name': 'Divine Critical Rate', 'set': 2, 'bonuses': [this.percBonus('CritcalChance',0.12)] },
    'ShieldAndHp': { 'name': 'Divine Life', 'set': 2, 'bonuses': [this.percBonus('Health',0.15)] },
    'ShieldAndSpeed': { 'name': 'Divine Speed', 'set': 2, 'bonuses': [this.percBonus('Speed',0.12)] },
    'UnkillableAndSpdAndCrDmg': { 'name': '', 'set': 4, 'bonuses': [] },
    'BlockReflectDebuffAndHpAndDef': { 'name': '', 'set': 4, 'bonuses': [] },
    'IgnoreCooldown': { 'name': '', 'set': 4, 'bonuses': [] },
    'RemoveDebuff': { 'name': '', 'set': 4, 'bonuses': [] }
  }

  statPropertyMap = {
    'Health': 'health',
    'Attack': 'attack',
    'Defence': 'defense',
    'Accuracy': 'accuracy',
    'Resistance': 'resistance',
    'Speed': 'speed',
    'CriticalChance': 'criticalChance',
    'CriticalDamage': 'criticalDamage'
  }

  getHeroStat(hero: Hero, stat: string): number {
    const sets = {};
    
    const baseValue = stat.startsWith('Crit') ? 100 : hero[this.statPropertyMap[stat]];
    let value = hero[this.statPropertyMap[stat]];
    if (!hero.artifacts) {
      return value;
    }

    for (let artifactId of hero.artifacts) {
      const artifact = this.getArtifactById(artifactId);
      if (!artifact) {
        continue;
      }

      value += this.getBonusValue(artifact, baseValue, stat);
      if (artifact.setKind in sets) {
        sets[artifact.setKind] ++;
      } else {
        sets[artifact.setKind] = 1;
      }
    }

    for (const set in sets) {
      if (Object.prototype.hasOwnProperty.call(sets, set)) {
        let count = sets[set];
        while (this.setMap[set].set <= count) {
          count -= this.setMap[set].set;
          for (const bonus of this.setMap[set].bonuses) {
            value += this.calcBonusValue(bonus, baseValue, stat);
          }
        }
      }
    }

    return Math.round(value);
  }

  getBonusValue(artifact: Artifact, baseValue: number, stat: string): number {
    let bonus = 0;
    bonus += this.calcBonusValue(artifact.primaryBonus, baseValue, stat);

    if (artifact.secondaryBonuses) {
      for (let secondaryBonus of artifact.secondaryBonuses) {
        bonus += this.calcBonusValue(secondaryBonus, baseValue, stat);
      }
    }

    return bonus;
  }

  calcBonusValue(bonus: ArtifactBonus, baseValue: number, stat: string): number {
    if (stat !== bonus.kind) {
      return 0;
    }
    if (bonus.isAbsolute) {
      return bonus.value;
    } else {
      return baseValue * bonus.value;
    }
  }
}
