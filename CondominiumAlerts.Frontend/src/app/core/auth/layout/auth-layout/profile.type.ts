export type Profile = PartialProfile & { email: string, profilePictureUrl: string; }

export type PartialProfile = {
  id: string;
  name: string;
  lastname: string;
  username: string;
  address: {
    street?: string;
    city?: string;
    postalCode?: string;
  };
}

export type EditProfileResponse = {
  isSuccess: boolean;
  data: any;
}
