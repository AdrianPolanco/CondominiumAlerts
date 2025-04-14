export interface NotificationDto {
  id: string;
  title: string;
  description?: string;
  createdAt: Date;
  levelOfPriority: LevelOfPriorityDto;
}

export interface LevelOfPriorityDto {
  id: string;
  title: string;
  priority: number;
}