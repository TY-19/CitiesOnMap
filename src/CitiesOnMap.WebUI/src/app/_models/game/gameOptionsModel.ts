export interface GameOptionsModel {
  showCountry: boolean;
  showPopulation: boolean;
  capitalsWithPopulationOver: number;
  citiesWithPopulationOver: number;
  distanceUnit: 'km' | 'mi';
  maxPointForAnswer: number;
  reducePointsPerUnit: number;
  allowNegativePoints: boolean;
}
