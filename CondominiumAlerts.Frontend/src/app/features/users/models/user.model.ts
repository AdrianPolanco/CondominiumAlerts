export type User = {
    id: string,
    name: string,
    lastName: string
}

export interface GetCondominiumsUsersCommand{
    condominiumId: string;
  }
  
  export interface GetCondominiumsUsersResponse{
    id: string;
    fullName: string;    // Contendrá el nombre real (que viene de profilePictureUrl)
    email: string;       // Contendrá el email (que viene de fullName)
    profilePictureUrl: string;
  }


