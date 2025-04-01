//Tipo para el endpoint de obtener condominios por usuario api/condominium/GetCondominiumsJoinedByUser?UserId={UserId}
export interface GetCondominiumsJoinedByUserResponse {
  id: string;
  name: string;
  address: string;
  imageUrl: string;
}
