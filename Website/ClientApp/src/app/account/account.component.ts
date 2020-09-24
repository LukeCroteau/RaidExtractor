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
import { ArtifactOptimizerWorker } from '../shared/artifact-optimizer-worker';
import { OptimizeDialogComponent } from '../optimize-dialog/optimize-dialog.component';
import { IArtifactCombination } from '../shared/artifact-optimizer-worker';

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

  activeTab: number = 0;

  account: RaidAccount;
  heroes: Hero[] = [];

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
          this.heroes = dump.heroes.sort((a, b) => {
            let c = a.name.localeCompare(b.name);
            if (c !== 0) return c;
            c = b.grade.localeCompare(a.grade);
            if (c !== 0) return c;
            c = b.awakenLevel - a.awakenLevel;
            if (c !== 0) return c;
            return b.level - a.level;
          });
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

  optimizer: ArtifactOptimizerWorker;

  pauseOptimizer() {
    if (this.optimizer) {
      this.optimizer.stop();
    }
  }

  resumeOptimizer() {
    if (this.optimizer) {
      this.optimizer.start();
    }
  }

  clearOptimizer() {
    if (this.optimizer) {
      this.optimizer.stop();
      this.optimizer = null;
    }
  }

  optimizeHero() {
    const dialogRef = this.dialog.open(OptimizeDialogComponent,
      {
        data: {
          account: this.account,
          hero: this.hero
        },
        width: '800px'
      });
    dialogRef.afterClosed().subscribe(settings => {
      if (!settings) {
        return;
      }

      this.optimizer = new ArtifactOptimizerWorker(settings);
      this.optimizer.start();
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

  selectCombination(combination: IArtifactCombination) {
    this.artifactByKind = {};
    this.hero.artifacts = [];
    for (let artifact of combination.artifacts) {
      this.artifactByKind[artifact.kind] = this.account.artifactsById[artifact.id];
      this.hero.artifacts.push(artifact.id);
    }
  }
}
