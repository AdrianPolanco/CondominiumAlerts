import { User, UserData } from "../../../core/auth/layout/auth-layout/user.type";
import { Condominium } from "../../../features/condominiums/models/condominium.model";

export type ChatOptions = {
    type: "condominium" | "user";
    condominium: Pick<Condominium, 'id' | 'name' | 'imageUrl'| 'address'> | null,
    user: User | null
}