import {SharedFormField} from './shared-form-field.interface';
import {Feedback} from './feedback.interface';

export interface SharedForm {
  fields: SharedFormField[];
  baseButtonLabel: string;
  submittedButtonLabel: string;
  feedback?: Feedback;
}
