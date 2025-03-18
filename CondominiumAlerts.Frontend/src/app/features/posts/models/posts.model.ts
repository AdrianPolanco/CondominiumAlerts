import { Data } from "@angular/router";

export interface CreatePostCommand {
  title: string;
  description: string;
  ImageUrl: File;
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
