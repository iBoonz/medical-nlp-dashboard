
import { ConvertableFromApi } from '../../models/index';

export class ApiConversionHelperService {

  convertFromApiArray<T extends ConvertableFromApi<T>>(model: T, array: any[]): T[] {
    let list: Array<T> = new Array<T>();
    if (array !== null && array !== undefined && array.length > 0) {
      for (let index = 0; index < array.length; index++) {
        list.push(model.createNew(array[index]));
      }
    }
    return list;
  }

  convertFromApiModel<T extends ConvertableFromApi<T>>(model: T, data: any): T {
    return model.createNew(data);
  }
}
