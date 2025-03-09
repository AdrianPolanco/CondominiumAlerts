import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../enviroments/environment';

@Injectable({
  providedIn: 'root',
})
export class PostService {
  private apiUrl = `${environment.backBaseUrl}/posts`;

  constructor(private http: HttpClient) { }

  getPosts(): Observable<any[]> {
    return this.http.get<any>(this.apiUrl).pipe(
      map(response => {
        console.log('Respuesta completa del backend:', response); // Verifica la respuesta completa
        return response.data; // Extrae el array de publicaciones
      })
    );
  }
}
