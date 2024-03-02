import { BrowserModule } from '@angular/platform-browser';
import { NO_ERRORS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
// import { HomeComponent } from './home/home.component';
import { AppRoutingModule } from './app-routing.module';
import { environment } from '../environments/environment';
import {
  FilteredListPipe,
  ProcessPageComponent,
} from './Modules/applications/components/process-page/process-page.component';
import { ObfuscationComponent } from './Modules/applications/components/obfuscation/obfuscation.component';
import { AdminComponent } from './Modules/applications/components/admin/admin.component';
import { ConnectComponent } from './Modules/applications/components/connect/connect.component';
import { AboutProjectComponent } from './Modules/applications/components/about-project/about-project.component';

import {
  MsalInterceptor,
  MsalBroadcastService,
  MsalModule,
  MsalService,
  MSAL_INSTANCE,
  MsalInterceptorConfiguration,
  MSAL_INTERCEPTOR_CONFIG,
  MSAL_GUARD_CONFIG,
  MsalGuardConfiguration,
} from '@azure/msal-angular';
import { MaslGuard } from './masl.guard';
import { HeaderComponent } from './components/header/header.component';
import {
  InteractionType,
  IPublicClientApplication,
  PublicClientApplication,
} from '@azure/msal-browser';
import { ToastrModule } from 'ngx-toastr';
import { MsalConfigService } from './msal-config';
import { TableModule } from 'primeng/table';
// import { FooterComponent } from './components/footer/footer.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { KeywordsComponent } from './Modules/applications/components/keywords/keywords.component';
import { NgxUiLoaderModule, NgxUiLoaderHttpModule } from 'ngx-ui-loader';
import { FooterComponent } from './components/footer/footer.component';
import { ApplicationsModule } from './Modules/applications/applications.module';
import { LoginComponent } from './components/login/login.component';
import {NgxPaginationModule} from 'ngx-pagination';


export function MSALInstanceFactory(): IPublicClientApplication {
  return new PublicClientApplication({
    auth: {
      clientId: environment.clientId,
      redirectUri: environment.redirectUri,
      authority: environment.authority,
    },
  });
}

export function msalInterceptorConfigFactory(): MsalInterceptorConfiguration {
  const protectedResourceMap = new Map<string, Array<string>>();
  protectedResourceMap.set(environment.apibaseUrl, ['openid', 'profile']);

  return {
    interactionType: InteractionType.Redirect,
    protectedResourceMap,
  };
}

export function msalGuardConfigFactory(): MsalGuardConfiguration {
  return {
    interactionType: InteractionType.Redirect, // Set to 'Redirect' for redirect-based authentication.
    authRequest: {
      scopes: ['openid', 'profile'], // Add the required scopes for authentication.
    },
  };
}

@NgModule({
  declarations: [
    AppComponent,
  ],schemas: [NO_ERRORS_SCHEMA ],
  imports: [

    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    AppRoutingModule,
    ReactiveFormsModule,
    MsalModule,
    CommonModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot({
      positionClass: 'toast-top-center',
      timeOut : 2500
    }),
    NgxUiLoaderModule,
    RouterModule.forRoot([]),
    TableModule,
    NgxPaginationModule

  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalConfigService,
      multi: true,
    },
    {
      provide: MSAL_INSTANCE,
      useFactory: MSALInstanceFactory,
    },
    {
      provide: MSAL_INTERCEPTOR_CONFIG,
      useFactory: msalInterceptorConfigFactory,
    },
    {
      provide: MSAL_GUARD_CONFIG,
      useFactory: msalGuardConfigFactory,
    },
    MsalService,
    MsalBroadcastService,
    MaslGuard,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
