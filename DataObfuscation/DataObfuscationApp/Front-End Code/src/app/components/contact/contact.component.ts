import { Component, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { MsalService } from '@azure/msal-angular';
import { AuthenticationResult } from '@azure/msal-browser';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent {
  currentyear: number = new Date().getFullYear();
  isShow!: boolean;
  topPosToStartShowing = 100;
  
  constructor(
    private authService: MsalService,
    private router: Router,

  ) { }

  isLoggedIn(): boolean {
    return this.authService.instance.getActiveAccount() != null;
  }
  login(){
  // this.router.navigate(['/dashboard/connect']);
  this.authService.instance.handleRedirectPromise().then(() => {

    this.authService.loginPopup()
    .subscribe((response: AuthenticationResult) => {
      this.authService.instance.setActiveAccount(response.account);
        this.router.navigate(['/dashboard/connect']);
    });
  })
}




  @HostListener('window:scroll')
  checkScroll() {
    const scrollPosition = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
    // console.log('[scroll]', scrollPosition);    
    if (scrollPosition >= this.topPosToStartShowing) {
      this.isShow = true;
    } else {
      this.isShow = false;
    }
  }

  // TODO: Cross browsing
  gotoTop() {
    window.scroll({ 
      top: 0, 
      left: 0, 
      behavior: 'smooth' 
    });
  }
}
