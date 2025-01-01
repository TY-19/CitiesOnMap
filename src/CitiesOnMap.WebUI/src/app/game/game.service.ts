import {HttpClient, HttpParams} from "@angular/common/http";
import {Injectable} from "@angular/core";
import {baseUrl} from "../app.config";
import {GameModel} from "../_models/game/gameModel";
import {Observable} from "rxjs";
import {AnswerResultModel} from "../_models/game/answerResultModel";
import {AnswerModel} from "../_models/game/answerModel";

@Injectable({
  providedIn: 'root',
})

export class GameService {
  constructor(private http: HttpClient) {

  }
  startGame(playerId?: string): Observable<GameModel> {
    let url = baseUrl + "/game/start";
    let params = new HttpParams();
    if(playerId) {
      params.set("playerId", playerId);
    }
    return this.http.post<GameModel>(url, null, { params });
  }
  getNextQuestion(gameId: string): Observable<GameModel> {
    let url = baseUrl + "/game/next-question";
    let params = new HttpParams().set("gameId", gameId);
    return this.http.get<GameModel>(url, { params });
  }
  sendAnswer(answer: AnswerModel): Observable<AnswerResultModel> {
    let url = baseUrl + "/game/answer";
    return this.http.post<AnswerResultModel>(url, answer);
  }
}
