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
  // get isLoggedIn() {
  //
  // }
  constructor(private authService: AuthService) {

  }
  onSubmit() {
    if(this.form.valid) {
      let username = this.form.controls['username'].value;
      let password = this.form.controls['password'].value;

    }
  }
  loginGoogle() {

  }
  logout() {

  }
}
