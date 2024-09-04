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

TODO: Anti bot mitigation. Publix is blocking automated login, find correct syntax for [these](https://www.zenrows.com/blog/selenium-avoid-bot-detection#disable-automation-indicator-webdriver-flags) experimental options in [CSharp](https://stackoverflow.com/a/63624756) Selenium
