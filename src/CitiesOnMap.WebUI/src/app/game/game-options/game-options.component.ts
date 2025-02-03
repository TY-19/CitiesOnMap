import {Component, Input, OnInit} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {GameOptionsModel} from "../../_models/game/gameOptionsModel";

@Component({
  selector: 'citom-game-options',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './game-options.component.html',
  styleUrl: './game-options.component.scss'
})
export class GameOptionsComponent implements OnInit {
  @Input() options?: GameOptionsModel;
  form!: FormGroup;
  ngOnInit() {
    if(!this.options) {
      this.options = this.getDefaultOptions();
    }
    this.initiateForm();
  }
  private getDefaultOptions(): GameOptionsModel {
    return {
      showCountry: false,
      showPopulation: true,
      capitalsWithPopulationOver: 0,
      citiesWithPopulationOver: 5000000,
      distanceUnit: "km",
      maxPointForAnswer: 5000,
      reducePointsPerUnit: 1,
      allowNegativePoints: false,
    };
  }
  private initiateForm() {
    this.form = new FormGroup({
      showCountry: new FormControl(this.options?.showCountry, [Validators.required]),
      showPopulation: new FormControl(this.options?.showCountry, [Validators.required]),
      capitalsWithPopulationOver: new FormControl(this.options?.capitalsWithPopulationOver, [Validators.required, Validators.min(0)]),
      citiesWithPopulationOver: new FormControl(this.options?.citiesWithPopulationOver, [Validators.required, Validators.min(0)]),
      distanceUnit: new FormControl(this.options?.distanceUnit, [Validators.required]),
      maxPointForAnswer: new FormControl(this.options?.maxPointForAnswer, [Validators.required]),
      reducePointsPerUnit: new FormControl(this.options?.reducePointsPerUnit, [Validators.required]),
      allowNegativePoints: new FormControl(this.options?.allowNegativePoints, [Validators.required]),
    });
  }
  readOptionsFromForm(): GameOptionsModel {
    return {
      showCountry: this.form.controls['showCountry'].value ?? this.options?.showCountry,
      showPopulation: this.form.controls['showPopulation'].value ?? this.options?.showPopulation,
      capitalsWithPopulationOver: this.form.controls['capitalsWithPopulationOver'].value ?? this.options?.capitalsWithPopulationOver,
      citiesWithPopulationOver: this.form.controls['citiesWithPopulationOver'].value ?? this.options?.citiesWithPopulationOver,
      distanceUnit: this.form.controls['distanceUnit'].value ?? this.options?.distanceUnit,
      maxPointForAnswer: this.form.controls['maxPointForAnswer'].value ?? this.options?.maxPointForAnswer,
      reducePointsPerUnit: this.form.controls['reducePointsPerUnit'].value ?? this.options?.reducePointsPerUnit,
      allowNegativePoints: this.form.controls['allowNegativePoints'].value ?? this.options?.allowNegativePoints,
    }
  }
}
