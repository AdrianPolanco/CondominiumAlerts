import { Data } from "@angular/router";

export interface CreatePostCommand {
  title: string;
  description: string;
  imageFile?: File | null;
  userId: string;
  LevelOfPriorityId: string;
  CondominiumId: string;
}

export interface PostFormData {
  title: string;
  description: string;
  imageFile?: File | null;
  LevelOfPriorityId: string;
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
export interface UpdatePostCommand {
  title: string;
  description: string;
  imageFile?: File;
  levelOfPriorityId: string;
}

export interface UpdatePostResponse {
  id: string;
  title: string;
  description: string;
  imageUrl: string;
  levelOfPriorityId: string;
  updatedAt: string;
}
