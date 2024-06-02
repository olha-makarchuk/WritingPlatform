import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {Author} from "../shared/models/author";
import {Genre} from "../shared/models/genre";
import { Publication } from '../shared/models/publication';
import { PublicationText } from '../shared/models/publication-text';
import { CreateComment } from '../shared/models/create-comment';
import { Comments } from '../shared/models/get-comment';
import { PublicationCreate } from '../shared/models/publication-create';
import { PersonalInformation } from '../shared/models/personal-informatin';
import { PersonalInformationChange } from '../shared/models/personal-informatin-change';


const API_URL = 'https://localhost:7265/api/v1.0/';

@Injectable({
  providedIn: 'root'
})

export class UserService {
  constructor(private http: HttpClient) { }

  getAuthors(): Observable<Array<Author>> {
    return this.http.get<Array<Author>>(API_URL + 'Author', { responseType: 'json' });
  }

  getGenres(): Observable<Array<Genre>> {
    return this.http.get<Array<Genre>>(API_URL + 'Genre', { responseType: 'json' });
  }

  getPublicationByGenre(): Observable<Array<Publication>> {
    return this.http.get<Array<Publication>>(API_URL + 'Publication', { responseType: 'json' });
  }

  getPublicationsByAuthor(authorId: number): Observable<Publication[]> {
    const body = { IdAuthor: authorId };
    return this.http.post<Publication[]>(API_URL + 'Publication/by-author', body);
  }

  getPublicationsByGenre(genreId: number): Observable<Publication[]> {
    const body = { IdGenre: genreId };
    return this.http.post<Publication[]>(API_URL + 'Publication/by-genre', body);
  }

  getAllPublications(): Observable<Array<Publication>> {
    return this.http.get<Array<Publication>>(API_URL + 'Publication', { responseType: 'json' });
  }

  getPublicationById(publicationId: number): Observable<Publication> {
    const body = { IdPublication: publicationId };
    return this.http.post<Publication>(API_URL + 'Publication/by-id', body);
  }

  getPublicationText(publicationId: number, currentPage: number): Observable<PublicationText> {
    const body = { IdPublication: publicationId,  currentPage: currentPage};
    return this.http.post<PublicationText>(API_URL + 'Publication/text', body);
  }

  createComment(publicationId: number, commentText: string, applicationUserId: string): Observable<CreateComment> {
    const body = { publicationId: publicationId,  applicationUserId: applicationUserId, commentText: commentText};
    return this.http.post<CreateComment>(API_URL + 'Comment', body);
  }

  getComments(publicationId: number): Observable<Array<Comments>> {
    const body = { IdPublication: publicationId };
    return this.http.post<Array<Comments>>(API_URL + 'Comment/by-publication', body);
  }

  CreatePublication(publication: PublicationCreate): Observable<void> {
    const formData: FormData = new FormData();
    formData.append('publicationName', publication.publicationName);
    formData.append('genreId', publication.genreId.toString());
    formData.append('userName', publication.userName);
    formData.append('filePath', publication.filePath, publication.filePath.name);
    formData.append('titlePath', publication.titlePath, publication.titlePath.name);
    formData.append('bookDescription', publication.bookDescription);
  
    return this.http.post<void>(API_URL + 'Publication', formData);
  }
  
  deleteComment(commentId: number): Observable<void> {
    return this.http.delete<void>(API_URL + 'Comment/' + commentId);
  }
  
  getUserByUserName(userName: string): Observable<PersonalInformation> {
    const body = { userName: userName };
    return this.http.post<PersonalInformation>(API_URL + 'UserAccount/by-username', body);
  }

  changeUserInfoByUserName(userId: string, userName: string, lastName: string, firstName: string, personalInformation: string): Observable<PersonalInformationChange> {
    const body = { 
      userId:userId,
      userName: userName,
      lastName: lastName,
      firstName: firstName,
      personalInformation: personalInformation };
    return this.http.post<PersonalInformationChange>(API_URL + 'UserAccount/change', body);
  }
}