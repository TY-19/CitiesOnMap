import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameComponent } from "../game/game.component";
import { AuthComponent } from '../auth/auth.component'
import { RouterLink } from '@angular/router';


@Component({
  selector: 'citom-home',
  standalone: true,
  imports: [CommonModule, GameComponent, AuthComponent, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  showSignIn: boolean = false;
  constructor() {

  }

  ngOnInit(): void {

  }
}
