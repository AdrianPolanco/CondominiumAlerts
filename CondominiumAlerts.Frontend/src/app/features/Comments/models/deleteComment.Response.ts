export interface DeleteCommentResponse {
  id: string;
  text: string;
  imageUrl: string | null;
  postId: string;
  userId: string;
  updatedAt: Date;
}
