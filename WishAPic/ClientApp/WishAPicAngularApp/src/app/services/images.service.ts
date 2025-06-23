import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ImgData } from '../models/image-data';
// const API_BASE_URL = "https://localhost:7213/api/";
const API_BASE_URL = "http://wishapic.runasp.net/api/";

@Injectable({
  providedIn: 'root'
})
export class ImagesService {

  constructor(private http: HttpClient) { }
    public getHistory(userId: string): Observable<any> {
    const headers = { Authorization: `Bearer ${localStorage['token']}` };

    const params = { userId }; // adds ?userId=... to URL

    return this.http.get<any>(`${API_BASE_URL}sdxl/GetAllImages`, {
      headers,
      params
    });
  }

  public postAddToFavorites(imageData: ImgData):Observable<any> {
    return this.http.post<ImgData>(`${API_BASE_URL}Images/AddToFavorites`,imageData);
  }
  public getFavorites(userId: string): Observable<any> {
    const params = new HttpParams().set('userId', userId);
    return this.http.get<any[]>(`${API_BASE_URL}Images/GetFavorites`,{params});
  }
  public deleteFromFavorites(imageData: ImgData): Observable<any>{
      const options = {body: imageData};
    return this.http.delete<ImgData>(`${API_BASE_URL}Images/DeleteFromFavorites`,options)
  }
    public deleteFromHistory(imageData: ImgData): Observable<any>{
      const options = {body: imageData};
    return this.http.delete<ImgData>(`${API_BASE_URL}Images/DeleteFromHistory`,options)
  }
}
