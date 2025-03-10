import { FileSelectEvent } from "primeng/fileupload";

export type SharedFormField = SharedFormFieldFile | SharedFormFieldCommon

export interface SharedFormFieldFile extends SharedFormFieldBase {
    type: 'file';
    onFileSelect: (event: FileSelectEvent) => void;
    filetype: ImagesTypes;
}

export type  ImagesTypes = 'image/png' | 'image/jpeg' | 'image/webp' | 'image/*';

export interface SharedFormFieldCommon extends SharedFormFieldBase {
    type: 'text' | 'number' | 'textarea' | 'select' | 'checkbox' | 'password' | 'phone';
}

export interface SharedFormFieldBase {
    name: string;
    label: string;
    options?: { label: string; value: any }[];
    defaultValue?: any;
    validators?: any[];
    errorMessages?: { [key: string]: string };
    icon?: string;
    showFormErrors?: boolean;
    disabled?: boolean;
  }
