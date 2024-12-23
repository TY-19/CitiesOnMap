import {Component} from '@angular/core';
import {MapComponent} from "../map/map.component";
import {GameService} from "./game.service";
import {GameModel} from "../_models/gameModel";
import {AnswerModel} from "../_models/answerModel";
import {Coordinate} from "ol/coordinate";
import {AnswerResultModel} from "../_models/answerResultModel";

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
  constructor(private gameService: GameService) {

  }
  selectedPoint?: Coordinate;
  showQuestion: boolean = false;
  showAnswer: boolean = true;
  game?: GameModel;
  answerResult?: AnswerResultModel;
  startGame(): void {
    this.gameService.startGame("test-1234")
      .subscribe(r => {
        this.game = r;
        this.nextQuestion();
      });
  }
  nextQuestion(): void {
    if(this.game) {
      this.showAnswer = false;
      this.gameService.getNextQuestion(this.game.id)
        .subscribe(r => {
          this.game = r;
          this.showQuestion = true;
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
      });
  }
}
