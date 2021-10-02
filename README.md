# vertical-pipelines

Generic "middleware" pipelines.

![.net](https://img.shields.io/badge/Frameworks-.netstandard21+net50-purple)
![GitHub](https://img.shields.io/github/license/verticalsoftware/vertical-pipelines)
![Package info](https://img.shields.io/nuget/v/vertical-pipelines.svg)

[![Dev build](https://github.com/verticalsoftware/vertical-pipelines/actions/workflows/dev-build.yml/badge.svg)](https://github.com/verticalsoftware/vertical-pipelines/actions/workflows/dev-build.yml)
[![Release](https://github.com/verticalsoftware/vertical-pipelines/actions/workflows/release.yml/badge.svg)](https://github.com/verticalsoftware/vertical-pipelines/actions/workflows/release.yml)
[![codecov](https://codecov.io/gh/verticalsoftware/vertical-pipelines/branch/dev/graph/badge.svg?token=4RNB0XF988)](https://codecov.io/gh/verticalsoftware/vertical-pipelines)

## Motivation

ASP.NET Core provides a middleware pipeline to handle HTTP requests. This micro library defines some types that enable you to construct logic pipelines of your own and control when they are invoked and the contextual data type that is available to the pipeline components.

## Features at a glance

- Implement middleware using classes or delegates
- Use application defined types to pass state around the middleware pipeline
- Integrate easily with dependency injection providers. 

## Usage

This library does its best to mimic the intent and familiar feel of [ASP.NET Core middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0). The only nuance is scoped/per-request service dependencies, which is discussed in this article.

### Overview

A middleware pipeline is an ordered series of discrete tasks that cooperatively work to complete an activity. Middleware pipelines promote [separation of concerns](https://en.wikipedia.org/wiki/Separation_of_concerns) and reduces coupling of logic between steps of a complex workflow. In addition to performing discrete work, each step (middleware) controls when and if control is transferred to the next step.

![]

## Issues or requests

What a tiny library... making this README took more effort than the code. What else could we want this to do? I'm sure you can think of something, in which case, create an issue [here](https://github.com/verticalsoftware/vertical-pipelines/issues).