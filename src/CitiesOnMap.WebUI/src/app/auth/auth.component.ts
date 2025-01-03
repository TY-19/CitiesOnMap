import {Component} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from './auth.service';
import {Router, RouterLink} from "@angular/router";

@Component({
  selector: 'citom-auth',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss'
})
export class AuthComponent {
  showSignIn: boolean = false;
  mode: "login" | "register" = "login";
  form: FormGroup = new FormGroup({
    username: new FormControl("", [ Validators.required ]),
    password: new FormControl("", [ Validators.required ])
  });
  registrationForm: FormGroup = new FormGroup({
    username: new FormControl("", [ Validators.required ]),
    email: new FormControl("", [ Validators.required, Validators.email ]),
    password: new FormControl("", [ Validators.required, Validators.minLength(8) ])
  });

  get userName(): string | null {
    return this.authService.userName;
  }

  constructor(private authService: AuthService,
              private router: Router) {

  }

  register(): void {
    if(this.registrationForm.valid) {
      const username: string = this.registrationForm.controls['username'].value;
      const email: string = this.registrationForm.controls['email'].value;
      const password: string = this.registrationForm.controls['password'].value;
      this.authService.registerUser(username, email, password)
        .subscribe((_) => {
          this.router.navigate(['game']).then();
        });
    }
  }
  login(): void {
    if(this.form.valid) {
      const username = this.form.controls['username'].value;
      const password = this.form.controls['password'].value;
      this.authService.login(username, password)
        .subscribe((_) => {
          this.router.navigate(['game']).then();
        });
    }
  }
  loginOauth(provider: string): void {
    this.authService.startExternalAuthorization(provider);
  }
  logout(): void {
    this.authService.logOut()
      .subscribe();
  }
}

