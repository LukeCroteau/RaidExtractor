import { Injectable } from '@angular/core';
import { Store, StoreConfig } from '@datorama/akita';
import { OptimizeDialogSettings } from './optimize-dialog.settings';

export interface OptimizeDialogState {
  optimizeDialogSettings: OptimizeDialogSettings
}

export function createInitialState(): OptimizeDialogState {
  return {
    optimizeDialogSettings: {
      filter: 'all',
      maxToSixteen: false,
      preferredSet: '',

      minimumHealth: 0,
      maximumHealth: 0,
      capHealth: 0,
      weightHealth: 0,

      minimumAttack: 0,
      maximumAttack: 0,
      capAttack: 0,
      weightAttack: 0,

      minimumDefense: 0,
      maximumDefense: 0,
      capDefense: 0,
      weightDefense: 0,

      minimumSpeed: 0,
      maximumSpeed: 0,
      capSpeed: 0,
      weightSpeed: 0,

      minimumCriticalChance: 0,
      maximumCriticalChance: 0,
      capCriticalChance: 100,
      weightCriticalChance: 0,

      minimumCriticalDamage: 0,
      maximumCriticalDamage: 0,
      capCriticalDamage: 0,
      weightCriticalDamage: 0,

      minimumResistance: 0,
      maximumResistance: 0,
      capResistance: 0,
      weightResistance: 0,

      minimumAccuracy: 0,
      maximumAccuracy: 0,
      capAccuracy: 250,
      weightAccuracy: 0,

      optimizeWeapon: true,
      optimizeHelmet: true,
      optimizeShield: true,
      optimizeGloves: true,
      optimizeChest: true,
      optimizeBoots: true,
      optimizeRing: true,
      optimizeCloak: true,
      optimizeBanner: true,
    }
  };
}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'optimize-dialog' })
export class OptimizeDialogStore extends Store<OptimizeDialogState> {

  constructor() {
    super(createInitialState());
  }

}

