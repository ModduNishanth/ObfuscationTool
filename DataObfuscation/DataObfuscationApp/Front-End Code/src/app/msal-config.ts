// import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
// import { Injectable } from '@angular/core';
// import { MsalService } from '@azure/msal-angular';
// import { Observable } from 'rxjs';

// @Injectable({
//   providedIn: 'root'
// })
// export class MsalConfigService implements HttpInterceptor {

//   constructor(private authService: MsalService) {}

//   intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
//     const token = this.authService.instance.getActiveAccount()?.idToken;

//     if (token) {
//       req = req.clone({
//         setHeaders: {
//           Authorization: `Bearer ${token}`,
//         },
//       });
//     }

//     return next.handle(req);
//   }
// }
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Token } from '@angular/compiler';

import { Injectable } from '@angular/core';

import { MsalService } from '@azure/msal-angular';

import { Observable, from, mergeMap } from 'rxjs';

@Injectable({

  providedIn: 'root'

})

export class MsalConfigService implements HttpInterceptor {
  constructor(private authService: MsalService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.instance.getActiveAccount()?.idToken == undefined ? sessionStorage.getItem('bearer_token') : this.authService.instance.getActiveAccount()?.idToken;
    if (token) {
      sessionStorage.setItem('bearer_token', token);
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });
    }
    return next.handle(req);
  }
  // intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
  //   return from(this.authService.instance.acquireTokenSilent({
  //     scopes: ['openid', 'profile'], // Add any additional scopes needed
  //   })).pipe(
  //     mergeMap((tokenResponse) => {
  //       if (tokenResponse.idToken) {
  //         req = req.clone({
  //           setHeaders: {
  //             Authorization: `Bearer ${tokenResponse.idToken}`,
  //           },
  //         });
  //       }
  //       return next.handle(req);
  //     })
  //   );
  // }

}
