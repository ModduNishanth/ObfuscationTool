import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MsalService } from '@azure/msal-angular';
import { AuthenticationResult } from '@azure/msal-browser';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  constructor(private authService: MsalService, private router: Router) {}

  isLoggedIn(): boolean {
    return this.authService.instance.getActiveAccount() != null;
  }
  login(){
  // this.router.navigate(['/dashboard/connect']);
  this.authService.instance.handleRedirectPromise().then(res => {

    this.authService.loginPopup()
    .subscribe((response: AuthenticationResult) => {
      this.authService.instance.setActiveAccount(response.account);
        this.router.navigate(['/obfuscation/project']);
    });
  })
}
}
