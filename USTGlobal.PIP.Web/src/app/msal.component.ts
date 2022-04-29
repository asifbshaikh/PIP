import { Component} from '@angular/core';
import { MsalService } from '@azure/msal-angular';

// This component is used only to avoid Angular reload
// when doing acquireTokenSilent()

@Component({
  selector: 'app-root',
  template: 'msal comp',
})
export class MsalComponent {
  constructor(private Msal: MsalService) {
  }
}
