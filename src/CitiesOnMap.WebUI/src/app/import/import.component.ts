import { Component } from '@angular/core';
import {ImportService} from "./import.service";

@Component({
  selector: 'citom-import',
  standalone: true,
  imports: [],
  templateUrl: './import.component.html',
  styleUrl: './import.component.scss'
})
export class ImportComponent {
  selectedFile: File | null = null;
  selectedFileName?: string;
  isProcessingFile: boolean = false;
  countriesImportResponse: string | null = null;
  citiesImportResponse: string | null = null;

  constructor(private importService: ImportService) {

  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0 && !this.isProcessingFile) {
      if(!input.files[0].name.endsWith(".csv")) {
        this.selectedFileName = "Invalid CSV file.";
        return;
      }
      this.selectedFile = input.files[0];
      this.selectedFileName = this.selectedFile?.name;
    }
  }
  importCountries() {
    if(!this.selectedFile) {
      return;
    }
    this.isProcessingFile = true;
    this.importService.importCountries(this.selectedFile)
      .subscribe(r => {
        this.countriesImportResponse = r.result;
        this.isProcessingFile = false;
      });
  }
  importCities() {
    if(!this.selectedFile) {
      return;
    }
    this.importService.importCities(this.selectedFile)
      .subscribe(r => {
        this.citiesImportResponse = r.result;
        this.isProcessingFile = false;
      });
  }
}
