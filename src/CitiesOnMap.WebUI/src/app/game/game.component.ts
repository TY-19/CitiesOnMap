import {Component, ViewChild} from '@angular/core';
import {MapComponent} from "../map/map.component";
import {GameService} from "./game.service";
import {GameModel} from "../_models/game/gameModel";
import {AnswerModel} from "../_models/game/answerModel";
import {Coordinate} from "ol/coordinate";
import {AnswerResultModel} from "../_models/game/answerResultModel";
import {LocalStorageService} from "../_common/localStorage.service";

@Component({
  selector: 'citom-game',
  standalone: true,
    imports: [
        MapComponent
    ],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss'
})
export class GameComponent {
  @ViewChild(MapComponent) map: MapComponent = null!;
  selectedPoint?: Coordinate;
  showQuestion: boolean = false;
  showAnswer: boolean = true;
  game?: GameModel;
  answerResult?: AnswerResultModel;

  constructor(private gameService: GameService,
              private localstorageService: LocalStorageService) {

  }

  startGame(): void {
    const playerId: string | null = this.localstorageService.playerId;
    this.gameService.startGame(playerId)
      .subscribe((r: GameModel) => {
        this.game = r;
        this.setParameters(this.game);
        this.nextQuestion();
      });
  }
  nextQuestion(): void {
    let gameId: string | null;
    if(this.game) {
      gameId = this.game.id;
    } else {
      gameId = this.localstorageService.gameId;
    }

    if(gameId) {
      this.showAnswer = false;
      this.gameService.getNextQuestion(gameId)
        .subscribe(r => {
          this.game = r;
          this.showQuestion = true;
          this.map.clearPoints();
        });
    }
  }
  setCoordinate(coordinate: Coordinate) {
    this.selectedPoint = coordinate;
  }
  checkAnswer(): void {
    if(this.game === undefined || this.selectedPoint === undefined) {
      return;
    }
    let answer: AnswerModel = {
      gameId: this.game.id,
      selectedLongitude: this.selectedPoint[0],
      selectedLatitude: this.selectedPoint[1]
    };
    this.gameService.sendAnswer(answer)
      .subscribe(r => {
        this.answerResult = r;
        this.showAnswer = true;
        this.map.displayAnswerPoint([r.city.longitude, r.city.latitude]);
      });
  }
  private setParameters(game: GameModel) {
    this.localstorageService.playerId = game.playerId;
    this.localstorageService.gameId = game.id;
  }
}
