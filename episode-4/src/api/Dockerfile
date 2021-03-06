FROM microsoft/dotnet:2.2-sdk AS build-env
ARG buildconfig
WORKDIR /app
COPY BookStore.WebApi.csproj .
COPY . .
RUN env
RUN if [ "${buildconfig}" = "Debug" ]; then \
        dotnet publish -o /publish -c Debug; \
    else \
        dotnet publish -o /publish -c Release; \
    fi 

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime-env
ARG buildconfig
ENV DEBIAN_FRONTEND noninteractive
WORKDIR /publish
COPY --from=build-env /publish .
RUN if [ "${buildconfig}" = "Debug" ]; then \
        apt-get update && \
        apt-get install -y --no-install-recommends apt-utils && \
        apt-get install curl unzip procps mongodb -y && \
        curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /publish/vsdbg; \
     else \ 
        echo "*Whistling*"; \
    fi 
ENV DEBIAN_FRONTEND teletype
ENTRYPOINT [ "dotnet","BookStore.WebApi.dll" ]