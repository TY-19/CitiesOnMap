import { Component } from '@angular/core';
import {ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'citom-callback',
  standalone: true,
  imports: [],
  templateUrl: './callback.component.html',
  styleUrl: './callback.component.scss'
})
export class CallbackComponent {
  constructor(
    private authService: AuthService,
    private activatedRoute: ActivatedRoute,
    private router: Router) {
  }
  ngOnInit(): void {

  }
}
