import { User } from "../../features/users/models/user.model";

export function isUser(data: any): data is User{
    return (data as User).name !== undefined;
}