export interface AddCondominiumCommand {
  name: string;
  address: string;
  imageFile: File;
  creatorUserId: string | undefined;
}
