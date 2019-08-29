import { ConvertableFromApi } from './api/convertable-from-api.model';
import { Patient } from 'models';

export class User extends ConvertableFromApi<User> {
  id: number;
  email: string;
  name: string;

  constructor(data?: any) {
    super(data);
  }

  readFromObject(): void {
    this.id = this.objectFromApi.id;
    this.email = this.objectFromApi.email;
    this.name = this.objectFromApi.name;
  }

  createNew(element: any): User {
    return new User(element);
  }
}

