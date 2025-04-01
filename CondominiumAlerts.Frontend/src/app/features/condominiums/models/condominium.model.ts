export type Condominium = {
  id: string;
  name: string;
  address: string;
  imageUrl: string | null;
  code: string;
  token: string;
  tokenExpirationDate: Date;
  amountOfUsers: number;
}