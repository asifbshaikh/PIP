import { NgxLoggerLevel } from 'ngx-logger';

// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: true,
  environmentName: 'UAT',
  domain : '.pipdigital.ustdev.com',
  appUrl: 'https://pipdigital.ustdev.com/',
  apiUrl: 'https://pipdigital.ustdev.com/pipuatapi/api/',
  errorPageUrl: 'https://pipdigital.ustdev.com/',
  logLevel: NgxLoggerLevel.OFF,
  serverLogLevel: NgxLoggerLevel.ERROR,
  serverLoggingUrl: 'https://pipdigital.ustdev.com/pipuatapi/api/logs',
  msalConfig: {
    tenant: 'https://login.microsoftonline.com/a4431f4b-c207-4733-9530-34c08a9b2b8d',
    clientId: 'e04bf2bc-88ed-4b1c-9b28-616688a5ee55',
    redirectUri: 'https://pipdigital.ustdev.com/',
    postLogoutRedirectUri: 'https://pipdigital.ustdev.com/'
  },
  scope: 'api://7d8d9a84-5475-4cab-ae8c-6312a03e2683/PIPScope',
  session: {
    idleTimeout: 1800, // 1800 seconds = 30 minutes
    timeout: 60, // 60 seconds
    keepaliveInterval: 15 // 15 seconds
  }
};
