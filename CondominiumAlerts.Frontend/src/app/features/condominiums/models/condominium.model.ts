export interface AddCondominiumCommand {
    name: string;
    address: string;
    profilePic: File;
}

export interface JoinCondominiumCommand{
  userId: string;
  condominiumCode: string;
}

export interface JoinCondominiumResponce{
  userId: string;
  CondominiumId: string;
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
