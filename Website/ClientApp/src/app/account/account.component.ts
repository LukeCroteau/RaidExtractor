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
  artifactsByKind: { [kind: string]: Artifact[] } = {};
  artifactsById: { [id: number]: Artifact } = {};

  ngOnInit(): void {
    this.route.params.subscribe((params: ParamMap) => {
      if ('key' in params) {
        this.accountService.get(params['key']).subscribe(dump => {
          if (!dump) {
            return;
          }

          this.account = dump;
          for (let artifact of this.account.artifacts) {
            let arr = this.artifactsByKind[artifact.kind];
            if (!arr) arr = [];
            arr.push(artifact);
            this.artifactsByKind[artifact.kind] = arr;
            this.artifactsById[artifact.id] = artifact;
          }
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

  setMap: { [index: string]: { name: string, set: number, bonuses: ArtifactBonus[] } } = {
    'None': { 'name': '', 'set': 1, 'bonuses': [] },
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

  test() {
    const hero = this.account.heroes[0];
    const weights = [0.01,0.5,0.5,1,0,10,1,1];

    const artifactsByKind = {};
    for (let kind in this.artifactsByKind) {
      const arr = [];
      for (let artifact of this.artifactsByKind[kind]) {
        if (artifact.requiredFraction && artifact.requiredFraction !== hero.fraction) {
          continue;
        }

        const bonuses = this.getBonuses(hero, artifact);
        let score = 0;
        for(let i = 0; i < bonuses.length; i ++) score += bonuses[i] * weights[i];
        arr.push({
          artifact: artifact,
          bonuses: bonuses,
          score: score
        });
      }
      if (arr.length == 0) {
        arr.push({
          artifact: null,
          bonuses: [0,0,0,0,0,0,0,0],
          score: 0
        })
      }
      artifactsByKind[kind] = arr.sort((a, b) => b.score - a.score);
    }

    let bestSetScore = 0;
    const setByKind = {};
    for (let set in this.setMap) {
      // TODO: Add set base-score to 'stear' towards a certain set
      setByKind[set] = {
        'set': this.setMap[set].set,
        'bonuses': [0,0,0,0,0,0,0,0],
        'score': 0,
        'maxScore': 0
      };
      for (const bonus of this.setMap[set].bonuses) {
        setByKind[set].bonuses[0] += this.calcBonusValue(bonus, hero.health, 'Health');
        setByKind[set].bonuses[1] += this.calcBonusValue(bonus, hero.attack, 'Attack');
        setByKind[set].bonuses[2] += this.calcBonusValue(bonus, hero.defense, 'Defence');
        setByKind[set].bonuses[3] += this.calcBonusValue(bonus, hero.accuracy, 'Accuracy');
        setByKind[set].bonuses[4] += this.calcBonusValue(bonus, hero.resistance, 'Resistance');
        setByKind[set].bonuses[5] += this.calcBonusValue(bonus, hero.speed, 'Speed');
        setByKind[set].bonuses[6] += this.calcBonusValue(bonus, 100, 'CriticalChance');
        setByKind[set].bonuses[7] += this.calcBonusValue(bonus, 100, 'CriticalDamage');
      }
      for(let i = 0; i < setByKind[set].bonuses.length; i ++) setByKind[set].score += setByKind[set].bonuses[i] * weights[i];
      setByKind[set].maxScore = setByKind[set].score * (setByKind[set].set == 2 ? 3 : 1);

      bestSetScore = Math.max(bestSetScore, setByKind[set].score);
    }

    let bestBonuses = [];
    let bestScore = 0;
    let bestArtifacts = [];
    for (let weapon of artifactsByKind['Weapon']) {
      // If the maximum set-score is lower than the difference between this and the previous artifact, then no point in continue'ing to look at this artifact
      if (artifactsByKind['Weapon'][0].score - weapon.score > bestSetScore) break;
      for (let helmet of artifactsByKind['Helmet']) {
        if (artifactsByKind['Helmet'][0].score - helmet.score > bestSetScore) break;
        for (let shield of artifactsByKind['Shield']) {
          if (artifactsByKind['Shield'][0].score - shield.score > bestSetScore) break;
          for (let gloves of artifactsByKind['Gloves']) {
            if (artifactsByKind['Gloves'][0].score - gloves.score > bestSetScore) break;
            for (let chest of artifactsByKind['Chest']) {
              if (artifactsByKind['Chest'][0].score - chest.score > bestSetScore) break;
              for (let boots of artifactsByKind['Boots']) {
                if (artifactsByKind['Boots'][0].score - boots.score > bestSetScore) break;
                const sets = {};
                this.addSet(sets, weapon);
                this.addSet(sets, helmet);
                this.addSet(sets, shield);
                this.addSet(sets, gloves);
                this.addSet(sets, chest);
                this.addSet(sets, boots);
                let setBonuses = [0,0,0,0,0,0,0,0];
                let setScore = 0;
                for (let setKind in sets) {
                  while (sets[setKind] > setByKind[setKind]) {
                    sets[setKind] -= setByKind[setKind].set;
                    setScore += setByKind[setKind].score;
                  }
                }

                for (let ring of artifactsByKind['Ring']) { 
                  for (let cloak of artifactsByKind['Cloak']) {
                    for (let banner of artifactsByKind['Banner']) {
                      const bonuses = [hero.health,hero.attack,hero.defense,hero.accuracy,hero.resistance,hero.speed,hero.criticalChance,hero.criticalDamage];
                      let score = 0;
                      for (let i = 0; i < bonuses.length; i++) {
                        bonuses[i] += weapon.bonuses[i] + helmet.bonuses[i] + shield.bonuses[i] + gloves.bonuses[i] + chest.bonuses[i] + boots.bonuses[i] + ring.bonuses[i] + cloak.bonuses[i] + banner.bonuses[i] + setBonuses[i];
                        // HACK: CriticalChance = 6 - soft cap
                        if (i == 6 && bonuses[i] > 100) bonuses[i] = 100;
                        score += bonuses[i] * weights[i];
                      }

                      if (score < bestScore) continue;

                      bestScore = score;
                      bestBonuses = bonuses;
                      bestArtifacts = [helmet, weapon, shield, gloves, chest, boots, ring, cloak, banner];
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    console.log('bestBonuses', bestBonuses);
    console.log('bestScore', bestScore);
    console.log('bestArtifacts', bestArtifacts);
  }

  addSet(sets: any, artifact: Artifact): any {
    if (artifact.setKind in sets) {
      sets[artifact.setKind]++;
    } else {
      sets[artifact.setKind] = 1;
    }
    return sets;
  }

  getBonuses(hero: Hero, artifact: Artifact): number[] {
    const result = [];
    result.push(this.getBonusValue(artifact, hero.health, 'Health'));
    result.push(this.getBonusValue(artifact, hero.attack, 'Attack'));
    result.push(this.getBonusValue(artifact, hero.defense, 'Defence'));
    result.push(this.getBonusValue(artifact, hero.accuracy, 'Accuracy'));
    result.push(this.getBonusValue(artifact, hero.resistance, 'Resistance'));
    result.push(this.getBonusValue(artifact, hero.speed, 'Speed'));
    result.push(this.getBonusValue(artifact, 100, 'CriticalChance'));
    result.push(this.getBonusValue(artifact, 100, 'CriticalDamage'));
    return result;
  }
}
