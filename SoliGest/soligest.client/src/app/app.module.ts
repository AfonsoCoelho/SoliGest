import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { RegistarComponent } from './registar/registar.component';
import { PwrecoveryComponent } from './pwrecovery/pwrecovery.component';
import { ChangepwComponent } from './changepw/changepw.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegistarComponent,
    PwrecoveryComponent,
    ChangepwComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
