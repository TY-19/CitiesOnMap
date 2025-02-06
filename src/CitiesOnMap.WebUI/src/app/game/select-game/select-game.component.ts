import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {GameService} from "../game.service";
import {LocalStorageService} from "../../_common/localStorage.service";
import {GameModel} from "../../_models/game/gameModel";
import {DatePipe, DecimalPipe} from "@angular/common";

@Component({
  selector: 'citom-select-game',
  standalone: true,
  imports: [
    DecimalPipe,
    DatePipe
  ],
  templateUrl: './select-game.component.html',
  styleUrl: './select-game.component.scss'
})
export class SelectGameComponent implements OnInit {
  @Output() selectGameEvent: EventEmitter<GameModel> = new EventEmitter<GameModel>();
  games: GameModel[] = [];
  constructor(private gameService: GameService,
              private localStorageService: LocalStorageService) {
  }
  ngOnInit() {
    let playerId = this.localStorageService.playerId;
    this.gameService.getPlayerGames(playerId)
      .subscribe(g => {
        this.games = g;
      });
  }
  selectGame(game: GameModel) {
    this.selectGameEvent.emit(game);
  }
}
