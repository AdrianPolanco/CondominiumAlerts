import { User } from "../../core/auth/layout/auth-layout/user.type";
import { Condominium } from "../condominiums/models/condominium.model";

export type PartialEvent = {
    id: string;
    title: string;
    start: Date;
    end?: Date;
    description?: string;
}

export type CondominiumEvent = PartialEvent & {
    isStarted: boolean;
    isFinished: boolean;
    isToday: boolean;
    createdBy: User;
    condominium: Pick<Condominium, "address" | "id" | "name" | "imageUrl">
    createdAt: Date;
    updatedAt: Date;
}