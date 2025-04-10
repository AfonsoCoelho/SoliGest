import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RegistarComponent } from './registar/registar.component';
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
import { FuncionarioDetailsComponent } from './funcionario-details/funcionario-details.component'; //vamos apagar
import { ChatComponent } from './chat/chat.component';
import { AboutUsComponent } from './about-us/about-us.component';
import { HelpComponent } from './help/help.component';

const routes: Routes = [
  { path: '', component: HomeComponent }, // A raiz agora carrega a página de registar
  { path: 'registar', component: RegistarComponent },
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomeComponent }, // Corrigido para exibir a página de login corretamente
  { path: 'pwrecovery', component: PwrecoveryComponent },
  { path: 'changepw', component: ChangepwComponent },
  { path: 'funcionario', component: FuncionarioComponent },
  { path: 'funcionario-create', component: FuncionarioCreateComponent },
  { path: 'paineis-solares', component: PaineisSolaresComponent },
  { path: 'avarias', component: AvariasComponent },
  { path: 'funcionario-edit/:id', component: FuncionarioEditComponent },
  { path: 'funcionario-details/:id', component: FuncionarioDetailsComponent },
  { path: 'chat', component: ChatComponent },
  { path: 'about-us', component: AboutUsComponent },
  { path: 'help', component: HelpComponent },

  { path: '**', redirectTo: '' } // Qualquer rota inválida vai para registar
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
