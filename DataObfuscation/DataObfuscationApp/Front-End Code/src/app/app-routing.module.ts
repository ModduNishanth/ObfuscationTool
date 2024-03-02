import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProcessPageComponent } from './Modules/applications/components/process-page/process-page.component';
import { ObfuscationComponent } from './Modules/applications/components/obfuscation/obfuscation.component';
import { AdminComponent } from './Modules/applications/components/admin/admin.component';
import { ConnectComponent } from './Modules/applications/components/connect/connect.component';
import { AboutProjectComponent } from './Modules/applications/components/about-project/about-project.component';
import { MaslGuard } from './masl.guard';
import { KeywordsComponent } from './Modules/applications/components/keywords/keywords.component';
import { LoginComponent } from './components/login/login.component';
import { ContainerComponent } from './Modules/applications/container/container.component';
import { HomeComponent } from './Modules/applications/components/home/home.component';
import { ContactComponent } from './components/contact/contact.component';

const routes: Routes = [
  {
    path: '',
    component: LoginComponent
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'contact',
    component: ContactComponent
  },
  {
    path: 'obfuscation',
    component: ContainerComponent,
    loadChildren :  () => import("./Modules/applications/applications-routing.module").then(m =>m.ApplicationsRoutingModule)
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
