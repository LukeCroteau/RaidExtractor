import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { AccountComponent } from './account/account.component';
import { AccountService } from './shared/account.service';
import { ExpiredComponent } from './expired/expired.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import {MatTableModule} from '@angular/material/table';
import {MatTabsModule} from '@angular/material/tabs';
import {MatButtonModule} from '@angular/material/button';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    AccountComponent,
    ExpiredComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'expired', component: ExpiredComponent, pathMatch: 'full' },
      { path: 'account/:key', component: AccountComponent }
    ]),
    BrowserAnimationsModule,
    MatSidenavModule,
    MatToolbarModule,
    MatTabsModule,
    MatTableModule,
    MatButtonModule
  ],
  providers: [AccountService],
  bootstrap: [AppComponent]
})
export class AppModule { }
