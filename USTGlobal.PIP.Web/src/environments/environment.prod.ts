import { NgxLoggerLevel } from 'ngx-logger';

// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: true,
  environmentName: 'Production',
  domain : '.PIPdigital.ust-global.com',
  appUrl: 'https://PIPdigital.ust-global.com/',
  apiUrl: 'https://PIPdigital.ust-global.com/pipapi/api/',
  errorPageUrl: 'https://PIPdigital.ust-global.com/',
  logLevel: NgxLoggerLevel.OFF,
  serverLogLevel: NgxLoggerLevel.ERROR,
  serverLoggingUrl: 'https://PIPdigital.ust-global.com/pipapi/api/logs',
  msalConfig: {
    tenant: 'https://login.microsoftonline.com/a4431f4b-c207-4733-9530-34c08a9b2b8d',
    clientId: '88c05158-2a4e-49a9-927c-c3a9d2744b54',
    redirectUri: 'https://PIPdigital.ust-global.com/',
    postLogoutRedirectUri: 'https://PIPdigital.ust-global.com/'
  },
  scope: 'api://34d80b50-f9f2-4cb1-8381-1dbf08597f04/PipwebRead',
  session: {
    idleTimeout: 1800, // 1800 seconds = 30 minutes
    timeout: 60, // 60 seconds
    keepaliveInterval: 15 // 15 seconds
  }
};

