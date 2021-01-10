import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { OptimizeDialogStore, OptimizeDialogState } from './optimize-dialog.store';

@Injectable({ providedIn: 'root' })
export class OptimizeDialogQuery extends Query<OptimizeDialogState> {

  constructor(protected store: OptimizeDialogStore) {
    super(store);
  }

}
