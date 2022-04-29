import { NgxLoggerLevel } from 'ngx-logger';

// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  environmentName: 'Demo',
  domain : '.xpanxion.co.in',
  appUrl: 'https://xipl0515.xpanxion.co.in:555/',
  apiUrl: 'https://xipl0515.xpanxion.co.in:555/pipbctapi/api/',
  errorPageUrl: 'https://xipl0515.xpanxion.co.in:555/',
  logLevel: NgxLoggerLevel.OFF,
  serverLogLevel: NgxLoggerLevel.ERROR,
  serverLoggingUrl: 'https://pipdemo.com/pipdemoapi/api/logs',
  msalConfig: {
    tenant: 'https://login.microsoftonline.com/4b566686-e90a-40e7-b13f-8e6a6c7bd03b',
    clientId: '717383e9-a052-4a5e-b3d5-e308d9312db7',
    redirectUri: 'https://pipdemo.com/',
    postLogoutRedirectUri: 'https://pipdemo.com/'
  },
  scope: '',
  session: {
    idleTimeout: 1800, // 1800 seconds = 30 minutes
    timeout: 60, // 60 seconds
    keepaliveInterval: 15 // 15 seconds
  }
};
