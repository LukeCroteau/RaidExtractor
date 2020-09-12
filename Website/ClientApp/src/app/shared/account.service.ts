import { Injectable } from '@angular/core';
import { AccountDump } from './clients';
import { Observable } from 'rxjs';
import { AccountClient } from './clients';
import { map } from 'rxjs/operators'

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private client: AccountClient)
  {}

  get(key: string): Observable<AccountDump> {
    const cached = localStorage.getItem(key);
    if (cached) {
      return Observable.create((observer:any) => {
        observer.next(JSON.parse(cached));
        observer.complete();
      });
    }

    return this.client.get(key).pipe(map(account => {
      localStorage.setItem(key, JSON.stringify(account));
      return account;
    }));
  }
}
