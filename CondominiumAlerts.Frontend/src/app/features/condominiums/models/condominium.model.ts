export interface AddCondominiumCommand {
    name: string;
    address: string;
    imageFile: File;
}


export interface AddCondominiumResponse {
  isSuccess: boolean;
  data?: {
    id: string;
    name: string;
    address: string;
    imageUrl: string;
  },
  errors?: any[]
}