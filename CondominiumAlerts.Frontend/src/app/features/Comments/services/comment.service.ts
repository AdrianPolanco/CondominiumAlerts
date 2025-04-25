import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../enviroments/environment';
import { AddCommentCommand } from '../../Comments/models/AddComment.Command'
import { AddCommentResponse } from '../../Comments/models/AddComment.Response'
import { getCommentByPostCommand } from '../../Comments/models/getCommentByPost.Command'
import { getCommentByPostResponse } from '../../Comments/models/getCommentByPost.Reponse'
import { UpdateCommentResponse } from '../../Comments/models/updateComment.Response'
import { UpdateCommentCommand } from '../../Comments/models/updateComment.Command'
import { DeleteCommentResponse } from '../../Comments/models/deleteComment.Response'
import { AuthService } from '../../../core/auth/services/auth.service';
import { DeleteCommentCommand } from '../models/deleteComment.Command';

@Injectable({
  providedIn: 'root',
})

export class CommetService
{
  private apiUrl = `${environment.backBaseUrl}/comment`;
  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) { }

  getCommentsByPost(command: getCommentByPostCommand): Observable<{ isSuccess: boolean; data: getCommentByPostResponse[] }> {
    const params = new HttpParams().set('postId', command.postid);
    return this.http.get<{ isSuccess: boolean; data: getCommentByPostResponse[] }>(this.apiUrl, { params });
  }



  createComment(data: AddCommentCommand, postId: string): Observable<AddCommentResponse> {
    const fb = new FormData();
    const userId = this.authService.currentUser?.uid ?? '';

    if (!userId) {
      throw new Error('Usuario no autenticado.');
    }

    fb.append('text', data.text);
    if (data.ImageFile) fb.append('imageFile', data.ImageFile);
    fb.append('userId', userId);
    fb.append('postId', postId);

    return this.http.post<AddCommentResponse>(this.apiUrl, fb);
  }

  updateComment(commentId: string, cmd: UpdateCommentCommand): Observable<any> {
    const formData = new FormData();
    formData.append('text', cmd.text);

    if (cmd.imageFile) {
      formData.append('imageFile', cmd.imageFile);
    }

    return this.http.put(
      `${this.apiUrl}/${commentId}`, 
      formData
    );
  }

  //Delete comments
  deleteComment(command: DeleteCommentCommand): Observable<DeleteCommentResponse> {
    const url = `${this.apiUrl}/delete`;
    return this.http.delete<DeleteCommentResponse>(url, {
      body: command
    });
  }
}
