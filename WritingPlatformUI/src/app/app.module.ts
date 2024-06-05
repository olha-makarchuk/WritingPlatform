import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app.routes';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { HomeComponent } from './home/home.component';
import { ProfileComponent } from "./profile/profile.component";
import { authInterceptorProviders } from './_helpers/auth.interceptor';
import { EventBusService } from "./_services/event-bus.service";
import { ToastrModule } from "ngx-toastr";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { RouterModule } from '@angular/router';
import { GenreComponent } from "./books/genre/genre.component";
import { CommonModule } from '@angular/common';
import { PublicationComponent } from "./books/publication/publication.component";
import { AuthorComponent } from "./books/author/author.component";
import { AllPublicationsComponent } from "./books/all-publications/all-publications.component";
import { ReadPublicationComponent } from "./books/read-publication/read-publication.component";
import { CreatePublicationComponent } from './books/create-publication/create-publication.component';
import { CatalogComponent } from './books/catalog/catalog.component';
import { MyPublicationsComponent } from './books/my-publications/my-publications.component';
import { NewBooksComponent } from './books/new-books/new-books.component';
import { PdfViewerModule } from 'ng2-pdf-viewer';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    HomeComponent,
    ProfileComponent,
    PublicationComponent,
    GenreComponent,
    AuthorComponent,
    AllPublicationsComponent,
    ReadPublicationComponent,
    CreatePublicationComponent,
    CatalogComponent,
    MyPublicationsComponent,
    NewBooksComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    RouterModule,
    CommonModule,
    PdfViewerModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot({
      timeOut: 3000,
      positionClass: 'toast-top-right',
      preventDuplicates: true,
    })
  ],
  providers: [authInterceptorProviders, EventBusService],
  bootstrap: [AppComponent]
})
export class AppModule { }
