import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';

import { PwrecoveryComponent } from './pwrecovery/pwrecovery.component';
import { ChangepwComponent } from './changepw/changepw.component';
import { FuncionarioComponent } from './funcionario/funcionario.component';
import { FuncionarioCreateComponent } from './funcionario-create/funcionario-create.component';
import { FuncionarioEditComponent } from './funcionario-edit/funcionario-edit.component';
import { PaineisSolaresComponent } from './paineis-solares/paineis-solares.component';
import { AvariasComponent } from './avarias/avarias.component';
import { ChatComponent } from './chat/chat.component';
import { AboutUsComponent } from './about-us/about-us.component';
import { HelpComponent } from './help/help.component';
import { MetricsComponent } from './metrics/metrics.component';
import { AuthGuard } from '../api-authorization/authorize.guard';
import { MyprofileComponent } from './myprofile/myprofile.component';

const routes: Routes = [
  { path: '', title: 'SoliGest', component: HomeComponent, canActivate: [AuthGuard] }, // A raiz agora carrega a página de registar
  { path: 'login', title: 'Login • SoliGest', component: LoginComponent },
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] }, // Corrigido para exibir a página de login corretamente
  { path: 'pwrecovery', component: PwrecoveryComponent },
  { path: 'changepw', component: ChangepwComponent },
  { path: 'funcionario', component: FuncionarioComponent, canActivate: [AuthGuard] },
  { path: 'funcionario-create', component: FuncionarioCreateComponent, canActivate: [AuthGuard] },
  { path: 'paineis-solares', component: PaineisSolaresComponent, canActivate: [AuthGuard] },
  { path: 'avarias', component: AvariasComponent, canActivate: [AuthGuard] },
  { path: 'funcionario-edit/:id', component: FuncionarioEditComponent, canActivate: [AuthGuard] },
  { path: 'chat', component: ChatComponent, canActivate: [AuthGuard] },
  { path: 'about-us', component: AboutUsComponent },
  { path: 'help', component: HelpComponent },
  { path: 'myprofile', component: MyprofileComponent, canActivate: [AuthGuard] },
  { path: 'metrics', component: MetricsComponent, canActivate: [AuthGuard] },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
