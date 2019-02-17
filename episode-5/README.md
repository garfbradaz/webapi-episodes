---
layout: post
author: gareth
category: blog
tags: [how-to,dotnetcore,beginners,api,mongodb,aspnetcore,unittests]
excerpt_separator: <!--more-->
series: ASP.NET Core Web API Episodes
comments: true
title: Episode 5 - JSON API using ASP.NET Core, Docker & MongoDB -  Modelling, Controller and Unit Tests Part I - BookStore
---
## Previously on Dcoding

In [Episode 4]({{ site.baseurl }}{% post_url 2018-12-31-Episode-4-JSON-API-ASP.NET-Core-Docker-Compose %}) I set up our `docker-compose` files to allow us to knit together our application and the services it will be use. Today's episode is focusing on **Modelling, Controller and Unit Tests for the BookStore Object**. <!--more-->

Here is a reminder on the sample *User Stories / Epics* for phase 1.

> As a *book store*
> I can *add* our *store* to the database
> *So* we can be accessible
>
> As a *book store*
> I can *add* our *inventory* to our database
> *So* we can expose our inventory
>
> As a *book store*
> We can *update* a books stock level
> *For* an accurate stock level 
>
> As a *API consumer*
> I can look up a *stores address*
> *So* we know where to buy a *book*
>
> As a *API consumer*
> I can look up a *book*
> *So* we can get a *list* of *stores* who sell a
> *book*
>
> As *book store IT Security*
> We can add *API Keys* to the API
> *For* API Consumers to use when querying the API

## Dependency on MongoDB

Our application will ultimately be using *MongoDB* as its back end data store. We need to take a dependency on the MongoDB driver. This will allow us to communicate with and use MongoDB.

Using your shell of choice, change directory to `../src/api` directory. To recap, our directory structure is:

```
    .
    ├── src
    |   ├── api
    |       |
    |       ├── BookStoreApp.WebApi.csproj
    |       ├── Dockerfile
    ├── tests
    |   ├── integration
    |   ├── unit
    |       ├── BookStore.Tests.csproj
    ├── docker
```

**Note:** Something has changed, can you guess what? Yep that's right, I have renamed the `.csproj` to `BookStoreApp.WebApi.csproj` instead of `BookStore.WebApi.csproj`.

Now run the following command, which will add the latest Nuget package to your api project.

```bash
dotnet add package  MongoDB.Driver
```

We will be returning to MongoDB once we have the unit test infrastructure set up in the next episode.

## BookStore
### Modelling

The first of our models (the *M* in **M**VC), will represent our **BookStore**. A simple [Plain old C# (POCO)](https://en.wikipedia.org/wiki/Plain_old_CLR_object) to represent this:

```c#
    /// <summary>
    /// BookStore POCO.
    /// </summary>
    public class BookStore
    {
        [BsonId]
        public ObjectId  Id {get; set;}
        public string Name {get;set;}
        public string AddressLine1 {get;set;}
        public string AddressLine2 {get;set;}
        public string AddressLine3 {get;set;}
        public string City {get;set;}
        public string PostCode {get;set;}
        public string TelephoneNumber {get;set;}

        /// <summary>
        /// Default constructor
        /// </summary>
        public BookStore()
        {
            this.Id = ObjectId.GenerateNewId();
        }
    }
```

I added a `Model` folder to my `../api` folder where this was added. MongoDB uses a serialization format standard called [BSON](https://docs.mongodb.com/manual/reference/glossary/#term-bson). BSON has the idea of *types* and one of those types is [`ObjectId`](https://docs.mongodb.com/manual/reference/bson-types/#objectid) which represents a unique id for the record. You decorate the `Id` property with `[BsonId]` attribute, to inform the MongoDB driver what field is your `BsonId` field.

I have also created a default constructor that sets that Id up. Now we have created the `Models` directory and structure is now the following:

```
    .
    ├── src
    |   ├── api
    |       ├── Models
    |           ├── BookStore.cs
    |       ├── BookStoreApp.WebApi.csproj
    |       ├── Dockerfile
    ├── tests
    |   ├── integration
    |   ├── unit
    |       ├── BookStore.Tests.csproj
```

### Controller

Now we need to create a **BookStore** controller. A controller is the *C* in MV**C** pattern. The controller will handle the HTTP requests that come in using *Actions*. The HTTP requests are mapped to Actions via the routing pipeline of MVC. For WebAPIs, they map to specific HTTP methods like `GET`, `POST`, `PUT` etc. 

ASP.NET expects you to follow conventions when creating your own Controllers:

- The Controller class name always ends with `Controller`.
- The Controller class should reside in a folder called `Controllers`.
- The Controller class should inherit from `Microsoft.AspNetCore.Mvc.ControllerBase` (WebAPI) / `Microsoft.AspNetCore.Mvc.Controller` (ASP.NET MVC Apps with Views).

When you do a `dotnet new webapi` the templates include a standard `ValuesController.cs` that lives in a `Controllers` folder. Create a new `BookStoreController.cs` file in there:

```
    .
    ├── src
    |   ├── api
    |       ├── Controllers
    |           ├── BookStoreController.cs
    |       ├── Models
    |           ├── BookStore.cs
    |       ├── BookStoreApp.WebApi.csproj
    |       ├── Dockerfile
    ├── tests
    |   ├── integration
    |   ├── unit
    |       ├── BookStore.Tests.csproj
```

To this file I have added the following:

```c#
    [Route("api/[controller]")]
    [ApiController]
    public class BookStoreController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var bookStore = new List<BookStore>{
                new BookStore {
                    Name = "Waterstones",
                    AddressLine1 = "The Dolphin & Anchor",
                    AddressLine2 = "West Street",
                    City = "Chichester",
                    PostCode = "PO19 1QD",
                    TelephoneNumber = "01234 773030"
                }
            };
            return await Task.Run(() => new JsonResult(bookStore));
        }
    }
```

Its a simple `GET` method that just returns a hard coded `BookStore` object. For today's episode we are just concentrating on getting our unit test infrastructure up and running so we can ignore `PUT`, `POST` and `DELETE` until my next episode.

### Unit Tests

Before we move forward we need to use the `dotnet` tooling to add a project reference to our tests project. Change directory to `tests\unit` and do the following to add a reference:

```bash
dotnet add reference ..\..\src\api\BookStoreApp.WebApi.csproj
```

I have also added two basic test methods to cover our new `GET` method in oir controller:

```c#
    public class ControllerTests
    {
        [Fact]
        public async Task BookStoreController_Get_Should_Return_ActionResult()
        {
            //Arrange
            var controller = new BookStoreController();

            //Act
            var result = await controller.Get();

            //Assert
            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task BookStoreController_Get_Should_Return_Correct_BookStore_Data()
        {
            //Arrange
            var controller = new BookStoreController();

            //Act
            var result = await controller.Get();
            var json = result.ToJson<BookStore>();

            //Assert
            Assert.True(json[0].Name == "Waterstones",$"Assert failed, received {json[0].Name} ");
            Assert.True(json[0].PostCode == "PO19 1QD",$"Assert failed, received {json[0].PostCode} ");
            Assert.True(json[0].TelephoneNumber == "01234773030",$"Assert failed, received {json[0].TelephoneNumber} ");
        }
    }
```

Before we start integrating with Docker, we can test using (you guessed it), the `dotnet` tooling. Make sure you are in the `tests\unit` directory and run:

```bash
dotnet restore
dotnet test
```

The `test` will set off a `dotnet build` first then run our XUnit tests. One test should fail and this will be outputted similar to this:

> Total tests: 2. Passed: 1. Failed: 1. Skipped: 0.
>
> Test Run Failed.
>
> Test execution time: 2.6340 Seconds

The error was intentional, there should of been a space in between `"01234773030"`, I fixed this:

```c#
            Assert.True(json[0].TelephoneNumber == "01234 773030",$"Assert failed, received {json[0].TelephoneNumber} ");
```

Re-run the tests and everything should now be green:

> Total tests: 2. Passed: 2. Failed: 0. Skipped: 0.
>
> Test Run Successful.
>
> Test execution time: 3.1173 Seconds

Now we have a project with some basic logic within a controller, and a very basic model. We have also started creating some tests to cover this code. Now we need to *build and run* the application. We could at this stage use the normal route for that, but as these episodes include **Docker**, lets integrate what we know into a docker pipeline.

## Docker

Firstly in the root of the project create a new Docker file. Your project structure should look like this now:

```
    .
    ├── src
    ├── tests
    ├── Dockerfile
```

Here we will create a multi-stage `Dockerfile` that will restore, build and run our tests. Here is the first stage:

```Dockerfile
FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

COPY src/api/BookStoreApp.WebApi.csproj ./src/api/
RUN dotnet restore ./src/api/BookStoreApp.WebApi.csproj
COPY tests/unit/BookStore.Tests.csproj ./tests/unit/
RUN dotnet restore ./tests/unit/BookStore.Tests.csproj

COPY . .
```

You should be familiar with this now from previous episodes. This time take note that we are copying the unit test project files in to the build context. Also note we need to keep the same directory structure as before, because we added a `dotnet add reference` previously into the test project. If the directories didn't match we would get build errors.

Run the following:

```bash
 docker build -t test-ep5 .
```

Now do a `docker image` and you will see a massive image there:

![test-5 image](/assets/img/posts/test-ep5-size.png)

Docker has a way of managing this, meet `.dockerignore`.

### .dockerignore

This file behaves similarly to a `.gitignore`. It tells docker which files to not copying in during a  build. So how do you know which files to ignore, well I learnt a good trick from [Wes Higbee](https://twitter.com/g0t4) by passing in a `ls alR` to list out your directories. Run the following:

```bash
docker run --rm test-ep5 ls -alR
```

This will list out your containers file system, and you can easily see what is copied in. So things like `.vscode`, `.git` folders and `bin` directories. None of this stuff is needed during the build stage of this multi stage `Dockerfile`, so lets exclude it, using similar glob patterns you can use in `.gitignore` files. Add a `.dockerignore` file to your root:

```
    .
    ├── src
    ├── tests
    ├── Dockerfile
    ├── .dockerignore
```

Then add the following, excluding files and artefacts that are not needed for a restore and build. We exclude things like our `docker` folder and `.ps1` scripts we have been using. Plus the `README.md` and dockerfiles.

```
**/.vscode/
**/.git/
docker/
**/bin/
**/obj/
**/.dockerignore/
**/Dockerfile*
**/docker-compose*.yml/
run.ps1
clean.ps1
README.md
```

Re-run:

```bash
 docker build -t test-ep5 .
 docker run --rm test-ep5 ls -alR
```

You will see a cleaner build plus about 100MB less space than previously:

![test-5 image](/assets/img/posts/test-ep5-size-2.png)

## Unit Tests in Docker

So now we have pruned our Image, we can add our tests to our `Dockerfile`. Add the following into your `Dockerfile`:

```dockerfile
RUN dotnet test tests/unit/BookStore.Tests.csproj
RUN dotnet publish src/api/BookStoreApp.WebApi.csproj -o /publish
```

We run what we ran on the commandline early (see no magic). This will run out unit tests, then if they pass, we will publish a new release ready for our 2nd stage to use, by running  `dotnet publish` and outputting to a new `/publish` directory.

Run again using the following and see your test success!

```bash
 docker build -t test-ep5 .
```

### Running your application

In previous episodes we had a multi stage `Dockerfile` with a 2nd stage that runs the application itself. No different here, our 2nd stage allows us to run our application.

**NB:** Once we have added this, we cannot use our **"ls aLR"** trick Wes taught us, as our `ENTRYPOINT` will be set with `dotnet`. Also the 1st build stage is thrown away once used so we cannot access the directory fully anyway.

Add the following to your `Dockerfile` which is our runtime stage:

```dockerfile
FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime-env
WORKDIR /publish
COPY --from=build-env /publish .
ENTRYPOINT [ "dotnet","BookStoreApp.WebApi.dll" ]
```

Rebuild the Image first:

```bash
 docker build -t test-ep5 .
```

Then we can run the Image as a container process, overriding some of the ASP.NET Core environment variables using `-e`. We would override these usually when using `docker-compose`.

```bash
docker run -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_URLS=http://+:5003" -p 5003:5003 --rm -it test-ep5
```

You can then use your application of choice (I'm using Postman, more to come on that) to hit [http://localhost:5003/api/bookstore](http://localhost:5003/api/bookstore). You should receive the following JSON payload:

```json
[
    {
        "id": "5c6947093497a200016c0dee",
        "name": "Waterstones",
        "addressLine1": "The Dolphin & Anchor",
        "addressLine2": "West Street",
        "addressLine3": null,
        "city": "Chichester",
        "postCode": "PO19 1QD",
        "telephoneNumber": "01234 773030"
    }
]
```

## Next time

That's it for today. Remember all code is on [Github](https://github.com/garfbradaz/webapi-episodes/tree/master/episode-5) if you want it. 

On our next episode we will start integrating MongoDB into the application.