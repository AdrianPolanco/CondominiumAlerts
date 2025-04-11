import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GetPriorityLevelResponce } from '../priority-levels/models/getPriorityLevelResponce';
import { getPriorityLevelsQuery } from '../priority-levels/models/getPriorityLevelsQuery';
import { getByIdPriorityLevelQuery } from '../priority-levels/models/getByIdPriorityLevelQuery';
import { getByIdPriorityLevelResponse } from '../priority-levels/models/getByIdPriorityLevelResponse';
import { addPriorityLevelCommand } from '../priority-levels/models/addPriorityLevelCommand';
import { addPriorityLevelResponse } from '../priority-levels/models/addPriorityLevelResponse';
import { updatePriorityLevelCommand } from '../priority-levels/models/updatePriorityLevelCommand';
import { updatePriorityLevelResponse } from '../priority-levels/models/updatePriorityLevelResponse';
import { deletePriorityLevelCommand } from '../priority-levels/models/deletePriorityLevelCommand';
import { deletePriorityLevelResponse } from '../priority-levels/models/deletePriorityLevelResponse';

@Injectable({
  providedIn: 'root'
})
export class PriorityLevelService {

  constructor(private httpClient: HttpClient) {
  }

  getPriorityLevels(request: getPriorityLevelsQuery) : Observable<{ isSuccess: boolean,data:GetPriorityLevelResponce}>{

    return this.httpClient.get<{ isSuccess: boolean,data:GetPriorityLevelResponce}>('/api/priorityLevels', {params:
      {condominiumId: request.condominiumId, pageNumber:  request.pageNumber, pageSize: request.pageSize}})
  }

  getPriorityLevelById(request: getByIdPriorityLevelQuery): Observable<{ isSuccess: boolean,data:getByIdPriorityLevelResponse}>{
    return this.httpClient.get<{ isSuccess: boolean,data:getByIdPriorityLevelResponse}>('/api/priorityLevels/id', {
      params:{id: request.id,condominiumId: request.condominiumId }
    })
  }

  postPriorityLevel(request: addPriorityLevelCommand): Observable<{ isSuccess: boolean,data:addPriorityLevelResponse}>{
console.log(request)
    return this.httpClient.post<{ isSuccess: boolean,data:addPriorityLevelResponse}>('/api/priorityLevels/add', request)
  }

  updatePriorityLevel(request: updatePriorityLevelCommand) : Observable<updatePriorityLevelResponse>{
    return this.httpClient.put<updatePriorityLevelResponse>('/api/priorityLevels/update', request)
  }

  deletePriorityLevel(request: deletePriorityLevelCommand ) : Observable<deletePriorityLevelResponse>{
    return this.httpClient.delete<deletePriorityLevelResponse>('/api/priorityLevels/delete', { body:{id: request.id,condominiumId: request.condominiumId }})
  }
}
