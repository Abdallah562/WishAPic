import { Component } from '@angular/core';
import { LoginUser } from '../models/login-user';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

    loginForm: FormGroup;
    isLoginFormSubmitted: boolean = false;
  
    constructor(private accountService: AccountService,
      private router: Router
    ){
      this.loginForm = new FormGroup({
        email: new FormControl(null, [Validators.required,Validators.email]),
        password: new FormControl(null, [Validators.required]),
      });
    }
  
  
    get login_emailControl(): any{
      return this.loginForm.controls["email"];
    }

    get login_passwordControl(): any{
      return this.loginForm.controls["password"];
    }

  
    loginSubmitted(){
      this.isLoginFormSubmitted = true;
      
      if(this.loginForm.valid){
  
        this.accountService.postLogin(this.loginForm.value).subscribe({
          next: (response: any) =>{
            console.log(response);
            this.isLoginFormSubmitted = false;
            this.accountService.currentUserName = response.fullName;
            this.accountService.setUserId(response.userId)
            localStorage["token"] = response.token;
            localStorage["refreshToken"] = response.refreshToken;
            console.log("Login: "+this.accountService.getUserId());
            
            this.router.navigate(['/app-home']);
            this.loginForm.reset();
          },
          error: (error:any) =>{
            console.log(error);
          },
          complete: () =>{},
        })
      }
    }
  

  
}
