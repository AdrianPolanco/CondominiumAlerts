export interface SharedFormField {
    name: string;
    label: string;
    type: 'text' | 'number' | 'textarea' | 'select' | 'checkbox' | 'password';
    options?: { label: string; value: any }[];
    validators?: any[];
    errorMessages?: { [key: string]: string };
    icon?: string;
    showFormErrors?: boolean;
  }
