import {Component, OnInit, ViewChild} from '@angular/core';
import {MapComponent} from "../map/map.component";
import {GameService} from "./game.service";
import {GameModel} from "../_models/game/gameModel";
import {AnswerModel} from "../_models/game/answerModel";
import {Coordinate} from "ol/coordinate";
import {AnswerResultModel} from "../_models/game/answerResultModel";
import {LocalStorageService} from "../_common/localStorage.service";
import {DecimalPipe} from "@angular/common";
import {RouterLink} from "@angular/router";
import {GameOptionsComponent} from "./game-options/game-options.component";
import {GameOptionsModel} from "../_models/game/gameOptionsModel";
import {SelectGameComponent} from "./select-game/select-game.component";

@Component({
  selector: 'citom-game',
  standalone: true,
  imports: [
    MapComponent,
    DecimalPipe,
    RouterLink,
    GameOptionsComponent,
    SelectGameComponent
  ],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss'
})
export class GameComponent implements OnInit {
  @ViewChild(MapComponent) map: MapComponent = null!;
  @ViewChild(GameOptionsComponent) optionsComponent?: GameOptionsComponent;
  selectedPoint?: Coordinate;
  displayMenuMode: "hidden" | "menu" | "options" | "games" = "hidden";
  optionsMenuMode: "create" | "update" = "create";
  private showQuestionInn: boolean = false;
  private showAnswerInn: boolean = false;
  get showQuestion() {
    return this.showQuestionInn;
  }
  set showQuestion(showQuestion: boolean) {
    this.showQuestionInn = showQuestion;
    this.showAnswerInn = !showQuestion;
  }
  get showAnswer() {
    return this.showAnswerInn;
  }
  set showAnswer(showAnswer: boolean) {
    this.showAnswerInn = showAnswer;
  }
  gameInn?: GameModel;
  get game(): GameModel | undefined {
    return this.gameInn;
  }
  set game(gameModel: GameModel) {
    this.gameInn = gameModel;
    this.points = gameModel.points;
  }
  answerResultInn?: AnswerResultModel;
  get answerResult(): AnswerResultModel | undefined {
    return this.answerResultInn;
  }
  set answerResult(answerResultModel) {
    this.answerResultInn = answerResultModel;
    this.points += answerResultModel?.points ?? 0;
  }
  points: number = 0;

  constructor(private gameService: GameService,
              private localstorageService: LocalStorageService) {

  }
  ngOnInit(): void {
    const gameId: string | null = this.localstorageService.gameId;
    const playerId: string | null = this.localstorageService.playerId;
    if(gameId && playerId) {
      this.gameService.getGame(gameId, playerId)
        .subscribe((r: GameModel) => {
          if(r) {
            this.game = r;
            this.showQuestion = this.game.currentCityName !== null;
          } else {
            this.displayMenuMode = "menu";
          }
        });
    } else {
      this.displayMenuMode = "menu";
    }
  }

  startGame(): void {
    let gameOptions: GameOptionsModel | null = null;
    if(this.optionsComponent) {
      gameOptions = this.optionsComponent.readOptionsFromForm();
    }
    const playerId: string | null = this.localstorageService.playerId;
    this.gameService.startGame(playerId, gameOptions)
      .subscribe((r: GameModel) => {
        this.game = r;
        this.setParameters(this.game);
        this.nextQuestion();
      });
    this.displayMenuMode = "hidden";
  }
  nextQuestion(): void {
    let gameId: string | null;
    if(this.game) {
      gameId = this.game.id;
    } else {
      gameId = this.localstorageService.gameId;
    }

    if(gameId) {
      this.showQuestion = false;
      this.showAnswer = false;
      this.gameService.getNextQuestion(gameId)
        .subscribe(r => {
          this.game = r;
          this.map.clearPoints();
          this.selectedPoint = undefined;
          this.showQuestion = true;
        });
    }
  }
  selectPoint(coordinate: Coordinate) {
    this.selectedPoint = coordinate;
  }
  checkAnswer(): void {
    if(this.game === undefined || this.selectedPoint === undefined) {
      return;
    }
    const answer: AnswerModel = {
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
  switchGame(game: GameModel) {
    this.game = game;
    this.setParameters(this.game);
    this.map.clearPoints();
    if(this.game.currentCityName) {
      this.showQuestion = true;
    } else {
      this.nextQuestion();
    }
    this.displayMenuMode = "hidden";
  }
  showOptions(mode: "create" | "update") {
    this.optionsMenuMode = mode;
    if(this.displayMenuMode === "options") {
      this.displayMenuMode = "hidden"
    } else {
      this.displayMenuMode = "options";
    }
  }
  updateOptions(): void {
    if(!this.optionsComponent || !this.game) {
      return;
    }
    let gameOptions: GameOptionsModel = this.optionsComponent.readOptionsFromForm();
    const playerId: string | null = this.localstorageService.playerId;
    this.gameService.updateGameOptions(playerId, this.game?.id, gameOptions)
      .subscribe((r: GameModel) => {
        this.game = r;
        this.setParameters(this.game);
      });
    this.displayMenuMode = "hidden";
  }
  changeMenuDisplay() {
    if(this.displayMenuMode !== "hidden") {
      this.displayMenuMode = "hidden";
    } else {
      this.displayMenuMode = "menu";
    }
  }
  private setParameters(game: GameModel) {
    this.localstorageService.playerId = game.playerId;
    this.localstorageService.gameId = game.id;
  }
}
