import { NgxLoggerLevel } from 'ngx-logger';

// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  environmentName: 'Dev',
  domain : '.client1.com',
  appUrl: 'http://localhost:4200/',
  apiUrl: 'https://localhost:5001/api/',
  errorPageUrl: 'http://localhost:4200/',
  logLevel: NgxLoggerLevel.TRACE,
  serverLogLevel: NgxLoggerLevel.ERROR,
  serverLoggingUrl: 'https://localhost:5001/api/logs',
  msalConfig: {
    tenant: 'https://login.microsoftonline.com/4b566686-e90a-40e7-b13f-8e6a6c7bd03b',
    clientId: '3d547642-44b3-4822-a05a-4c7e7345f241',
    redirectUri: 'http://localhost:4200/',
    postLogoutRedirectUri: 'http://localhost:4200/'
  },
  scope: 'api://53a06f48-7f82-486a-9fa0-e703ac608ec3/PipScope',
  session: {
    idleTimeout: 1800, // 1800 seconds = 30 minutes
    timeout: 60, // 60 seconds
    keepaliveInterval: 15 // 15 seconds
  }
};
