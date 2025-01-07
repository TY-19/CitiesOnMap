import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthComponent } from '../auth/auth.component'
import {FooterComponent} from "../footer/footer.component";

@Component({
  selector: 'citom-home',
  standalone: true,
    imports: [CommonModule, AuthComponent, FooterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {

}
