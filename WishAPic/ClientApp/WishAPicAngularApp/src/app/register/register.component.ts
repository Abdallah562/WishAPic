import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';
import { response } from 'express';
import { RegisterUser } from '../models/register-user';
import { compareValidation } from '../validators/custom-validators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm: FormGroup;
  isRegisterFormSubmitted: boolean = false;

  constructor(private accountService: AccountService,
    private router: Router
  ){
    this.registerForm = new FormGroup({
      fullName: new FormControl(null, [Validators.required]),
      email: new FormControl(null, [Validators.required,Validators.email]),
      password: new FormControl(null, [Validators.required]),
      confirmPassword: new FormControl(null, [Validators.required]),
    },
  {
    validators: [compareValidation("password","confirmPassword")]
  });
  }

  get register_fullNameControl(): any{
    return this.registerForm.controls["fullName"];
  }

  get register_emailControl(): any{
    return this.registerForm.controls["email"];
  }

  get register_passwordControl(): any{
    return this.registerForm.controls["password"];
  }

  get register_confirmPasswordControl(): any{
    return this.registerForm.controls["confirmPassword"];
  }

  registerSubmitted(){
    this.isRegisterFormSubmitted = true;
    console.log("Regester");
    
    if(this.registerForm.valid){
      console.log("Valid");

      this.accountService.postRegister(this.registerForm.value).subscribe({
        next: (response: any) =>{
          console.log(response);
          this.isRegisterFormSubmitted = false;
          this.accountService.currentUserName = response.fullName;
          this.accountService.setUserId(response.userId)
          localStorage["token"] = response.token;
          localStorage["refreshToken"] = response.refreshToken;

          this.router.navigate(['/app-home']);
          this.registerForm.reset();
        },
        error: (error) =>{
          console.log(error);
        },
        complete: () =>{},
      })
    }
  }

}
