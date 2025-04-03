import { priorityDto } from "./priorityDto";

export interface GetPriorityLevelResponce {
    pageNumber: number;
    pageSize: number;
    totalRecords: number;
    priorities: Array<priorityDto>;
}