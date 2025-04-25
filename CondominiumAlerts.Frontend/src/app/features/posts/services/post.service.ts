import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../enviroments/environment';
import {
  CreatePostCommand,
  CreatePostsResponse,
  UpdatePostCommand,
  UpdatePostResponse,
  PostFormData,
  DeletePostCommand,
  DeletePostResponse
} from '../models/posts.model';
import { AuthService } from '../../../core/auth/services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class PostService {
  private apiUrl = `api/posts`;

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

  getCurrentUserId(): string | null {
    return this.authService.currentUser?.uid ?? null;
  }

  getPostById(postId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${postId}`).pipe(
      map(response => response.data)
    );
  }

  createPost(formData: PostFormData, condominiumId: string): Observable<CreatePostsResponse> {
    const fb = new FormData();
    const userId = this.authService.currentUser?.uid ?? '';

    if (!userId) {
      throw new Error('Usuario no autenticado.');
    }

    fb.append('title', formData.title);
    fb.append('description', formData.description);
    if (formData.imageFile) fb.append('imageFile', formData.imageFile);
    fb.append('userId', userId);
    fb.append('levelOfPriorityId', formData.LevelOfPriorityId);
    fb.append('condominiumId', condominiumId);

    return this.http.post<CreatePostsResponse>(this.apiUrl, fb);
  }

  updatePost(postId: string, cmd: UpdatePostCommand): Observable<UpdatePostResponse> {
    const fb = new FormData();

    fb.append('title', cmd.title);
    fb.append('description', cmd.description);
    fb.append('levelOfPriorityId', cmd.levelOfPriorityId);

    // Solo adjuntar la imagen si existe
    if (cmd.imageFile) {
      fb.append('imageFile', cmd.imageFile);
    }

    console.log('Actualizando post ID:', postId);
    console.log('Datos enviados:', {
      title: cmd.title,
      description: cmd.description,
      levelOfPriorityId: cmd.levelOfPriorityId,
      hasImage: !!cmd.imageFile
    });

    return this.http.put<UpdatePostResponse>(`${this.apiUrl}/${postId}`, fb);
  }

  //Delete post
  deletePost(command: DeletePostCommand): Observable<DeletePostResponse> {
    const url = `${this.apiUrl}/delete`;
    return this.http.delete<DeletePostResponse>(url, {
      body: command
    });
  }
}
