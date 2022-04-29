import { Configuration, CacheLocation } from 'msal';
import { MsalAngularConfiguration } from '@azure/msal-angular';
import { environment } from '@env';

export const config = {
    auth: {
        clientId: environment.msalConfig.clientId,
        authority: environment.msalConfig.tenant,
        validateAuthority: true,
        redirectUri: environment.appUrl,
        postLogoutRedirectUri: environment.msalConfig.postLogoutRedirectUri,
        navigateToLoginRequestUrl: true
    },
    cache: {
        cacheLocation: 'localStorage'
    },
    scopes: {
        loginRequest: ['openid', 'profile', environment.scope]
    },
    resources: {
        api: {
            resourceUri: environment.apiUrl,
            resourceScope: environment.scope
        }
    },
    unprotectedResources: {
      i18nAssets : {
        enJson: './assets/i18n/en.json'
      }
    }
};

// checks if the app is running on IE
const isIE = window.navigator.userAgent.indexOf('MSIE ') > -1 || window.navigator.userAgent.indexOf('Trident/') > -1;

const protectedResourceMap: [string, string[]][] = [
  [config.resources.api.resourceUri, [config.resources.api.resourceScope]]
];

const unprotectedResources: string[] =
  [config.unprotectedResources.i18nAssets.enJson];

export function MSALConfigFactory(): Configuration {
  return {
    auth: {
      clientId: config.auth.clientId,
      authority: config.auth.authority,
      validateAuthority: true,
      redirectUri: config.auth.redirectUri,
      postLogoutRedirectUri: config.auth.postLogoutRedirectUri,
      navigateToLoginRequestUrl: true,
    },
    cache: {
      cacheLocation: <CacheLocation>config.cache.cacheLocation,
      storeAuthStateInCookie: isIE, // set to true for IE 11
    },
  };
}

export function MSALAngularConfigFactory(): MsalAngularConfiguration {
  return {
    popUp: !isIE,
    consentScopes: [
      config.resources.api.resourceScope,
      config.scopes.loginRequest[0],
      config.scopes.loginRequest[1],
      config.scopes.loginRequest[2]
    ],
    unprotectedResources,
    protectedResourceMap,
    extraQueryParameters: {}
  };
}
