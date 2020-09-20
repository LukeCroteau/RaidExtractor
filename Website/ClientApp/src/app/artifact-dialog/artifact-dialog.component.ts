import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Hero } from '../shared/clients';
import { HeroDialogComponent } from '../hero-dialog/hero-dialog.component';
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

  constructor(
    private dialogRef: MatDialogRef<HeroDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private data: {
      account: RaidAccount,
      artifacts: Artifact[],
      hero: Hero,
    }
  ) {
    this.account = data.account;
    this.artifacts = data.artifacts.sort((a, b) => {
      let c = a.setKind.localeCompare(b.setKind);
      if (c !== 0) return c;
      c = a.primaryBonus.kind.localeCompare(b.primaryBonus.kind);
      if (c !== 0) return c;
      c = (a.primaryBonus.isAbsolute ? 1 : 0) - (b.primaryBonus.isAbsolute ? 1 : 0);
      if (c !== 0) return c;
      c = a.primaryBonus.value - b.primaryBonus.value;
      if (c !== 0) return c;
      c = a.rank.localeCompare(b.rank);
      if (c !== 0) return c;
      c = a.rarity.localeCompare(b.rarity);
      return c;
    });
  }

  select(artifact: Artifact) {
    this.dialogRef.close(artifact);
  }
}
