# PlayCode API

### Description

This repository contains a sample .NET web API application, demonstrating an implementation of the PlayCode API. The actual endpoints are defined here: 

[https://github.com/Bingo-Norge-Collab/PlayCodeApi/blob/957cd1ec72fba4e3bddfeef1031f733d357b30f8/src/PlayCodeApi/Program.cs#L45](https://github.com/Bingo-Norge-Collab/PlayCodeApi/blob/957cd1ec72fba4e3bddfeef1031f733d357b30f8/src/PlayCodeApi/Program.cs#L45)

The running application with Swagger can be explored here:

TODO - insert link

### Who is this for?

If your system uses `playcodes` as a means of managing play sessions (money, logging in/out), this is for you. Otherwise, a different solution must be deviced.

It is assumed that your `gameserver` already has the ability to manage `playcodes` and that this API will mostly serve as an adapter.

### Great, but what do I do with this?

First and foremost you should explore the API and figure out how to implement it in your system.

There are multiple ways to implement the API;

- Implement API directly in the `gameserver` application, if possible.
- Implement API in a separate application that runs on the same machine, and talks to your `gameserver` application. This will give you more options in terms of choice of programming language, etc.
- If HTTP is not achievable due to restrictions on your `gameserver` OS or otherwise, there’s an option to use a TCP client instead (code sample will be added if required).

### How to contribute

Everyone is invited to make sure the API is feature-complete with regards to their own systems. Anything missing, redundant or just plain wrong? Let us know by:

- Creating an Issue: https://github.com/Bingo-Norge-Collab/PlayCodeApi/issues
- Make changes and create a pull request.

Everyone is invited to contribute code samples in other languages, by creating your own branch and making a pull request (to master).