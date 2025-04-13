import { User } from "../../core/auth/layout/auth-layout/user.type";
import { Condominium } from "../condominiums/models/condominium.model";

export type PartialEvent = {
    id: string;
    title: string;
    start: Date;
    end: Date;
    description?: string;
}

export type CondominiumEvent = PartialEvent & {
    isStarted: boolean;
    isFinished: boolean;
    isToday: boolean;
    createdBy: CondominiumEventSubscriber;
    suscribers: CondominiumEventSubscriber[]
    createdAt: Date;
    updatedAt: Date;
    isSubscribed: boolean;
}

type CondominiumEventSubscriber = {
  id: string;
  name: string;
  lastname: string;
  profilePictureUrl: string | null;
  username: string;
}
