import { Data } from "@angular/router";

export interface CreatePostCommand {
  title: string;
  description: string;
  imageFile: File;
  userId: string;
  LevelOfPriorityId: string;
  CondominiumId: string;
}

export interface CreatePostsResponse {
  isSuccess: boolean;
  data?: {
    id: string;
    title: string;
    description: string;
    imageUrl: string;
    CondominiumId: string;
    UserId: string;
    LevelOfPriorityId: string;
    CreatedAt: string;
    UpdatedAt: string;
  },
  errors?: any[]
}
