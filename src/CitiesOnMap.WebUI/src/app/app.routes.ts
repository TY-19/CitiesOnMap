import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MapComponent } from './map/map.component';
import { CallbackComponent } from './auth/callback/callback.component';
import {GameComponent} from "./game/game.component";
import {AttributionsComponent} from "./attributions/attributions.component";

export const routes: Routes = [
  { path: '', pathMatch: 'full', component: HomeComponent },
  { path: 'game', component: GameComponent },
  { path: 'map', component: MapComponent },
  { path: 'callback', component: CallbackComponent },
  { path: 'attributions', component: AttributionsComponent }
];
