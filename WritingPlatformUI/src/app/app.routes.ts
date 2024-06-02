import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { ProfileComponent } from './profile/profile.component';
import { AuthGuard } from "./shared/guards/auth.guard";
import { GenreComponent } from "./books/genre/genre.component";
import { PublicationComponent } from "./books/publication/publication.component";
import { AuthorComponent } from './books/author/author.component';
import { AllPublicationsComponent } from './books/all-publications/all-publications.component';
import { ReadPublicationComponent } from './books/read-publication/read-publication.component';
import { CreatePublicationComponent } from './books/create-publication/create-publication.component';
import { CatalogComponent } from './books/catalog/catalog.component';

const routes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'genres', component: GenreComponent},
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'all-publications/by-author/:authorId', component: AllPublicationsComponent },
  { path: 'all-publications/by-genre/:genreId', component: AllPublicationsComponent },
  { path: 'all-publications', component: AllPublicationsComponent },
  { path: 'publication/:publicationId', component: PublicationComponent },
  { path: 'authors', component: AuthorComponent },
  { path: 'authors/:Idauthor', component: AuthorComponent },
  { path: 'read-publication/:publicationId/:currentPage', component: ReadPublicationComponent},
  { path: 'read-publication', component: ReadPublicationComponent},
  { path: 'create-publication', component: CreatePublicationComponent},
  { path: 'catalog', component: CatalogComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
