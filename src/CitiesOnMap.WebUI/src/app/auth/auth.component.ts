import { Component } from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from './auth.service';

@Component({
  selector: 'citom-auth',
  standalone: true,
  imports: [
    ReactiveFormsModule,
  ],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss'
})
export class AuthComponent {
  form: FormGroup = new FormGroup({
    username: new FormControl("username", [ Validators.required ]),
    password: new FormControl("password", [ Validators.required ])
  });

  constructor(private authService: AuthService) {

  }

  register(): void {
    if(this.form.valid) {
      let email = this.form.controls['username'].value;
      let password = this.form.controls['password'].value;
      let username = email.substring(0, email.indexOf('@'));
      this.authService.registerUser(username, email, password)
        .subscribe(res => console.log(res));
    }
  }
  onSubmit() {
    if(this.form.valid) {
      let username = this.form.controls['username'].value;
      let password = this.form.controls['password'].value;

    }
  }
  loginOauth(provider: string) {
    this.authService.startExternalAuthorization(provider);
  }
  logout() {

  }
  profile() {
    this.authService.getUserInfo();
  }
}
