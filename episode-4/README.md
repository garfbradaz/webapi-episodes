---
layout: post
author: gareth
category: blog
tags: [how-to,dotnet-core,beginners,api,docker-compose,mongodb]
excerpt_separator: <!--more-->
series: ASP.NET Core Web API Episodes
comments: true
title: Episode 4 - JSON API using ASP.NET Core, Docker & MongoDB -  Docker Part II Docker Compose
---
# Previously on Dcoding

In [Episode 3]({{ site.baseurl }}{% post_url 2018-12-29-Episode-3-JSON-API-ASP.NET-Core-Docker--Setting-Up-Docker--DockerFiles %}) I set up our Dockerfiles for creating our *Docker Images* for our BookStore app. This will allow us to rapidly test our application as we move forward in later Episodes. Today's episode is **Docker Part 2: Docking Compose**. <!--more-->

## What is Docker Compose?

The [official documentation](https://docs.docker.com/compose/) a good explanation:

> Compose is a tool for defining and running multi-container Docker applications.

Docker compose allows you to define your *services* you need to build and run a full scale application. Think about it, not just your application you need. Depending on the type of application you may need:

- Database of some sort.

- Load Balancer.

- Web Server.

- Other applications / APIs.

You may even want to run your unit/integration testing during your Continuous Integration pipeline (CI). If each of the applications you need as part of the full application.

## BookStore.WebApi Set up

If we take a look at our application for the *BookStore.WebApi*, we can see the following:

![architecture diagram](/assets/img/posts/docker-arch.png)

As you can see from my quickly drawn up diagram (used [Microsoft Whiteboard](https://www.microsoft.com/en-gb/p/microsoft-whiteboard/9mspc6mp8fm4?activetab=pivot:overviewtab)), we have two *services* running in containers, 1 for the application and the other for the database (*MongoDB*), so we can compose these together using `docker-compose`.

## docker-compose.dev.yml

So in  [Episode 2]({{ site.baseurl }}{% post_url 2018-12-19-Episode-2-JSON-API-Dotnet-Core-Docker---Project-Structure %}) we set up the Project Structure, so change directory to the `./docker` directory.

```
    .
    ├── src
    |   ├── api
    |       |
    |       ├── BookStore.WebApi.csproj
    |       ├── Dockerfile
    ├── tests
    |   ├── integration
    |   ├── unit
    |       ├── BookStore.Tests.csproj
    ├── docker
```

Within that directory, you should have two empty YAML files:

- **docker-compose.dev.yml**
- **docker-compose.yml**

We are concentrating on a *Development* environment first, so add the following to `docker-compose.dev.yml`:

```yaml
version: "3"
services:
  webapi:
    image: garfbradaz/bookstoreapi:develop
    container_name: webapi_tutorial_debug
    build:
      args:
        buildconfig: Debug
      context: ../src/api
      dockerfile: Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:5003
    ports:
      - "5003:5003"
    depends_on:
      - mongodb
  mongodb:
    image: mongo:latest
    container_name: mongodb
    ports:
      - "27017:27017"
```

And also, just add the following line to `docker-compose.yml` file so that when we run `docker-compose up` it doesn't fail:

```yaml
version: "3"
```

As in **Episode 3** lets break this *YAML* file down now and what we are declaring. These files are *YAML* and following the normal *YAML* syntax rules around indenting etc.

### `version:`

This is really important. The currently released version (as of December 2018) is **3(.7)**. You only have to include the whole number within the `version` number field within the YAML file. Each *major* upgrade (from 1.x, to 2.x, to 3.x) brings about possible breaking changes, including syntax changes to the YAML structure itself.

Also the version of Compose relates to the version of the released *Docker Engine* so have a good read of the [Compatibility Matrix](https://docs.docker.com/compose/compose-file/compose-versioning/#compatibility-matrix), but usually you pick the latest version.

### `services:`

[Docker services](https://docs.docker.com/get-started/part3/) is where you define each application. So we have two services defined:

- `webapi:` which is our ASP.NET Core *BookStore.WebApi* application.
- `mongodb:` which is the back-end data store, *MongoDB*.

### `image:`

Each service has an `image` defined. Mongo's [`mongo:latest`](https://hub.docker.com/_/mongo) will be pulled directly from *hub.docker.com*.

Our own will be built locally for now (until we publish it later) and its simply called `garfbradaz/bookstoreapi:develop`. Note the *tag* of **develop**. We now have denoted our debug image, and we can add things like symbols etc for debugging purposes.

### 'container_name:'

This is just a nice friendly name for our container. You can see the name when you run `docker ps` after you have run a container.

## Run your applications

Docker compose has a command  'docker compose up', which allows you to (re)build, (re)create and attach containers for the service. You run the command using the following (make sure you are in the `./docker` directory):

```bash
docker-compose -f docker-compose.dev.yml up -d --build
```

This command overrides the file (`-f`) to `docker-compose.dev.yml` and runs `up`. The containers will run in detached mode (`-d`) and run in the background. We will also (re)build the images (`--build`). Because we have a `build:` section in our `docker-compose.dev.yml` file for our code those values will be used. We set the context (`../src/api`) which is the relative directory to the source code we are building (Relative to the `./docker` directory), plus tell `docker-compose` the Dockerfile name.

We also send in some `args` *into* the Dockerfile. Currently we ignore these, but we will be coming back to them later in this post.

Lastly we set some `environment` variables for our application/ASP.NET Core to use. Specifically around setting up a `Development` environment and HTTP URLs.

**Note:** If we dont set these, our application will try and use HTTPS because that is the default now (which is a good thing). Because we haven't set any self-signed developer certificates up yet this will become a bit of a pain. We will do it, but to get up and running, we are switching off HTTPS for now.

We also pull down a **MongoDB** image and start a new database, listening on port 27017. This is the standard port mapping for MongoDB.

This command will also create a default network for your applications to live in. Normally named after the directory `docker-compose` is run with a postfix of **default**. So mine is `docker_default`.

## Check the containers are running

You can now run a `docker ps` on your commandline of choice. You should see  your `webapi_tutorial_debug` and `mongodb` (**Hint:** `container_name` you set in the `docker-compose.dev.yml` file).

![docker ps](/assets/img/posts/docker-ps.png)

## Stop containers

When you have finished you can clear up your containers by running the following, which will stop and remove the containers networks created for this service:

```bash
docker-compose -f docker-compose.dev.yml down
```

## Powershell Scripts (Optional)

I have created two powershell scripts that automate this. You just need to run them in the root of the project:

### Run containers

```powershell
.\run.ps1
```

### Close and clean containers

```powershell
.\clean.ps1
```

Powershell Core is now cross platform as well so you can install Powershell Core and use these scripts on Mac and Linux boxes if you wish.

## Debug Arguments

Previously I mentioned we set a `arg` called `buildconfig:` to `Debug`. We haven't used this so far. But we will now. I use this to build a debug version of our ASP.NET Core *BookStore.WebApi* app, so we can debug into the container using *vsdbg*.

Have a read of my article [Debug .NET Core in Docker]({{ site.baseurl }}{% post_url 2018-12-13-debug-dotnet-core-in-docker %}) about what this is. For this article, change directory to `./src/api` and make sure your `Dockerfile` looks like this:

```docker
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
```

### Next time

Now we have our architecture spun up and ready, we can start building some ASP.NET Core code (using C#) to start creating our Models for our *BookStore.WebApi*. 
