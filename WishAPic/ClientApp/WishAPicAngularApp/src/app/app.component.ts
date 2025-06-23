import { Component } from '@angular/core';
import { AccountService } from './services/account.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
    public userId: string | null = null;
  title: any;

  constructor(public accountService: AccountService, public router:Router){
    this.userId = this.accountService.getUserId();
    console.log(accountService.currentUserName);
  }

  onLogOutClicked() {
      this.accountService.getLogout().subscribe({
        next: (response: string) => {
          this.accountService.currentUserName = null;
          localStorage.removeItem("token");
          this.router.navigate(['/login']);
        },
        
        error: (error) => {
          console.log(error);
        },
        complete: () => {},
      });
    }
}
