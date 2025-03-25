import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../enviroments/environment';
import { CreatePostCommand } from '../models/posts.model';
import { CreatePostsResponse } from '../models/posts.model';
import { AuthService } from '../../../core/auth/services/auth.service';
import { user } from '@angular/fire/auth';

@Injectable({
  providedIn: 'root',
})
export class PostService {
  private apiUrl = `${environment.backBaseUrl}/posts`;


  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) { }

  getPosts(condominiumId: string): Observable<any[]> {
    const params = new HttpParams().set('condominiumId', condominiumId);

    return this.http.get<any>(this.apiUrl, { params }).pipe(
      map(response => {
        console.log('Respuesta completa del backend:', response); 
        return response.data;
      })
    );
  }

  createPost(cmd: CreatePostCommand, condominiumId: string): Observable<CreatePostsResponse> {
    const fb = new FormData();

    const userId = this.authService.currentUser?.uid ?? '';
    if (!userId) {
      throw new Error('Usuario no autenticado.');
    }

    const fixedLevelOfPriorityId = 'fc279d15-da6d-4e8e-9407-74dc73b4a628';
    fb.append('title', cmd.title);
    fb.append('description', cmd.description);
    fb.append('imageFile', cmd.imageFile);
    fb.append('CondominiumId', condominiumId);
    fb.append('userId', userId);
    fb.append('levelOfPriorityId', fixedLevelOfPriorityId);

    console.log('User ID:', userId);
    console.log('LevelOfPriorityId:', fixedLevelOfPriorityId);

    return this.http.post<CreatePostsResponse>('/api/posts', fb);
  }
}
