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
    let stateVerifier = localStorage.getItem("oauth-request-state") ?? "";
    this.activatedRoute.queryParams.subscribe((params) => {
      const authorizationCode = params['code'];
      const state: string | undefined = params['state'];
      if (this.authService. verifyState(state)) {
        console.error('Invalid state parameter');
        return;
      }
      let provider = stateVerifier.split(':')[0];
      if (authorizationCode) {
        this.authService.loginExternal(provider, authorizationCode)
          .subscribe(res => {
            this.router.navigate(['/']);
          })
      } else {
        console.error('Authorization code not found in callback');
      }
    });
  }
}
