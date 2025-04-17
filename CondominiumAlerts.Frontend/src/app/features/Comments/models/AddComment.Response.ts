export interface AddCommentResponse
{
  isSuccess: boolean;
  data?: {
    id: string;
    text: string;
    imageUrl: string;
    postid: string;
    userid: string;
    CreatedAt: string;
    UpdatedAt: string;
  };
}
