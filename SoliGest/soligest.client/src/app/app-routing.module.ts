import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegistarComponent } from './registar/registar.component';
import { LoginComponent } from './login/login.component';
import { PwrecoveryComponent } from './pwrecovery/pwrecovery.component';
import { ChangepwComponent } from './changepw/changepw.component';

const routes: Routes = [
  { path: '', component: RegistarComponent }, // A raiz agora carrega a página de registar
  { path: 'registar', component: RegistarComponent },
  { path: 'login', component: LoginComponent }, // Corrigido para exibir a página de login corretamente
  { path: 'pwrecovery', component: PwrecoveryComponent },
  { path: '**', redirectTo: 'registar' } // Qualquer rota inválida vai para registar
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
