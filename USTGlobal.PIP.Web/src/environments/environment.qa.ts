import { NgxLoggerLevel } from 'ngx-logger';

// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  environmentName: 'Qa',
  domain : '.xpanxion.co.in',
  appUrl: 'https://xipl0515.xpanxion.co.in/',
  apiUrl: 'https://xipl0515.xpanxion.co.in/pipqaapi/api/',
  errorPageUrl: 'https://xipl0515.xpanxion.co.in/',
  logLevel: NgxLoggerLevel.OFF,
  serverLogLevel: NgxLoggerLevel.ERROR,
  serverLoggingUrl: 'https://xipl0515.xpanxion.co.in/pipqaapi/api/logs',
  msalConfig: {
    tenant: 'https://login.microsoftonline.com/4b566686-e90a-40e7-b13f-8e6a6c7bd03b',
    clientId: '7164b5d8-f1ff-4cd8-a913-c8c3b59c774b',
    redirectUri: 'https://xipl0515.xpanxion.co.in/',
    postLogoutRedirectUri: 'https://xipl0515.xpanxion.co.in/'
  },
  scope: 'api://4cb85146-ec4e-4a6f-b65e-a95bfe3da6b3/PipScope',
  session: {
    idleTimeout: 1800, // 1800 seconds = 30 minutes
    timeout: 60, // 60 seconds
    keepaliveInterval: 15 // 15 seconds
  }
};
