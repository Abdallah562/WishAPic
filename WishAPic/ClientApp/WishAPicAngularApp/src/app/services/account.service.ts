import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterUser } from '../models/register-user';
import { Observable } from 'rxjs';
import { LoginUser } from '../models/login-user';
// const API_BASE_URL = "https://localhost:7213/api/account/";
const API_BASE_URL = "http://wishapic.runasp.net/api/account/"

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  public currentUserName: string | null = null;
  private currentUserId: string | null = null;

  constructor(private httpClient: HttpClient) { }
  getUserId(): string | null {
    if(this.currentUserId)
      return this.currentUserId;
    else
      return null;
  }
    setUserId(id: string) {
    this.currentUserId = id;
  }

  public postRegister(RegisterUser:RegisterUser):
   Observable<any>{
    return this.httpClient.post<RegisterUser>(`${API_BASE_URL}register`,RegisterUser);
  }

  public postLogin(LoginUser:LoginUser):
  Observable<any>{
   return this.httpClient.post<LoginUser>(`${API_BASE_URL}login`,LoginUser);
 }

 public getLogout(): Observable<string>{
  return this.httpClient.get<string>(`${API_BASE_URL}logout`);
}

public postGenerateNewToken():
Observable<any>{
  var token = localStorage["token"];
  var refreshToken = localStorage["refreshToken"]
 return this.httpClient.post<LoginUser>(`${API_BASE_URL}generate-new-jwt-token`,{token: token, refreshToken: refreshToken});
}

}
