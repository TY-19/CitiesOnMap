import {GameOptionsModel} from "./gameOptionsModel";

export interface GameModel {
  id: string;
  playerId: string;
  points: number;
  currentCityName: string | null;
  cityPopulation: number | null;
  country: string | null;
  lastPlayTime: Date;
  options: GameOptionsModel;
}
