nswag openapi2csclient /Input:swagger.json /Output:RaidExtractor/AccountDumpClient.cs /namespace:RaidExtractor
nswag openapi2tsclient /Input:swagger.json /Output:Website/ClientApp/src/app/shared/clients.ts /Template:Angular /UseSingletonProvider:true /RxJsVersion:6.0 /InjectionTokenType:InjectionToken
