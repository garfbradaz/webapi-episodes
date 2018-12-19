---
layout: post
author: gareth
category: blog
tags: [how-to,dotnet-core,beginners,api]
excerpt_separator: <!--more-->
---
## Previously on Decoding

In [Episode 1]({{ site.baseurl }}{% post_url 2018-12-18-Episode-1-json-api-dotnet-core-docker %}) I set the scene for what this series will contain. Today's episode is simply on setting your *directory* structure and creating the relevant projects.

## Directory Structure

Firstly, create a directory structure for your *Source (src)* where our **dotnet projects** will be saved, *tests* will hold our **Unit** and **Integration** tests and lastly, *docker* which will hold our **docker-compose**.

```
    .
    ├── src
    ├── tests
    ├── docker
```

### Src

Change directory to *src* and we will run the following *dotnet* command to new up a Project. Before we move on any further and do that though, lets talk about *dotnet new* for people who have not used it.

This command will create a dotnet project based off a default project template. You get a bunch of defaults with the SDK ([and you can create your own](https://docs.microsoft.com/en-us/dotnet/core/tutorials/create-custom-template). Run the following:

```bash
dotnet new --help
```

This will give you a list of available templates you can use:

![templates](/assets/posts/img/ep2-dotnet-new.png)

Take note of the *short name* as this is what we will be using next. So now we know what `dotnet new` does, lets create the project:

```bash
dotnet new webapi --auth Individual -o api -n BookStore.WebApi
```

This will create a folder with the name *api* and in it will create a new *webapi* project using authentication (we will explore this more in a later episode). The project will be called *BookStore.WebApi*.

**Tip:** You can do a `dotnet new webapi --help` and see all the available switches for that short name.

Your directory structure should look like this now:

```
    .
    ├── src
    |   ├── api
    |       |
    |       ├── BookStore.WebApi
    ├── tests
    ├── docker
```

### Tests

Change directory to the *tests* folder. Firstly mainly create a *integration* test folder. These will hold our **Postman** tests (again for another episode). Secondly, we need to create our Unit Test project. I will be using *Xunit*, but as you can see, there were a few different projects for testing when you did a `dotnet new --help`.

```bash
dotnet new xunit -o unit -n BookStore.Tests
```

Again this will create a *xunit* project type within a folder called *unit*. The project name will be *BookStore.Tests*. Within the *integration* folder just add an empty **postman.json** file.

```
    .
    ├── src
    |   ├── api
    |       |
    |       ├── BookStore.WebApi
    ├── tests
    |   ├── integration
    |   ├── unit
    |       ├── BookStore.Tests
    ├── docker
```

### Docker

Change directory to *docker* folder and for now just create two empty files:

> docker-compose.yml
>
> docker-compose.dev.yml

Yes you guessed it, this files will become apparent (and fatter) in a future episode.