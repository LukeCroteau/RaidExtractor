nswag openapi2csclient /Input:swagger.json /Output:RaidExtractor.Core/AccountDumpClient.cs /namespace:RaidExtractor.Core
nswag openapi2tsclient /Input:swagger.json /Output:Website/ClientApp/src/app/shared/clients.ts /Template:Angular /UseSingletonProvider:true /RxJsVersion:6.0 /InjectionTokenType:InjectionToken
