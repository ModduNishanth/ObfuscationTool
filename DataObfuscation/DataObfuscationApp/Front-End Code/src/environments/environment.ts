// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
//    The list of file replacements can be found in `angular.json`.


//  export const environment = {
//    baseUrl :"https://kalpitaobfuscation.azurewebsites.net/api/",
//    apibaseUrl: 'https://kalpitaobfuscation.azurewebsites/api/',
//    production: false,
//    clientId: 'a0bacaaa-a789-4cb7-8840-178e7549fcac',
//    redirectUri: 'https://kalpitaobfuscation.azurewebsites',
//    tenantId: '8049d4ef-045b-4505-8745-7bca3a5691a3',
//    authority: 'https://login.microsoftonline.com/8049d4ef-045b-4505-8745-7bca3a5691a3',
//    postLogoutRedirectUri: 'https://kalpitaobfuscation.azurewebsites/',
//    scopes: ['openid', 'profile'],
//    timeout: 50000,
//  };


export const environment = {
  baseUrl :"https://localhost:7216/api/",
  apibaseUrl: 'https://localhost:7216/api/',
  production: false,
  clientId: 'a0bacaaa-a789-4cb7-8840-178e7549fcac',
  redirectUri: 'http://localhost:4200',
  tenantId: '8049d4ef-045b-4505-8745-7bca3a5691a3',
  authority: 'https://login.microsoftonline.com/8049d4ef-045b-4505-8745-7bca3a5691a3',
  postLogoutRedirectUri: 'http://localhost:4200/',
  scopes: ['openid', 'profile'],
};
