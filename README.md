# PlayCode API

### Description

This repository contains a sample .NET web API application, demonstrating an implementation of the PlayCode API. The actual endpoints are defined here: 

https://github.com/Bingo-Norge-Collab/PlayCodeApi/blob/957cd1ec72fba4e3bddfeef1031f733d357b30f8/src/PlayCodeApi/Program.cs#L45

### Who is this for?

If your system uses `playcodes` as a means of managing play sessions (money, logging in/out), this is for you. Otherwise, a different solution must be deviced.

It is assumed that your `gameserver` already has the ability to manage `playcodes` and that this API will mostly serve as an adapter.

### How can I explore the API?

The running application with Swagger can be explored here:

- https://playcodedemo.development.bingo/swagger/index.html

You can also download and run the docker image from [docker hub](https://hub.docker.com/r/egsthomas/playcodedemo/tags): 

Pull image and run it locally (requires docker to be installed):

- `docker pull egsthomas/playcodedemo:latest` 
- `docker run -d -p 80:8080 --name playcodedemo egsthomas/playcodedemo`
- Open url: http://localhost/swagger/index.html

You can also download the source code and run the application locally in visual studio:

- [Download Visual Studio Community Edition](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&passive=false)
- [Open Solution file](https://github.com/Bingo-Norge-Collab/PlayCodeApi/blob/main/src/PlayCodeApi/PlayCodeApi.sln)
- Hit F5 (should open https://localhost:5556/swagger/index.html in your main browser)

(may require a [separate download of .NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0))

### Great, but what do I do with this?

First and foremost you should explore the API and figure out how to implement it in your system.

There are multiple ways to implement the API;

- Implement API directly in the `gameserver` application, if possible.
- Implement API in a separate application that runs on the same machine, and talks to your `gameserver` application. This will give you more options in terms of choice of programming language, etc.
- If HTTP is not achievable due to restrictions on your `gameserver` OS or otherwise, there’s an option to use a TCP client instead (code sample will be added if required).

### How to contribute

Everyone is invited to make sure the API is feature-complete with regards to their own systems. Anything missing, redundant or just plain wrong? 
Let us know by creating an Issue: https://github.com/Bingo-Norge-Collab/PlayCodeApi/issues

Everyone is also invited to contribute code samples in other languages, by creating your own branch and making a pull request (to master).