export interface AddCondominiumResponse {
  isSuccess: boolean;
  data?: {
    id: string;
    name: string;
    address: string;
    imageUrl: string;
  };
  errors?: any[];
}
