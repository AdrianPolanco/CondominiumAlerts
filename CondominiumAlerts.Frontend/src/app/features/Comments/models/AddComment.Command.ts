export interface AddCommentCommand
{
  text: string;
  ImageFile?: File | null;
  imageUrl?: string;    
  previewUrl?: string;
}
