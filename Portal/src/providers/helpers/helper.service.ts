import { ObjectHelperService } from './object-helper.service';
import { ApiConversionHelperService } from './api-conversion-helper.service';
import { StringHelperService } from './string-helper.service';

export class Helper {

  static objects: ObjectHelperService = new ObjectHelperService();
  static api: ApiConversionHelperService = new ApiConversionHelperService();
  static strings: StringHelperService = new StringHelperService();

}
