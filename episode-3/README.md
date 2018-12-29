---
layout: post
author: gareth
category: blog
tags: [how-to,dotnet-core,beginners,api]
excerpt_separator: <!--more-->
series: ASP.NET Core Web API Episodes
---

## Previously on Decoding

In [Episode 2]({{ site.baseurl }}{% post_url 2018-12-19-Episode-2-JSON-API-Dotnet-Core-Docker---Project-Structure %}) I set up the project directory structure. Today's episode is **Docker Part 1: DockerFiles**. <!--more-->

## Lets talk about Docker

Firstly before we get into how to set up Docker, I want to get into bigging the big D up! Apart from *Visual Studio Code* and *WSL*, no other technology has transformed how I develop code in 2018. Yes there are pain points, but once you get through those, then your development experience starts (i dare say it) become a little more blissful. 

I can now pull down pre-developed images of applications, databases, load balancers, webs servers, SDKs build tools, pixie dust (nic: testing you are still reading!), and use them like lego bricks, to build a system, *without* installing the binaries on my machine.

Take this tutorial for example, apart from the .NET SDK installed for the IDE, *MongoDB*, *nginx* and the *dotnet runtime* are all installed via images, for which I can build and throw away when finished. No installation needed on my local development (host) machine. Even SQL Server runs on Linux now, and has a Image to pull down.

This Lego Brick approach to building software now means I dont need to worry about installing all the tools on my machine, configure those tools and un-install when finished. I can package it all up in a *Dockerfile* to use now, later or even on another machine, without polluting my host machine and slowing it down.

### What is Docker?

Docker is product that utilises a technology *containers*, which have been around in Linux for a while. Containers are isolated/sandboxed processes, which only use the bare minimum binaries needed to run the application *within* the container, including the file system. Unlike *Virtual Machine* which run a whole operating system, and the bloat around it.

Now as I was writing this post, [Dave Swersky](https://twitter.com/dswersky), wrote an awesome post on [What is Docker, and why is it so popular?](https://dev.to/raygun/what-is-docker-and-why-is-it-so-popular-45c7). Check that out.

## Dockerfile

If this is the first time you have used Docker, or you do not have it installed, check out dockers [getting started](https://www.docker.com/get-started) to start your journey. You will need to sign up to Docker Hub(https://hub.docker.com/) as well. Think as hub as the repository for Container Images you can access. Not just Dockers, but the communities as a whole. You may here the term *Container Registry* for the Hub as well (You can set up your own private Registries on Azure for example).

So a couple of commands and terminology we need to clear up, which are useful now before moving forward.

### Help

So the `docker` command has a nice help facility, so if you want to see a list of docker commands:

```bash
docker --help
```

### Images

A image is the package that includes everything needed to run your application, from the environment, code and configuration. You normally use other companies/communities/developers pre-built images from the hub or *base images*, but you can [create your own](https://docs.docker.com/develop/develop-images/baseimages/) just as easily.

If you want to see which images you have on your host machine (your development PC/Mac), then run the following command:

```bash
docker images ls
```

If this is all new, then nothing will be listed currently, but when we start building our *Dockerfile*, we will come back to the output on what this means.

### Containers

The container is your image *running* in a discrete process. You can have multiple containers running on your host machine. Again there is a command to see what containers are running:

```bash
docker container ls
```

You can also use the short cut (One I personally use a lot):

```bash
docker ps
```

## Develop first Dockerfile

What is a Dockerfile then? Here is a good description from Docker [themselves](https://docs.docker.com/engine/reference/builder/#usage):

> Docker can build images automatically by reading the instructions from a Dockerfile. A Dockerfile is a text
> document that contains all the commands a user could call on the command line to assemble an image. Using docker
> build users can create an automated build that executes several command-line instructions in succession.

To begin with, we will build our first Dockerfile to:

- Compile our .NET Core Web API for the BookStore.
- Run our .NET Core Web API.

We call *Dockerfiles* that do more than one thing, *multi-stage* builds. They allow us to *build* and *run* without maintaining separate Dockerfiles (which was the case once upon a time). Because I don't learn this stuff on my own, let me link to [Alex Ellis](https://blog.alexellis.io/mutli-stage-docker-builds/) talking more about this.

**Future Post:** While I'm chatting about Alex, in the future I will be refactoring this **BookStore.App** to use [OpenFaaS](https://www.openfaas.com/) an open source serverless architecture that *isn't* coupled to one particular cloud. I'm excited about this, as I have wanted to use this tech for a while now, I even have the Raspberry Pis to cluster.....

So, back to the matter at hand. Firstly we need to *change directory* to `src/api` directory we set up in [Episode 2]({{ site.baseurl }}{% post_url 2018-12-19-Episode-2-JSON-API-Dotnet-Core-Docker---Project-Structure %}). Just to recap we set up the following:

```
    .
    ├── src
    |   ├── api
    |       |
    |       ├── BookStore.WebApi.csproj
    ├── tests
    |   ├── integration
    |   ├── unit
    |       ├── BookStore.Tests.csproj
    ├── docker
```

Then create a file called `Dockerfile` in the root of `api`:

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

Add the following to the file:

```dockerfile
FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app
COPY BookStore.WebApi.csproj .
COPY . .
RUN dotnet publish -c Release -o /publish

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime-env
WORKDIR /publish
EXPOSE 5000
COPY --from=build-env /publish .
ENTRYPOINT [ "dotnet","BookStore.WebApi.dll" ]
```

If this is new to you, then let me explain the format on this file and what it means.

### FROM

This statement sets the *base image* created by Microsoft (or any organisation who has created an base image) and creates a new build stage (each `FROM` creates a new stage). These images will live on a public repository like *hub.docker.com*. So make sure you have logged in via *Docker Desktop* otherwise the first build step will fail.

Having multiple `FROM` denotes a multi-stage Dockerfile.

- [microsoft/dotnet:2.2-sdk](https://hub.docker.com/r/microsoft/dotnet/) - SDK image. This base image is designed to allow you to build/publish using the *dotnet CLI*. As you can see I'm using the 2.2 .NET Core Version (denoted the common tag name of **:2.2-sdk**).

- [microsoft/dotnet:2.2-aspnetcore-runtime](https://hub.docker.com/r/microsoft/dotnet/) - Runtime image. This base image is designed to run a ASP.NET Core application using the *dotnet runtime*. Again it follows the same format with denoting the dotnet version by the common tag of **:2.2-aspnetcore-runtime**.

### WORKDIR

This statement will create a directory for the following statements will work in. If the directory exists, then the directory is just set to the value in `WORKDIR`. In our case `/app` is created by the command and `/publish` is set to (as publish is created in the `dotnet publish`).

### COPY

This statement will copy the files specified *into* the containers file system (normally into the file system specified in `WORKDIR`) from the host machine of the directory where the Dockerfile is placed. So in our instance, the `../src/api` directory.

**Note:** We have two `COPY` commands in the build stage of our Dockerfile context. We do this for the **BookStore.WebApi.csproj** so that we have a build cache entry for this file, so if it hasn't changed, we dont copy it on every build (reduces build times).  Also `COPY . .` will copy everything else.

```Dockerfile
COPY BookStore.WebApi.csproj .
COPY . .
```

Our 2nd stage actually copies the output from `build-env` **/publish** directory into the next stage.

### RUN

This statement will run a set of commands that will create another layer in your image and commit the results, which will form part of the container. It allows you to set up your container before running it.

Our example is the `dotnet publish` will be `RUN` as part of the `docker build` instruction set, creating the binaries to use when we `dotnet` using the published `.dll`.

### ENTRYPOINT

This statement defines what is run when you `docker run` and start the container. Our `ENTRYPOINT` runs the `dotnet` command using our published `.dll`. You can pass more parameters into the `ENTRYPOINT` command via `docker run`.

Our multi-stage Dockerfile builds our source code in the first stage and copies the published output so that it is run using the *.NET Core Runetime*.

## Build the Image

Now we have a Dockerfile, we can build this file into an *image* which will be stored locally on our host. Make sure you are on the commandline of choice, and change directory to `../src/api` and run the following command:

```bash
docker build -t garfbradaz/ep3-api .
```

This command will run a `docker build`. The "." signifies you want to use the *Dockerfile* in the current directory and `-t garfbradaz/ep3-api` is a parameter that will build an Image with a **[-]t**ag name of *garfbradaz/ep3-api*. You can replace this string with anything you want.

The first time you run this will take a while, because there will be no *cache entries* from previous builds for your layers. You can also see all the layers that are built from the Microsoft *base images* which are made up of layers also:

![docker-net-layers](/assets/img/posts/docker-net-layers.png)

If you run a `docker images` again you will now see your image has been built (plus the .NET Core Images):

![dotnet-net-images](/assets/img/posts/dotnet-net-images.png)

### Run the Image as a Container

So, we have created a *Dockerfile* that includes the steps to build an image, which includes the resources needed to run the application within a *container*. This is the magic, as we can run this image on any OS that supports Docker itself (If we are building *Linux* containers).

So the following will run your container as isolated/sandboxed process:

```bash
docker run --env ASPNETCORE_ENVIRONMENT=Development --env ASPNETCORE_URLS=http://+:5000 -p 5000:5000 -t --rm -it  garfbradaz/ep3-api
```

This command will run your Container interactively (`-it`), and override some environment variables (`--env`); overriding port mapping to 5000 (`-p`) and clean up the container with finished (`--rm`).

I use Powershell to validate the container is running fine, but you can use *postman* (we explore setting this up in another chapter).

```powershell
Invoke-RestMethod -Uri http://localhost:5000/api/values -Method 'Get'
```

This should fail with a 401 status, because we set up the `[Authorize]` attribute against the `ValuesController.cs`.  But at least this proves we are hitting the Web API running using Kestrel within the Container process.

### Next time

We will be exploring `docker compose` to allow us to run and knit together multiple containers together so we can interact with MongoDB for example. 


