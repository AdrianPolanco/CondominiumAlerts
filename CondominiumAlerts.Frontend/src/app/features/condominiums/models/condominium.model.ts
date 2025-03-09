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

export interface getCondominiumCommand{
  condominiumId: string;
}

export interface getCondominiumResponse{
   id: string,
   name: string,
   address: string,
   imageUrl: string,
   code: string,
   token: string,
   tokenExpirationDate: Date,
   AmountOfUsers: number
}

export interface getCondominiumsJoinedByUserCommand{
  userId: string;
}

export interface getCondominiumsJoinedByUserResponse{
   id: string,
   name: string,
   address: string,
   imageUrl: string,
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
