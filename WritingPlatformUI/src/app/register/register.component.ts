import { Component } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import {Registration} from "../shared/models/registration";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  form: any = {
    firstName: null,
    lastName: null,
    username: null,
    email: null,
    password: null,
    role: 'User' 
  };
  isSuccessful = false;
  isSignUpFailed = false;
  errorMessage = '';

  constructor(private authService: AuthService, private toastr: ToastrService) { }

  onSubmit(): void {
    const registrationUserData: Registration = this.form;

    this.authService.register(registrationUserData).subscribe({
      next: data => {
        this.isSuccessful = true;
        this.isSignUpFailed = false;
        this.toastr.success('Registered successfully!');
      },
      error: err => {
        this.errorMessage = err.error.message;
        this.isSignUpFailed = true;
        this.toastr.error('An error occurred!');
      }
    });
  }
}
