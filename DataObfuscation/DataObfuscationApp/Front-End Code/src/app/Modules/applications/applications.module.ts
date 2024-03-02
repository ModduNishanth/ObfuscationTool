import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ApplicationsRoutingModule } from './applications-routing.module';
import { ContainerComponent } from './container/container.component';
import { HeaderComponent } from 'src/app/components/header/header.component';
import { FooterComponent } from 'src/app/components/footer/footer.component';
import { HomeComponent } from 'src/app/Modules/applications/components/home/home.component';
import { BrowserModule } from '@angular/platform-browser';
import { ProcessPageComponent } from './components/process-page/process-page.component';
import { ObfuscationComponent } from './components/obfuscation/obfuscation.component';
import { KeywordsComponent } from './components/keywords/keywords.component';
import { ConnectComponent } from './components/connect/connect.component';
import { AdminComponent } from './components/admin/admin.component';
import { AboutProjectComponent } from './components/about-project/about-project.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TableModule } from 'primeng/table';
import {NgxPaginationModule} from 'ngx-pagination';

@NgModule({
  declarations: [
    ContainerComponent,
    HeaderComponent
    ,FooterComponent,ProcessPageComponent,ObfuscationComponent,KeywordsComponent,
    ConnectComponent,HomeComponent,AdminComponent,AboutProjectComponent

  ],
  imports: [
    TableModule,
    CommonModule,
    ApplicationsRoutingModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    NgxPaginationModule
  ],
  exports:[
  ContainerComponent,
  HeaderComponent
  ,FooterComponent,ProcessPageComponent,ObfuscationComponent,KeywordsComponent,
  ConnectComponent,HomeComponent,AdminComponent,AboutProjectComponent
]
})
export class ApplicationsModule { }
