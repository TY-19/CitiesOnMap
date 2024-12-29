import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MapComponent } from './map/map.component';
import { CallbackComponent } from './auth/callback/callback.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', component: HomeComponent },
  { path: 'map', component: MapComponent },
  { path: 'callback', component: CallbackComponent },
];
