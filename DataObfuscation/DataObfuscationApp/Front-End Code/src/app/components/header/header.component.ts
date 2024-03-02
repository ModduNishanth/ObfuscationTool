import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MsalService } from '@azure/msal-angular';
import { AuthenticationResult } from '@azure/msal-browser';
import { log } from 'console';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {
  constructor(private authService:MsalService,private router:Router){
  }

  ngOnInit(): void {
    // this.msalService.instance.handleRedirectPromise().then(res =>{
    //   if (res !=null && res.account !=null){
    //     this.msalService.instance.setActiveAccount(res.account)
    //   }
    // })
  }

  // isloggedIn(): boolean{
  //   return this.msalService.instance.getActiveAccount() != null
  // }

  // login() {

  //   // this.authService.loginRedirect();
  //   this.msalService.loginPopup()

  //     .subscribe((response: AuthenticationResult) => {

  //       this.msalService.instance.setActiveAccount(response.account);

  //     });

  // }

  logout() {
    this.authService.logout();
       this.router.navigate(['/login']);
  }
}
