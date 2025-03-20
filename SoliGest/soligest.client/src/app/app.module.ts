import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { RegistarComponent } from './registar/registar.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ApiAuthorizationModule } from '../api-authorization/api-authorization.module';
import { AuthInterceptor } from '../api-authorization/authorize.interceptor';
import { AuthGuard } from '../api-authorization/authorize.guard';
import { AuthorizeService } from '../api-authorization/authorize.service';
import { HomeComponent } from './home/home.component';
import { ChangepwComponent } from './changepw/changepw.component';
import { PwrecoveryComponent } from './pwrecovery/pwrecovery.component';
import { FuncionarioComponent } from './funcionario/funcionario.component';
import { FuncionarioCreateComponent } from './funcionario-create/funcionario-create.component';
import { FuncionarioDeleteComponent } from './funcionario-delete/funcionario-delete.component';
import { FuncionarioEditComponent } from './funcionario-edit/funcionario-edit.component';
import { FuncionarioDetailsComponent } from './funcionario-details/funcionario-details.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegistarComponent,
    ChangepwComponent,
    PwrecoveryComponent,
    HomeComponent,
    FuncionarioComponent,
    FuncionarioCreateComponent,
    FuncionarioDeleteComponent,
    FuncionarioEditComponent,
    FuncionarioDetailsComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule, FormsModule,
    ReactiveFormsModule, ApiAuthorizationModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    AuthGuard,
    AuthorizeService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
