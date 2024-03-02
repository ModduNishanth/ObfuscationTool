import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ConnectComponent } from './components/connect/connect.component';

import { AboutProjectComponent } from './components/about-project/about-project.component';
import { KeywordsComponent } from './components/keywords/keywords.component';
import { ProcessPageComponent } from './components/process-page/process-page.component';
import { ObfuscationComponent } from './components/obfuscation/obfuscation.component';
import { AdminComponent } from './components/admin/admin.component';
import { MaslGuard } from 'src/app/masl.guard';
import { HomeComponent } from './components/home/home.component';

const routes: Routes = [
  {
    path: 'home',
    component:HomeComponent,
    canActivate:[MaslGuard]
  },
  {
    path: 'project',
    component: ConnectComponent,
    canActivate: [MaslGuard],
  },
  {
    path: 'help',
    component: AboutProjectComponent,
    canActivate: [MaslGuard],
  },
  {
    path: 'keywords',
    component: KeywordsComponent,
    canActivate: [MaslGuard],
  },

  {
    path: 'mapping',
    component: ProcessPageComponent,
    canActivate: [MaslGuard],
  },
  {
    path: 'obfuscation',
    component: ObfuscationComponent,
    canActivate: [MaslGuard],
  },
  {
    path: 'functions',
    component: AdminComponent,
    canActivate: [MaslGuard],
  },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ApplicationsRoutingModule { }
