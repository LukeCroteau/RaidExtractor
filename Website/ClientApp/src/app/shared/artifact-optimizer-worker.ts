import { Subscription, interval } from 'rxjs';
import { ArtifactOptimizerSettings } from './artifact-optimizer-settings';
import { StatValue } from './stat-value';

export class ArtifactOptimizerWorker {
  constructor(settings: ArtifactOptimizerSettings) {
    function applyStatValues(values: StatValue[], array: number[]): void {
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

    applyStatValues(settings.weights, this.weights);
    applyStatValues(settings.softCap, this.softCaps);
    applyStatValues(settings.hardCap, this.hardCaps);
  }

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

  private weights: number[] = [1,1,1,1,1,1,1,1];
  private softCaps: number[] = [0,0,0,0,0,0,0,0];
  private hardCaps: number[] = [0,0,0,0,0,0,0,0];

  weaponIndex = 0;
  helmetIndex = 0;
  shieldIndex = 0;
  glovesIndex = 0;
  chestIndex = 0;
  bootsIndex = 0;
  ringIndex = 0;
  cloakIndex = 0;
  bannerIndex = 0;

  start() {
    this.timer = interval(0).subscribe(_ => {
      if (this.isCalculating) {
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

  }

  stop() {
    this.timer.unsubscribe();
  }
}
