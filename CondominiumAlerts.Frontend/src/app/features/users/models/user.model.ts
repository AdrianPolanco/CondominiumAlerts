export type User = {
    id: string,
    name: string,
    lastName: string
}

export interface getCondominiumsUsersCommand{
    condominiumId: string;
  }
  
  export interface getCondominiumsUsersResponse{
     id: string,
     profilePictureUrl: string,
     fullName: string,
     email: string,
  }
  