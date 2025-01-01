import {AnswerModel} from "./answerModel";
import {City} from "./city";

export interface AnswerResultModel {
  city: City;
  answer: AnswerModel;
  distance: number;
  points: number;
}
