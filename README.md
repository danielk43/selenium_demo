## Usage
This project builds a Docker image with dotnet sdk 8.0 and the latest stable Chrome for Testing version  
Running the image with these .cs and .csproj files mounted will build the project and execute NUnit framework testing

Logging into the Publix website is not supported in the curent config, so build args can be skipped  

```
docker build -t chrome-dotnet --build-args LOGIN_USER=<user> --build-arg LOGIN_PASS=<password> .
docker run --rm -it -v $PWD:/data chrome-dotnet dotnet test -v m
```

This is a demo project and is not expected to be scalable or meant for production use  
For example, credentials should never be built into an image but provided by CICD, K8s, Secrets Manager, etc  

TODO: Anti bot mitigation. Publix is blocking automated login; implement [these](https://piprogramming.org/articles/How-to-make-Selenium-undetectable-and-stealth--7-Ways-to-hide-your-Bot-Automation-from-Detection-0000000017.html) suggestions, look for others. Fix non-headless mode.

## Debug
For C#/F# debugging, run a container and install the dotnet-repl shell
```
docker run -it -v $PWD:/data chrome-dotnet bash
dotnet tool install --tool-path /usr/local/bin dotnet-repl
dotnet repl
```
See the [project](https://github.com/jonsequitur/dotnet-repl) for more documentation

## Additional Credits / Useful Links
https://github.com/devpabloassis/seleniumdotnetcore  
https://github.com/Hudrolax/uc-docker  
https://github.com/janex-PL/DotnetSeleniumDockerRuntimeExample  
https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl  
https://learn.microsoft.com/en-us/dotnet/core/install/linux-debian  
https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet  
https://stackoverflow.com/questions/23679283  
https://stackoverflow.com/questions/2883576  
https://stackoverflow.com/questions/4291912  
https://stackoverflow.com/questions/4603911  
https://stackoverflow.com/questions/58651526  
https://stackoverflow.com/questions/6229769  
https://www.automatetheplanet.com/selenium-webdriver-csharp-cheat-sheet  
https://www.lambdatest.com/blog/findelements-in-selenium-c-sharp  
https://www.lambdatest.com/blog/nunit-testing-tutorial-for-selenium-csharp  
https://www.nuget.org/packages  
https://www.selenium.dev/selenium/docs/api/dotnet/index.html  
