import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from '../app/login/login.component';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { RegistarComponent } from '../app/registar/registar.component';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,    
    ReactiveFormsModule,
    RouterModule.forChild(
      [
        { path: 'signin', component: LoginComponent },
        { path: 'new', component: RegistarComponent },
      ]
    )
  ],
})
export class ApiAuthorizationModule { }
