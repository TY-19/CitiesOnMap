import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameComponent } from "../game/game.component";
import { AuthComponent } from '../auth/auth.component';

@Component({
  selector: 'citom-home',
  standalone: true,
  imports: [CommonModule, GameComponent, AuthComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  constructor() {

  }

  ngOnInit(): void {

  }
}
