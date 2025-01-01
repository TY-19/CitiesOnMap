import {Country} from "./country";
import {CapitalType} from "../../_enums/capitalType";

export interface City {
  id: string;
  name: string;
  nameAscii: string;
  latitude: number;
  longitude: number;
  countryId: number;
  country: Country;
  administrationName: string;
  capitalType: CapitalType;
  population: number;
}
