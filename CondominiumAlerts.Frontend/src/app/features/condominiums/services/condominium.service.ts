import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, takeUntil } from 'rxjs';
import { AddCondominiumResponse } from "../models/addCondominium.response";
import { GetCondominiumsJoinedByUserResponse } from "../models/getCondominiumsJoinedByUser.response";
import { GetCondominiumsJoinedByUserCommand } from "../models/getCondominiumsJoinedByUser.command";
import { GetCondominiumResponse } from "../models/getCondominium.response";
import { GetCondominiumCommand } from "../models/getCondominium.command";
import { JoinCondominiumResponse } from "../models/joinCondominium.response";
import { JoinCondominiumCommand } from "../models/joinCondominium.command";
import { AddCondominiumCommand } from "../models/addCondominium.command";
import { AuthenticationService } from '../../../core/services/authentication.service';
import { AutoUnsubscribe } from '../../../shared/decorators/autounsuscribe.decorator';
import { Condominium } from '../models/condominium.model';

@AutoUnsubscribe()
@Injectable({
  providedIn: 'root'
})
export class CondominiumService {

  constructor(private httpClient: HttpClient, 
    private authenticationService: AuthenticationService) { 
      this.authenticationService.userToken$.pipe(takeUntil(this.destroy$)).subscribe((token) => {
        this.token = token;
      })
  }

  private destroy$ = new Subject<void>();
  private token: string | null = null;
  private current: GetCondominiumsJoinedByUserResponse | null = null;

  get currentCondominium(): GetCondominiumsJoinedByUserResponse | null{
    return this.current;
  }

    create(cmd: AddCondominiumCommand): Observable<AddCondominiumResponse> {
        const fb = new FormData();
        fb.append('name', cmd.name);
        fb.append('address', cmd.address);
        fb.append('imageFile',cmd.imageFile);
        console.log("data", {
            name: fb.get('name'),
            address: fb.get('address'),
            imageFile: fb.get('imageFile')
        })
        return this.httpClient.post<AddCondominiumResponse>(
            "/api/condominium",
            fb,
        )
    }

    setCondominium(condominium: GetCondominiumsJoinedByUserResponse): void{
      this.current = condominium;
    }

    join(cmd: JoinCondominiumCommand): Observable<JoinCondominiumResponse>{
      const fb = new FormData();
      fb.append('userId', cmd.userId)
      fb.append('condominiumCode', cmd.condominiumCode)

      return this.httpClient.post<JoinCondominiumResponse>(
        "/api/condominium/join", fb)
    }

    get(cmd: GetCondominiumCommand): Observable<GetCondominiumResponse>{
      
      return this.httpClient.get<GetCondominiumResponse>(
        "/api/condominium/GetById", {params: {condominiumId: cmd.condominiumId}})
    }
 
    getCondominiumsJoinedByUser(cmd: GetCondominiumsJoinedByUserCommand): Observable<{
          isSuccess:boolean,
          data: GetCondominiumsJoinedByUserResponse[]
        }>{
      
      return this.httpClient.get<{
            isSuccess:boolean,
            data: GetCondominiumsJoinedByUserResponse[]
          }>(
        "/api/condominium/GetCondominiumsJoinedByUser",  {params: {userId: cmd.userId}, headers: {Authorization: `Bearer ${this.token}`}})
    }
}
