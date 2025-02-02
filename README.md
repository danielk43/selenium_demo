## Usage
This project builds a Docker image with dotnet sdk 9.0 and latest stable Chromium driver version  
Run the image with these .cs and .csproj files mounted to compile and execute NUnit framework testing  

Pull image from Dockerhub
```
docker pull danielk43/selenium_dotnet:latest
```
Or build it and run
Environment variables can be skipped
```
docker build --tag selenium_dotnet .
docker run --rm \
--env LOGIN_USER=<user> \
--env LOGIN_PASS=<password> \
--env SCREEN_WIDTH=<screen_width> \
--env SCREEN_HEIGHT=<screen_height> \
--volume $PWD:/data \
selenium_dotnet
```
The default entrypoint cmd is `dotnet test -v n` but any cmd can be passed as an arg to `docker run` which will execute with the non-root user   

Testing will change ownership recursively in the current directory. This is so the non-root user has write access  
When finished, clean up the environment:  
```
sudo chown -R ${USER}: .
git clean -ffdx
```

TODO: implement [these](https://piprogramming.org/articles/How-to-make-Selenium-undetectable-and-stealth--7-Ways-to-hide-your-Bot-Automation-from-Detection-0000000017.html)

## Debug
For C#/F#/SQL debugging, run a container and install the dotnet-repl shell  
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
https://peter.sh/experiments/chromium-command-line-switches  
https://stackoverflow.com/questions/2883576  
https://stackoverflow.com/questions/4291912  
https://stackoverflow.com/questions/4603911  
https://stackoverflow.com/questions/6229769  
https://stackoverflow.com/questions/15310110  
https://stackoverflow.com/questions/23679283  
https://stackoverflow.com/questions/55012929  
https://stackoverflow.com/questions/58651526  
https://www.automatetheplanet.com/selenium-webdriver-csharp-cheat-sheet  
https://www.lambdatest.com/blog/findelements-in-selenium-c-sharp  
https://www.lambdatest.com/blog/nunit-testing-tutorial-for-selenium-csharp  
https://www.nuget.org/packages  
https://www.selenium.dev/selenium/docs/api/dotnet/index.html  

