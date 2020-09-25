import { Component } from '@angular/core';
import { versionInfo } from './shared/version-info';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'Raid: Shadow Legends - Artifact Optimizer';
  versionInfo = versionInfo;
}
