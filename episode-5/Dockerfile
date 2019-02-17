FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

COPY src/api/BookStoreApp.WebApi.csproj ./src/api/
RUN dotnet restore ./src/api/BookStoreApp.WebApi.csproj
COPY tests/unit/BookStore.Tests.csproj ./tests/unit/
RUN dotnet restore ./tests/unit/BookStore.Tests.csproj

COPY . .

RUN dotnet test tests/unit/BookStore.Tests.csproj
RUN dotnet publish src/api/BookStoreApp.WebApi.csproj -o /publish

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime-env
WORKDIR /publish
COPY --from=build-env /publish .
ENTRYPOINT [ "dotnet","BookStoreApp.WebApi.dll" ]