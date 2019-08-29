export class StringHelperService {

  NotNullOrWhiteSpace(obj: string): boolean {
    return obj !== null && obj !== undefined && obj !== '';
  }

}
