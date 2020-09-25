import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Hero } from '../shared/clients';
import { Artifact } from '../shared/clients';
import { RaidAccount } from '../shared/raid-account';
import { Raid } from '../shared/raid';

@Component({
  selector: 'app-artifact-dialog',
  templateUrl: './artifact-dialog.component.html'
})
export class ArtifactDialogComponent {
  artifacts: Artifact[];
  account: RaidAccount;
  equippedArtifacts: number[] = [];
  sets: { name: string, setKind: string }[] = [];

  filter = 'all';
  filterSet = '';
  orderBy = 'default';

  constructor(
    private dialogRef: MatDialogRef<ArtifactDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private data: {
      account: RaidAccount,
      artifacts: Artifact[],
      hero: Hero,
    }
  ) {
    this.account = data.account;
    for (let h of this.account.heroes) {
      if (!h.artifacts || h.id === data.hero.id) {
        continue;
      }

      for (let id of h.artifacts) {
        this.equippedArtifacts.push(id);
      }
    }

    for (let setKind in Raid.sets) {
      if (!Raid.sets.hasOwnProperty(setKind)) {
        continue;
      }
      this.sets.push({ name: Raid.sets[setKind].name, setKind: setKind });
    }

    this.refreshArtifacts();
  }

  refreshArtifacts() {
    let baseValue = 100;
    if (this.orderBy !== 'default' && !this.orderBy.startsWith('Crit')) {
      baseValue = this.data.hero[Raid.statProperty[this.orderBy]];
    }

    this.artifacts = this.data.artifacts.sort((a, b) => {
      if (this.orderBy !== 'default') {
        const statA = this.account.getBonusValue(a, baseValue, this.orderBy);
        const statB = this.account.getBonusValue(b, baseValue, this.orderBy);
        const stat = statB - statA;
        if (stat !== 0) {
          return stat;
        }
      }

      let c = a.setKind.localeCompare(b.setKind);
      if (c !== 0) return c;
      c = a.primaryBonus.kind.localeCompare(b.primaryBonus.kind);
      if (c !== 0) return c;
      c = (a.primaryBonus.isAbsolute ? 1 : 0) - (b.primaryBonus.isAbsolute ? 1 : 0);
      if (c !== 0) return c;
      c = b.primaryBonus.value - a.primaryBonus.value;
      if (c !== 0) return c;
      c = a.rank.localeCompare(b.rank);
      if (c !== 0) return c;
      c = a.rarity.localeCompare(b.rarity);
      return c;
    }).filter(a => {
      if (this.filter === 'not-equipped' && this.equippedArtifacts.indexOf(a.id) >= 0) return false;
      if (this.filterSet && a.setKind !== this.filterSet) return false;
      if (!a.requiredFraction) return true;
      return a.requiredFraction === this.data.hero.fraction;
    });

  }

  select(artifact: Artifact) {
    this.dialogRef.close(artifact);
  }
}
