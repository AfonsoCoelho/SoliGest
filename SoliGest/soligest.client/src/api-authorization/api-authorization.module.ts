import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from '../app/login/login.component';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { FuncionarioCreateComponent } from '../app/funcionario-create/funcionario-create.component';

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,    
    ReactiveFormsModule,
    RouterModule.forChild(
      [
        { path: 'signin', component: LoginComponent },
        { path: 'new', component: FuncionarioCreateComponent },
      ]
    )
  ],
})
export class ApiAuthorizationModule { }
