import { Component } from '@angular/core';
import {RouterLink} from "@angular/router";

@Component({
  selector: 'citom-footer',
  standalone: true,
  imports: [
    RouterLink
  ],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent {
  year: string;
  constructor() {
    this.year = (new Date()).getFullYear().toString();
  }
}
