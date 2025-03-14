export type RegisterUserRequest = {
  username: string;
  name: string;
  lastname: string;
  email: string;
  password: string;
  confirmPassword: string;
  profilePictureUrl: string | null;
}
