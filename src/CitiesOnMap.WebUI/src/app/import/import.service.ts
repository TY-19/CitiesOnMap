import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {baseUrl} from "../app.config";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root',
})

export class ImportService {
  constructor(private http: HttpClient) {

  }
  importCountries(file: File): Observable<any> {
    const url: string = baseUrl + "/import/countries"
    const formData = new FormData();
    formData.append('csvFile', file);
    return this.http.post<object>(url, formData);
  }
  importCities(file: File): Observable<any> {
    const url: string = baseUrl + "/import/cities";
    const formData = new FormData();
    formData.append('csvFile', file);
    return this.http.post<object>(url, formData);
  }
}
