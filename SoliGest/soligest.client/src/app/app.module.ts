import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { NgApexchartsModule } from "ng-apexcharts";

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
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
import { FuncionarioEditComponent } from './funcionario-edit/funcionario-edit.component';
import { PaineisSolaresComponent } from './paineis-solares/paineis-solares.component';

import { AvariasComponent } from './avarias/avarias.component';
import { ChatComponent } from './chat/chat.component';
import { AboutUsComponent } from './about-us/about-us.component';
import { HelpComponent } from './help/help.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { MetricsComponent } from './metrics/metrics.component';
import { MyprofileComponent } from './myprofile/myprofile.component';




@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    ChangepwComponent,
    PwrecoveryComponent,
    HomeComponent,
    FuncionarioComponent,
    FuncionarioCreateComponent,
    FuncionarioEditComponent,
    PaineisSolaresComponent,
    AvariasComponent,
    ChatComponent,
    AboutUsComponent,
    HelpComponent,
    HeaderComponent,
    FooterComponent,
    MetricsComponent,
    MyprofileComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule, FormsModule,
    ReactiveFormsModule, ApiAuthorizationModule,
    NgApexchartsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    AuthGuard,
    AuthorizeService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
