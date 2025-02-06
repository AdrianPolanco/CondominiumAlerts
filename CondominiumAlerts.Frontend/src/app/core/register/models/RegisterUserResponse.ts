export type RegisterUserResponse = {
  isSuccess: boolean;
  data?: {
    id: string;
    name: string;
    lastname: string;
    email: string;
    createdAt: Date;
    phone: {
      number: string
    }
  },
  errors?: any[]
}
