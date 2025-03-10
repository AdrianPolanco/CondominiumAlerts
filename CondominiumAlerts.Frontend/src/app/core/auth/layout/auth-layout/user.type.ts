import {Profile} from './profile.type';

export type User = Profile & { createdAt: Date }

export type UserData = {
  isSuccess: boolean;
  data: User;
}
