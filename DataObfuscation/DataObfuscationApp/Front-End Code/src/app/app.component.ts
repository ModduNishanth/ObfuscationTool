import { Component, OnInit } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { AuthenticationResult } from '@azure/msal-browser';
import { log } from 'console';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  title = 'app';

  constructor() {}

  ngOnInit(): void {
    // this.msalService.instance.handleRedirectPromise().then((res) => {
    //   if (res != null && res.account != null) {
    //     this.msalService.instance.setActiveAccount(res.account);
    //   }
    // });
  }

  // isloggedIn(): boolean {
  //   return this.msalService.instance.getActiveAccount() != null;
  // }

  // login() {
  //   // this.authService.loginRedirect();
  //   this.msalService
  //     .loginPopup()
  //     .subscribe((response: AuthenticationResult) => {
  //       this.msalService.instance.setActiveAccount(response.account);
  //     });
  // }

  // logout() {
  //   this.msalService.logout();
  // }
}
