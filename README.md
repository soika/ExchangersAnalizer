## Install SDK
* Windows
    * https://www.microsoft.com/net/learn/get-started/windows
* Linux
    * https://www.microsoft.com/net/learn/get-started/linux

## Check SDK
Open terminal and run the command:
```sh
dotnet --version
```
## Run & Build
1. Clone the repository
```sh
https://github.com/soika/ExchangersAnalizer
```
2. Go to your clone folder:
```sh
cd .../PATH_TO_FOLDER/ExchangersAnalizer
```
3. Restore packages: 
```sh
dotnet restore
```
4. For *Run* with appsettings.Development.json:
```sh
dotnet run -c Debug
```
5. For *Publish* appsettings.Production.json:
```sh
dotnet punlish -c Release
```
or
```sh
dotnet publish -c Release -o E:\Workspace\Projects\CSharp\Publish\ExchangersAnalizer
```
## Run publish files:
Go to publish folder and run command:
```sh
dotnet ExchangersAnalizer.dll
```
## Run as windows service
Coming soon
