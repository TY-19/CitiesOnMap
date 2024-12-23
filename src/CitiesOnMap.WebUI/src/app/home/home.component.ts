import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {GameComponent} from "../game/game.component";

@Component({
  selector: 'citom-home',
  standalone: true,
  imports: [CommonModule, GameComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  constructor() {

  }

  ngOnInit(): void {

  }
}
