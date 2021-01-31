import { AccountDump } from './clients';
import { Artifact } from './clients';
import { Hero } from './clients';
import { Raid } from './raid';
import { ArtifactBonus } from './clients';

export class RaidAccount {
  artifacts: Artifact[];
  heroes: Hero[];
  team: Hero[];
  artifactsByKind: { [kind: string]: Artifact[] } = {};
  artifactsById: { [id: number]: Artifact } = {};

  constructor(dump: AccountDump) {
    this.artifacts = dump.artifacts;
    for (let artifact of this.artifacts) {
      if (artifact.setKind === '38') artifact.setKind = 'AccuracyAndSpeed';
      if (artifact.setKind === '37') artifact.setKind = 'HpAndDefense';
      if (artifact.setKind) artifact.setKind = artifact.setKind.replace('Defence', 'Defense');
    }

    this.heroes = dump.heroes;
    this.team = [];

    for (let artifact of this.artifacts) {
      let arr = this.artifactsByKind[artifact.kind];
      if (!arr) arr = [];
      arr.push(artifact);
      this.artifactsByKind[artifact.kind] = arr;
      this.artifactsById[artifact.id] = artifact;
    }
  }

  getHeroStat(hero: Hero, stat: string): number {
    const sets: { [setKind: string]: number } = {};
    
    const baseValue = stat.startsWith('Crit') ? 100 : hero[Raid.statProperty[stat]];
    let value = hero[Raid.statProperty[stat]];
    if (!hero.artifacts) {
      return value;
    }

    for (let artifactId of hero.artifacts) {
      const artifact = this.artifactsById[artifactId];
      if (!artifact) {
        continue;
      }

      value += this.getBonusValue(artifact, baseValue, stat);
      sets[artifact.setKind] = (sets[artifact.setKind] || 0) + 1;
    }

    for (const set in sets) {
      if (Object.prototype.hasOwnProperty.call(sets, set)) {
        let count = sets[set];
        while (Raid.sets[set].setSize <= count) {
          count -= Raid.sets[set].setSize;
          for (const bonus of Raid.sets[set].bonuses) {
            value += this.calcBonusValue(bonus, baseValue, stat);
          }
        }
      }
    }

    return Math.round(value);
  }

  getHeroType(hero: Hero): string {
    return `${Raid.fraction[hero.fraction]}-${Raid.rarity[hero.rarity]}${Raid.role[hero.role]}${Raid.element[hero.element]}`;
  }

  getArtifactType(artifact: Artifact): string {
    let result = `${Raid.kind[artifact.kind]}-${Raid.rank[artifact.rank]}${Raid.rarity[artifact.rarity]}`;
    if (artifact.requiredFraction) {
      result += `-${Raid.fraction[artifact.requiredFraction]}`;
    }
    return result;
  }

  getArtifactSet(artifact: Artifact): string {
    if (!artifact || !Raid.sets[artifact.setKind]) return '';
    return Raid.sets[artifact.setKind].name;
  }

  getBonusValue(artifact: Artifact, baseValue: number, stat: string): number {
    if (!artifact) {
      return 0;
    }
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

    const value = bonus.value + bonus.enhancement;
    if (bonus.isAbsolute) {
      return value;
    } else {
      return Math.round(baseValue * value);
    }
  }

  getArtifactStat(artifact: Artifact, stat: string, isAbsolute: boolean): string {
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

  artifactToHtml(artifact: Artifact) {
    if (!artifact) {
      return '';
    }
    let result = `<b>${this.statToString(artifact.primaryBonus.value, artifact.primaryBonus.isAbsolute)}&nbsp;${Raid.statAbbr[artifact.primaryBonus.kind]}</b>`;
    for (let i = 0; i < artifact.secondaryBonuses.length; i ++) {
      if (i === 0) result += '&nbsp;-&nbsp;';
      if (i !== 0) result += '&nbsp;/&nbsp;';
      result += `${this.statToString(artifact.secondaryBonuses[i].value, artifact.secondaryBonuses[i].isAbsolute)}&nbsp;${Raid.statAbbr[artifact.secondaryBonuses[i].kind]}`;
    }
    return result;
  }

  artifactToHtmlMultiline(artifact: Artifact) {
    if (!artifact) {
      return '';
    }
    let result = `<b>${this.statToString(artifact.primaryBonus.value, artifact.primaryBonus.isAbsolute)}&nbsp;${Raid.statAbbr[artifact.primaryBonus.kind]}</b><br/>`;
    for (let i = 0; i < artifact.secondaryBonuses.length; i ++) {
      result += `${this.statToString(artifact.secondaryBonuses[i].value, artifact.secondaryBonuses[i].isAbsolute)}&nbsp;${Raid.statAbbr[artifact.secondaryBonuses[i].kind]}<br/>`;
    }
    return result;
  }

  grades(hero: Hero): { isAwakened: boolean }[] {
    const result: { isAwakened: boolean }[] = [];
    const grade = parseInt(hero.grade.replace('Stars', ''));
    for (let i = 0; i < grade; i ++) {
      result.push({ isAwakened: i < hero.awakenLevel });
    }
    return result;
  }
}
