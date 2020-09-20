import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Hero } from '../shared/clients';
import { RaidAccount } from '../shared/raid-account';

@Component({
  selector: 'app-hero-dialog',
  templateUrl: './hero-dialog.component.html'
})
export class HeroDialogComponent {

  constructor(
    private dialogRef: MatDialogRef<HeroDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public account: RaidAccount
  ) {
  }

  select(hero: Hero) {
    this.dialogRef.close(hero);
  }
}
