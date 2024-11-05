import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { baseUrl } from '../app.config';
import { CommonModule } from '@angular/common';
import { MapComponent } from "../map/map.component";

@Component({
  selector: 'citom-home',
  standalone: true,
  imports: [CommonModule, MapComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  constructor() {
    
  }
  
  ngOnInit(): void {
    
  }
}
