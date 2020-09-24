import { Subscription, interval } from 'rxjs';
import { ArtifactOptimizerSettings } from './artifact-optimizer-settings';
import { StatValue } from './stat-value';
import { Artifact } from './clients';
import { Raid } from './raid';

const numberOfStats = 8;
const numberOfSlots = 9;

interface IArtifactWithScore {
  artifact?: Artifact;
  bonuses: number[];
  score: number;
  maxScore: number; // Max score includes highest possible set-bonus, used for sorting
}

interface IArtifactSetScore {
  setKind: string;
  setSize: number;
  bonuses: number[];
  baseScore: number;
  score: number;
  maxScore: number;
}

export interface IArtifactCombination {
  score: number;
  bonuses: number[];
  artifacts: Artifact[];
}

export class ArtifactOptimizerWorker {
  iterations: number = 100;
  isRunning: boolean = false;

  constructor(
    private settings: ArtifactOptimizerSettings
  ) {
    this.applyStatValues(settings.weights, this.weights);
    this.applyStatValues(settings.minValues, this.minValues);
    this.applyStatValues(settings.softCap, this.softCaps);
    this.applyStatValues(settings.hardCap, this.hardCaps);

    this.maxSetScore = 0;
    for (let setKind in Raid.sets) {
      if (!Raid.sets.hasOwnProperty(setKind)) {
        continue;
      }

      const bonuses = [0, 0, 0, 0, 0, 0, 0, 0];

      for (const bonus of Raid.sets[setKind].bonuses) {
        // This can probably be done nicer, but this was easier
        bonuses[0] = settings.account.calcBonusValue(bonus, settings.hero.health, 'Health');
        bonuses[1] = settings.account.calcBonusValue(bonus, settings.hero.attack, 'Attack');
        bonuses[2] = settings.account.calcBonusValue(bonus, settings.hero.defense, 'Defense');
        bonuses[3] = settings.account.calcBonusValue(bonus, settings.hero.speed, 'Speed');
        bonuses[4] = settings.account.calcBonusValue(bonus, 100, 'CriticalChance');
        bonuses[5] = settings.account.calcBonusValue(bonus, 100, 'CriticalDamage');
        bonuses[6] = settings.account.calcBonusValue(bonus, settings.hero.resistance, 'Resistance');
        bonuses[7] = settings.account.calcBonusValue(bonus, settings.hero.accuracy, 'Accuracy');
      }

      let baseScore = 0;
      for (let weight of settings.weights) {
        if (weight.kind === setKind) {
          baseScore = weight.value;
        }
      }

      let score = baseScore;
      for (let stat = 0; stat < numberOfStats; stat ++) score += bonuses[stat] * this.weights[stat];

      let maxScore = score;
      if (Raid.sets[setKind].setSize === 2) maxScore *= 3;

      this.maxSetScore = Math.max(this.maxSetScore, maxScore);

      // todo: allow additional score value per set and set this value in baseScore
      this.setScores[setKind] = {
        setKind: setKind,
        setSize: Raid.sets[setKind].setSize,
        bonuses: bonuses,
        baseScore: baseScore, 
        score: score,
        maxScore: maxScore,
      };
    }

    for (let i = 0; i < this.slots.length; i ++) {
      const slot = this.slots[i];
      const artifacts = this.filterBySlot(slot);

      const artifactsWithStats: IArtifactWithScore[] = [];

      for (const artifact of artifacts) {
        const bonuses = this.getBonuses(artifact);

        let score = 0;
        for (let stat = 0; stat < numberOfStats; stat ++) score += bonuses[stat] * this.weights[stat];

        let maxScore = score;
        if (artifact.setKind in this.setScores) {
          maxScore += this.setScores[artifact.setKind].maxScore;
        }

        artifactsWithStats.push({
          artifact: artifact,
          bonuses: bonuses,
          score: score,
          maxScore: maxScore
        });
      }

      if (artifactsWithStats.length === 0) {
        artifactsWithStats.push({
          bonuses: [0, 0, 0, 0, 0, 0, 0, 0],
          score: 0,
          maxScore: 0
        });
      }

      this.artifactsBySlot[i] = artifactsWithStats.sort((a, b) => b.maxScore - a.maxScore);
    }
  }

  private applyStatValues(values: StatValue[], array: number[]): void {
    if (!values || !values.length) {
      return;
    }

    for (let value of values) {
      if (!(value.kind in this.kindToIndexMap)) {
        continue;
      }
      array[this.kindToIndexMap[value.kind]] = value.value;
    }
  }

  private getBonuses(artifact: Artifact): number[] {
    const hero = this.settings.hero;
    const account = this.settings.account;

    const result: number[] = [];
    result.push(account.getBonusValue(artifact, hero.health, 'Health'));
    result.push(account.getBonusValue(artifact, hero.attack, 'Attack'));
    result.push(account.getBonusValue(artifact, hero.defense, 'Defense'));
    result.push(account.getBonusValue(artifact, hero.speed, 'Speed'));
    result.push(account.getBonusValue(artifact, 100, 'CriticalChance'));
    result.push(account.getBonusValue(artifact, 100, 'CriticalDamage'));
    result.push(account.getBonusValue(artifact, hero.resistance, 'Resistance'));
    result.push(account.getBonusValue(artifact, hero.accuracy, 'Accuracy'));
    return result;
  }


  private filterBySlot(slot: string): Artifact[] {
    return this.settings.artifacts.filter(a => a.kind === slot && (!a.requiredFraction || a.requiredFraction === this.settings.hero.fraction));
  }

  private slotIndexes = [0, 0, 0, 0, 0, 0, 0, 0, 0];
  private slots = ['Weapon', 'Helmet', 'Shield', 'Gloves', 'Chest', 'Boots', 'Ring', 'Cloak', 'Banner'];
  private artifactsBySlot: IArtifactWithScore[][] = [];
  private setScores: { [setKind: string ]: IArtifactSetScore } = {};
  private maxSetScore: number = 0;

  private kindToIndexMap: { [kind: string]: number } = {
    'Health': 0,
    'Attack': 1,
    'Defense': 2,
    'Speed': 3,
    'CriticalChance': 4,
    'CriticalDamage': 5,
    'Resistance': 6,
    'Accuracy': 7,
  };

  private timer: Subscription;
  private isCalculating: boolean;
  private isDone: boolean;

  private weights: number[] = [0,0,0,0,0,0,0,0];
  private minValues: number[] = [0,0,0,0,0,0,0,0];
  private softCaps: number[] = [-1,-1,-1,-1,-1,-1,-1,-1];
  private hardCaps: number[] = [-1,-1,-1,-1,-1,-1,-1,-1];

  possibilities: number;
  calculated: number = 0;
  iterationsPerSecond: number = 0;

  start() {
    if (this.isRunning) {
      return;
    }

    this.possibilities = 1;
    for (let i = 0; i < numberOfStats; i ++) {
      this.possibilities *= this.artifactsBySlot[i].length - this.slotIndexes[i];
    }
    console.log(`Brute forcing ${this.possibilities} possible artifact combinations...`);

    this.isRunning = true;
    this.timer = interval(5).subscribe(() => {
      if (this.isCalculating || this.isDone) {
        return;
      }
      this.isCalculating = true;
      try {
        this.calculate();
      } finally {
        this.isCalculating = false;
      }
    });
  }

  private calculate(): void {
    const startTime = new Date().getTime();
    for (let iteration = 0; iteration < this.iterations; iteration ++) {
      if (this.isDone) return;

      const artifacts: IArtifactWithScore[] = [];
      const bonuses = [0, 0, 0, 0, 0, 0, 0, 0];
      const sets: { [set: string]: number } = {};
      for (let i = 0; i < numberOfSlots; i ++) {
        const artifact = this.artifactsBySlot[i][this.slotIndexes[i]];
        artifacts.push(artifact);

        for (let b = 0; b < numberOfStats; b ++) {
          bonuses[b] += artifact.bonuses[b];
        }

        if (artifact.artifact) {
          sets[artifact.artifact.setKind] = (sets[artifact.artifact.setKind] || 0) + 1;
        }
      }

      let score = 0;
      for (let setKind in sets) {
        if (!sets.hasOwnProperty(setKind) || !(setKind in Raid.sets) || !(setKind in this.setScores)) {
          continue;
        }

        const setScore = this.setScores[setKind];
        while (sets[setKind] >= setScore.setSize) {
          sets[setKind] -= setScore.setSize;

          score += setScore.baseScore;
          for (let b = 0; b < numberOfStats; b ++) {
            bonuses[b] += setScore.bonuses[b];
          }
        }
      }

      let isValid = true;
      for (let b = 0; b < numberOfStats; b ++) { 
        if (bonuses[b] < this.minValues[b] || (this.hardCaps[b] !== -1 && bonuses[b] > this.hardCaps[b])) {
          isValid = false;
          break;
        }

        score += ((this.softCaps[b] !== -1 && bonuses[b] > this.softCaps[b]) ? this.softCaps[b] : bonuses[b]) * this.weights[b];
      }

      if (isValid) {
        this.recordCombination(artifacts, bonuses, score);
      }

      this.nextIteration();
    }

    console.log(`${this.iterations} took ${new Date().getTime() - startTime}ms, best score ${this.bestCombinations.length > 0 ? this.bestCombinations[0].score : 0}`);
    this.calculated += this.iterations;
    if (!this.iterationsPerSecond) {
      this.iterationsPerSecond = Math.round(this.iterations * (1000 / (new Date().getTime() - startTime)));
    }
    this.iterationsPerSecond = Math.round(((this.iterationsPerSecond * 19) + Math.round(this.iterations * (1000 / (new Date().getTime() - startTime)))) / 20);
    this.iterations = Math.round(this.iterations * (50 / (new Date().getTime() - startTime)));
  }

  maxCombinations = 10;
  bestCombinations: IArtifactCombination[] = [];

  private recordCombination(
    artifactsWithScore: IArtifactWithScore[],
    bonuses: number[],
    score: number): boolean {

    if (this.bestCombinations.length === this.maxCombinations) {
      const worstScore = this.bestCombinations[this.bestCombinations.length - 1].score;
      if (score < worstScore) {
        // If the score + maximum set score is lower than the worst score, there will be no way to improve the combinations
        return score + this.maxSetScore > worstScore;
      }
    }

    const artifacts: Artifact[] = [];
    for (let i = 0; i < numberOfSlots; i ++) {
      if (artifactsWithScore[i].artifact) {
        artifacts.push(artifactsWithScore[i].artifact);
      }
    }

    const combination: IArtifactCombination = {
      artifacts: artifacts,
      bonuses: bonuses,
      score: score,
    };

    for (let i = 0; i < this.bestCombinations.length; i ++) {
      if (score < this.bestCombinations[i].score) {
        continue;
      }

      this.bestCombinations.splice(i, 0, combination);
      if (this.bestCombinations.length > this.maxCombinations) {
        this.bestCombinations.length = this.maxCombinations;
      }
      return true;
    }

    if (this.bestCombinations.length < this.maxCombinations) {
      this.bestCombinations.push(combination);
    }
    return true;
  }

  private nextIteration() {
    for(let i = 0; i < numberOfSlots; i ++) {
      this.slotIndexes[i] ++;
      if (this.slotIndexes[i] < this.artifactsBySlot[i].length) return;
      this.slotIndexes[i] = 0;
    }

    this.isDone = true;
  }

  stop() {
    this.isRunning = false;

    if (this.timer) {
      this.timer.unsubscribe();
      this.timer = null;
    }
  }
}
