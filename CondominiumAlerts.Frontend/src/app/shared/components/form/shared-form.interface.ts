import {SharedFormField} from './shared-form-field.interface';
import {Feedback} from './feedback.interface';
import { ValidatorFn } from '@angular/forms';

export interface SharedForm {
  fields: SharedFormField[];
  baseButtonLabel: string;
  submittedButtonLabel: string;
  feedback?: Feedback;
  formValidators?: ValidatorFn[];
}
