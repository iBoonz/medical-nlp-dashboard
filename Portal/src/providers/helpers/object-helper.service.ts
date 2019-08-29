export class ObjectHelperService {

  hasValue(obj: any): boolean {
    return obj !== null && obj !== undefined;
  }

  hasValueAndElements(obj: any[]): boolean {
    return this.hasValue(obj) && obj.length > 0;
  }

}
