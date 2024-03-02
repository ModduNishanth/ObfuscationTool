// export const environment = {
//   baseUrl :"https://kalpitaobfuscation.azurewebsites.net/api/",
//   production: true
// };

// export const environment = {
//   baseUrl :"https://kalpitaobfuscation.azurewebsites.net/api/",
//   apibaseUrl: 'https://kalpitaobfuscation.azurewebsites.net/api/',
//   production: true,
//   clientId: 'a0bacaaa-a789-4cb7-8840-178e7549fcac',
//   redirectUri: 'https://kalpitaobfuscation.azurewebsites.net',
//   tenantId: '8049d4ef-045b-4505-8745-7bca3a5691a3',
//   authority: 'https://login.microsoftonline.com/8049d4ef-045b-4505-8745-7bca3a5691a3',
//   postLogoutRedirectUri: 'https://kalpitaobfuscation.azurewebsites.net/',
//   scopes: ['openid', 'profile'],
//   timeout: 50000,
// };

export const environment = {
  baseUrl :'$$baseUrl$$/',
  apibaseUrl: '$$baseUrl$$/',
  production: true,
  clientId: '$$clientId$$',
  redirectUri: '$$baseUrl$$',
  tenantId: '8049d4ef-045b-4505-8745-7bca3a5691a3',
  authority: 'https://login.microsoftonline.com/8049d4ef-045b-4505-8745-7bca3a5691a3',
  postLogoutRedirectUrl: '$$baseUrl$$/logout',
  scopes: ['openid', 'profile']
};