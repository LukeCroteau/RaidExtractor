import { Artifact } from './clients';
import { Hero } from './clients';
import { StatValue } from './stat-value';

export class ArtifactOptimizerSettings {
  constructor(
    public hero: Hero,
    public artifacts: Artifact[],
    public weights: StatValue[],
    public softCap?: StatValue[],
    public hardCap?: StatValue[]
  ) {
    this.softCap = softCap || [new StatValue('CriticalChance', 100)];
    this.hardCap = hardCap || [];
  }
}
