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
4. For *Run*:
```sh
dotnet run -c Debug
```
5. For *Publish*:
```sh
dotnet punlish
```
## Run as windows service

After excuting publish command go to the published folder and find the .dll: 
