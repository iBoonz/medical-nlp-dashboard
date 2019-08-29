import { Component } from '@angular/core';

// LOCALE
import { registerLocaleData } from '@angular/common';
import localeFr from '@angular/common/locales/fr';
import localeNl from '@angular/common/locales/nl';


// TRANSLATIONS
import { TranslateService } from '@ngx-translate/core';

@Component({
  // tslint:disable-next-line
  selector: 'body',
  template: '<router-outlet></router-outlet>'
})
export class AppComponent {

  constructor(translateService: TranslateService) {

    registerLocaleData(localeFr);
    registerLocaleData(localeNl);

    translateService.addLangs(['en', 'nl']);
    translateService.setDefaultLang('en');

    const browserLang: string = translateService.getBrowserLang();
    translateService.use(browserLang.match(/en|nl/) ? browserLang : 'en');
  }

}
