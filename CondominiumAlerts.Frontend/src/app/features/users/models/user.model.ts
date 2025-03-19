export type User = {
    id: string,
    name: string,
    lastName: string
}

export interface GetCondominiumsUsersCommand{
    condominiumId: string;
  }
  
  export interface GetCondominiumsUsersResponse{
    id: string,
    profilePictureUrl: string,
    fullName: string,
    email: string,
  }


  