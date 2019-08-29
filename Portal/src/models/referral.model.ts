import { Patient, User } from 'models';

export class Referral {
  patient: Patient;
  user: User;
  fileNames: string[];
}

