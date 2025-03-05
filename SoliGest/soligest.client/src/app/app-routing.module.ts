import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegistarComponent } from './registar/registar.component';
import { LoginComponent } from './login/login.component';
import { PwrecoveryComponent } from './pwrecovery/pwrecovery.component';

const routes: Routes = [{ path: '', component: PwrecoveryComponent }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
