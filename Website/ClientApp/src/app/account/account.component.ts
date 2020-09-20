import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { AccountService } from '../shared/account.service';
import { RaidAccount } from '../shared/raid-account';
import { Hero } from '../shared/clients';
import { MatDialog } from '@angular/material/dialog';
import { HeroDialogComponent } from '../hero-dialog/hero-dialog.component';
import { ArtifactDialogComponent } from '../artifact-dialog/artifact-dialog.component';
import { Artifact } from '../shared/clients';
import { IHero } from '../shared/clients';

@Component({
  selector: 'account-dump',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private accountService: AccountService,
    private dialog: MatDialog
  ) {
  }

  account: RaidAccount;

  hero: Hero;
  artifactByKind: { [kind: string]: Artifact } = { };

  ngOnInit(): void {
    this.route.params.subscribe((params: ParamMap) => {
      if ('key' in params) {
        this.accountService.get(params['key']).subscribe(dump => {
          if (!dump) {
            this.router.navigate(['/expired']);
            return;
          }

          this.account = dump;
        });
      } else {
        this.router.navigate(['/']);
      }
    });
  }

  selectHero() {
    const dialogRef = this.dialog.open(HeroDialogComponent,
      {
        data: this.account,
        width: '800px'
      });
    dialogRef.afterClosed().subscribe(hero => {
      if (!hero) {
        return;
      }

      this.hero = new Hero(<IHero>{ ...hero });
      this.hero.artifacts = [];
      this.artifactByKind = {};
    });
  }

  selectArtifact(kind: string) {
    const dialogRef = this.dialog.open(ArtifactDialogComponent,
      {
        data: {
          account: this.account,
          hero: this.hero,
          artifacts: this.account.artifactsByKind[kind],
        },
        width: '800px'
      });
    dialogRef.afterClosed().subscribe(artifact => {
      const artifactByKind = this.artifactByKind;
      this.artifactByKind[kind] = artifact || artifactByKind[kind];

      this.hero.artifacts = [];
      for (let k in artifactByKind) {
        if (Object.prototype.hasOwnProperty.call(artifactByKind, k)) {
          const artifact = this.artifactByKind[k];
          if (!artifact) {
            continue;
          }

          this.hero.artifacts.push(artifact.id);
        }
      }
    });
  }

  //test() {
  //  const hero = this.account.heroes[0];
  //  const weights = [0.01,0.5,0.5,1,0,10,1,1];

  //  const artifactsByKind = {};
  //  for (let kind in this.artifactsByKind) {
  //    const arr: { artifact: Artifact, bonuses: number[], score: number }[] = [];
  //    for (let artifact of this.artifactsByKind[kind]) {
  //      if (artifact.requiredFraction && artifact.requiredFraction !== hero.fraction) {
  //        continue;
  //      }

  //      const bonuses = this.getBonuses(hero, artifact);
  //      let score = 0;
  //      for(let i = 0; i < bonuses.length; i ++) score += bonuses[i] * weights[i];
  //      arr.push({
  //        artifact: artifact,
  //        bonuses: bonuses,
  //        score: score
  //      });
  //    }
  //    if (arr.length === 0) {
  //      arr.push({
  //        artifact: null,
  //        bonuses: [0, 0, 0, 0, 0, 0, 0, 0],
  //        score: 0
  //      });
  //    }
  //    artifactsByKind[kind] = arr.sort((a, b) => b.score - a.score);
  //  }

  //  let bestSetScore = 0;
  //  const setByKind = {};
  //  for (let set in this.setMap) {
  //    // TODO: Add set base-score to 'stear' towards a certain set
  //    setByKind[set] = {
  //      'set': this.setMap[set].set,
  //      'bonuses': [0,0,0,0,0,0,0,0],
  //      'score': 0,
  //      'maxScore': 0
  //    };
  //    for (const bonus of this.setMap[set].bonuses) {
  //      setByKind[set].bonuses[0] += this.calcBonusValue(bonus, hero.health, 'Health');
  //      setByKind[set].bonuses[1] += this.calcBonusValue(bonus, hero.attack, 'Attack');
  //      setByKind[set].bonuses[2] += this.calcBonusValue(bonus, hero.defense, 'Defense');
  //      setByKind[set].bonuses[3] += this.calcBonusValue(bonus, hero.accuracy, 'Accuracy');
  //      setByKind[set].bonuses[4] += this.calcBonusValue(bonus, hero.resistance, 'Resistance');
  //      setByKind[set].bonuses[5] += this.calcBonusValue(bonus, hero.speed, 'Speed');
  //      setByKind[set].bonuses[6] += this.calcBonusValue(bonus, 100, 'CriticalChance');
  //      setByKind[set].bonuses[7] += this.calcBonusValue(bonus, 100, 'CriticalDamage');
  //    }
  //    for(let i = 0; i < setByKind[set].bonuses.length; i ++) setByKind[set].score += setByKind[set].bonuses[i] * weights[i];
  //    setByKind[set].maxScore = setByKind[set].score * (setByKind[set].set === 2 ? 3 : 1);

  //    bestSetScore = Math.max(bestSetScore, setByKind[set].score);
  //  }

  //  let bestBonuses = [];
  //  let bestScore = 0;
  //  let bestArtifacts = [];
  //  for (let weapon of artifactsByKind['Weapon']) {
  //    // If the maximum set-score is lower than the difference between this and the previous artifact, then no point in continue'ing to look at this artifact
  //    if (artifactsByKind['Weapon'][0].score - weapon.score > bestSetScore) break;
  //    for (let helmet of artifactsByKind['Helmet']) {
  //      if (artifactsByKind['Helmet'][0].score - helmet.score > bestSetScore) break;
  //      for (let shield of artifactsByKind['Shield']) {
  //        if (artifactsByKind['Shield'][0].score - shield.score > bestSetScore) break;
  //        for (let gloves of artifactsByKind['Gloves']) {
  //          if (artifactsByKind['Gloves'][0].score - gloves.score > bestSetScore) break;
  //          for (let chest of artifactsByKind['Chest']) {
  //            if (artifactsByKind['Chest'][0].score - chest.score > bestSetScore) break;
  //            for (let boots of artifactsByKind['Boots']) {
  //              if (artifactsByKind['Boots'][0].score - boots.score > bestSetScore) break;
  //              const sets = {};
  //              this.addSet(sets, weapon);
  //              this.addSet(sets, helmet);
  //              this.addSet(sets, shield);
  //              this.addSet(sets, gloves);
  //              this.addSet(sets, chest);
  //              this.addSet(sets, boots);
  //              let setBonuses = [0,0,0,0,0,0,0,0];
  //              let setScore = 0;
  //              for (let setKind in sets) {
  //                while (sets[setKind] > setByKind[setKind]) {
  //                  sets[setKind] -= setByKind[setKind].set;
  //                  setScore += setByKind[setKind].score;
  //                }
  //              }

  //              for (let ring of artifactsByKind['Ring']) { 
  //                for (let cloak of artifactsByKind['Cloak']) {
  //                  for (let banner of artifactsByKind['Banner']) {
  //                    const bonuses = [hero.health,hero.attack,hero.defense,hero.accuracy,hero.resistance,hero.speed,hero.criticalChance,hero.criticalDamage];
  //                    let score = 0;
  //                    for (let i = 0; i < bonuses.length; i++) {
  //                      bonuses[i] += weapon.bonuses[i] + helmet.bonuses[i] + shield.bonuses[i] + gloves.bonuses[i] + chest.bonuses[i] + boots.bonuses[i] + ring.bonuses[i] + cloak.bonuses[i] + banner.bonuses[i] + setBonuses[i];
  //                      // HACK: CriticalChance = 6 - soft cap
  //                      if (i == 6 && bonuses[i] > 100) bonuses[i] = 100;
  //                      score += bonuses[i] * weights[i];
  //                    }

  //                    if (score < bestScore) continue;

  //                    bestScore = score;
  //                    bestBonuses = bonuses;
  //                    bestArtifacts = [helmet, weapon, shield, gloves, chest, boots, ring, cloak, banner];
  //                  }
  //                }
  //              }
  //            }
  //          }
  //        }
  //      }
  //    }
  //  }

  //  console.log('bestBonuses', bestBonuses);
  //  console.log('bestScore', bestScore);
  //  console.log('bestArtifacts', bestArtifacts);
  //}

  //addSet(sets: any, artifact: Artifact): any {
  //  if (artifact.setKind in sets) {
  //    sets[artifact.setKind]++;
  //  } else {
  //    sets[artifact.setKind] = 1;
  //  }
  //  return sets;
  //}

  //getBonuses(hero: Hero, artifact: Artifact): number[] {
  //  const result: number[] = [];
  //  result.push(this.getBonusValue(artifact, hero.health, 'Health'));
  //  result.push(this.getBonusValue(artifact, hero.attack, 'Attack'));
  //  result.push(this.getBonusValue(artifact, hero.defense, 'Defense'));
  //  result.push(this.getBonusValue(artifact, hero.accuracy, 'Accuracy'));
  //  result.push(this.getBonusValue(artifact, hero.resistance, 'Resistance'));
  //  result.push(this.getBonusValue(artifact, hero.speed, 'Speed'));
  //  result.push(this.getBonusValue(artifact, 100, 'CriticalChance'));
  //  result.push(this.getBonusValue(artifact, 100, 'CriticalDamage'));
  //  return result;
  //}
}
