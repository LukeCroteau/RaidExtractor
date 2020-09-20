import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountClient } from './clients';
import { map } from 'rxjs/operators'
import { RaidAccount } from './raid-account';
import { AccountDump } from './clients';
import { IAccountDump } from './clients';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private client: AccountClient)
  {}

  get(key: string): Observable<RaidAccount> {
    const cached = localStorage.getItem(key);
    if (cached) {
      return Observable.create((observer:any) => {
        observer.next(new RaidAccount(new AccountDump(<IAccountDump>JSON.parse(cached))));
        observer.complete();
      });
    }

    return this.client.get(key).pipe(map(account => {
      localStorage.setItem(key, JSON.stringify(account));
      return new RaidAccount(account);
    }));
  }
}
