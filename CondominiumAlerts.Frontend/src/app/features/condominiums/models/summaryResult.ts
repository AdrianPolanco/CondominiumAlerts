import { Condominium } from "./condominium.model";

export type SummaryResult = {
    id: string;
    content: string;
    condominiumId: string;
    condominium: Condominium;
    triggeredBy: string;
    createdAt: string;
    user: {
        id: string;
        username: string;
        email: string;
        name: string;
        lastname: string;
        profilePicture: string;
        
    }
}