import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../enviroments/environment';

@Injectable({
  providedIn: 'root',
})
export class PostService {
  private apiUrl = `${environment.backBaseUrl}/posts`;

  constructor(private http: HttpClient) { }

  getPosts(condominiumId: string): Observable<any[]> {
    const params = new HttpParams().set('condominiumId', condominiumId);

    return this.http.get<any>(this.apiUrl, { params }).pipe(
      map(response => {
        console.log('Respuesta completa del backend:', response); 
        return response.data;
      })
    );
  }
}
