import { ConvertableFromApi } from './api/convertable-from-api.model';
import { User } from './user.model';
import { UrlService } from 'providers/http/base/url.service';

export class Patient extends ConvertableFromApi<Patient> {
  id: number;
  email: string;
  name: string;
  language: Language;
  gender: Gender;
  dateOfBirth: Date;
  nihii: number;
  remarks: string;
  constructor(data?: any) {
    super(data);
  }

  readFromObject(): void {
    this.id = this.objectFromApi.id;
    this.email = this.objectFromApi.email;
    this.name = this.objectFromApi.name;
    this.language = this.objectFromApi.language;
    this.gender = this.objectFromApi.gender;
    this.dateOfBirth = new Date(this.objectFromApi.dateOfBirth);
    this.remarks = this.objectFromApi.remarks;
    this.nihii = this.objectFromApi.nihii;
  }

  createNew(element: any): Patient {
    return new Patient(element);
  }
}

