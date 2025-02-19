import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AddCondominiumCommand, AddCondominiumResponse, JoinCondominiumCommand , JoinCondominiumResponce} from '../models/condominium.model';

@Injectable({
  providedIn: 'root'
})
export class CondominiumService {

  constructor(
      private httpClient: HttpClient
  ) { }


    create(cmd: AddCondominiumCommand): Observable<AddCondominiumResponse> {
        const fb = new FormData();
        fb.append('name', cmd.name);
        fb.append('address', cmd.address);
        fb.append('imageFile',cmd.imageFile);
        return this.httpClient.post<AddCondominiumResponse>(
            "/api/condominium",
            fb,
        )
    }

    join(cmd: JoinCondominiumCommand): Observable<JoinCondominiumResponce>{
      const fb = new FormData();
      fb.append('UserId', cmd.userId)
      fb.append('condominiumCode', cmd.condominiumCode)

      return this.httpClient.post<JoinCondominiumResponce>(
        "/api/condominium/join", fb)
    }
}
