import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Registration } from "../shared/models/registration";
import { TokenApi } from "../shared/models/token-api";

const AUTH_API = 'https://localhost:7265/api/v1.0/';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  constructor(private http: HttpClient) { }

  login(username: string, password: string): Observable<any> {
    return this.http.post(AUTH_API + 'Authentication/login', {
      username,
      password
    }, httpOptions);
  }

  register(registrationUserData: Registration): Observable<any> {
    return this.http.post(AUTH_API + 'Authentication/registration', registrationUserData, httpOptions);
  }

  refreshToken(tokenData: TokenApi) {
    return this.http.post(AUTH_API + 'Token/refresh', tokenData, httpOptions);
  }
}
