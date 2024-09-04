## Usage
This project builds a Docker image with dotnet sdk 8.0 and the latest stable Chrome for Testing version
Running the image with these .cs and .csproj files mounted will build the project and execute NUnit framework testing

```
docker build -t chrome-dotnet --build-args LOGIN_USER=<user> --build-arg LOGIN_PASS=<password> .
docker run --rm -it -v $PWD:/data chrome-dotnet dotnet test -v n
```

This is a demo project and is not expected to be scalable or meant for production use

TODO: Debug login, Publix did not make it easy to click
