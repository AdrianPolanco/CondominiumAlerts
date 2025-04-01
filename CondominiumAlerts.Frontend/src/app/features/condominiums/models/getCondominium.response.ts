export interface GetCondominiumResponse {
  id: string;
  name: string;
  address: string;
  imageUrl: string;
  code: string;
  token: string;
  tokenExpirationDate: Date;
  AmountOfUsers: number;
}
