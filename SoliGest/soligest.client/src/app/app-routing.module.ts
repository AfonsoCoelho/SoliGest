import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegistarComponent } from './registar/registar.component';
import { LoginComponent } from './login/login.component';
import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';


const routes: Routes = [{ path: '', component: LoginComponent }, { path: 'registar', component: RegistarComponent }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
