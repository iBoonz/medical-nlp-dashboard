export abstract class ConvertableFromApi<T> {

  readonly objectFromApi: any;
  abstract readFromObject(): void;
  abstract createNew(element: any): T;

  constructor(data?: any) {
    if (data === null || data === undefined) {
      return;
    }

    this.objectFromApi = data;
    this.readFromObject();
  }
}
