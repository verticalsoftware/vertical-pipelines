# vertical-pipelines

Generic "middleware" pipelines.

![.net](https://img.shields.io/badge/Frameworks-.netstandard21+net50-purple)
![GitHub](https://img.shields.io/github/license/verticalsoftware/vertical-pipelines)
![Package info](https://img.shields.io/nuget/v/vertical-pipelines.svg)

[![Dev build](https://github.com/verticalsoftware/vertical-pipelines/actions/workflows/dev-build.yml/badge.svg)](https://github.com/verticalsoftware/vertical-pipelines/actions/workflows/dev-build.yml)
[![Release](https://github.com/verticalsoftware/vertical-pipelines/actions/workflows/release.yml/badge.svg)](https://github.com/verticalsoftware/vertical-pipelines/actions/workflows/release.yml)
[![codecov](https://codecov.io/gh/verticalsoftware/vertical-pipelines/branch/dev/graph/badge.svg?token=4RNB0XF988)](https://codecov.io/gh/verticalsoftware/vertical-pipelines)

## Motivation

ASP.NET Core provides a middleware pipeline to handle HTTP requests. This micro library defines functionality that enable you to construct logic pipelines of your own and control when they are invoked and the contextual data type that is available to the pipeline components.

## Features at a glance

- Use application defined types to pass state around the middleware pipeline
- Integrate easily with dependency injection providers. 

> ⚠️ Heads Up!
> 
> **Version 3 is a breaking design change**
> 
> After a lot of reflection, this library is reverting back to a more simple form. Mainly,
> the design implemented some internal anti-patterns and required a lot of complexity around
> creation of middleware components and having to differentiate between singleton and per-invocation (scoped)
> service dependencies. This next iteration leaves the factory patterns of the components entirely
> up to the client application (and ideally to dependency injection). 
> 
> The reference for version 2 of this library is [here](./README_v2.md), but please note it will no longer be maintained.

## Usage

Middleware components are implemented using the `IPipelineMiddlware<TContext>` interface. The `TContext` generic paramter is a type of the application's choosing, and represents a contextual data object that is passed along the components of the pipeline. The only constraint is that `TContext` must be a class type.

The structure of the implementation is very simple.

```csharp
// Define a contrived context type...
public class MyContext
{
    public string[] Parameters { get; set; }
    
    public IDictionary<string, object> AdditionalData { get; set; }
    
    public object Result { get; set; }
}

// Construct middleware components
public class MiddlewareA : IPipelineMiddleware<MyContext>
{
    public MiddlewareA(/* Dependencies */)
    {
    }
    
    public async Task InvokeAsync(MyContext context,
        PipelineDelegate<MyContext> next,
        CancellationToken cancellationToken)
    {
        // Perform discrete middleware logic
        Console.WriteLine("Middleware A invoked");
        
        // Pass control to next delegate
        await next(context, cancellationToken);
    }
}

public class MiddlewareB : IPipelineMiddleware<MyContext>
{
    public MiddlewareB(/* Dependencies */)
    {
    }
    
    public async Task InvokeAsync(MyContext context,
        PipelineDelegate<MyContext> next,
        CancellationToken cancellationToken)
    {
        // Perform discrete middleware logic
        Console.WriteLine("Middleware A invoked");

        // Pass control to next delegate
        await next(context, cancellationToken);
    }
}

// Construct the pipeline
var pipelineFactory = new PipelineFactory(new IPipelineMiddleware<MyContext>[]
    {
        new MiddlewareA(),
        new MiddlewareB(),
        new MiddelwareAction(async (context, next, cancelToken) => 
        {
            // Perform discrete middleware logic inline
            Console.Writeline("Inline middleware invoked");
            
            // Pass control to next delegate
            await next(context, cancelToken);
        });
    });

var pipelineDelegate = pipelineFactory.CreatePipeline();

await pipelineDelegate(context, cancellationToken);

// Output:
//   Middelware A invoked
//   Middleware B invoked   
//   Inline middleware invoked
```

Notice the different possible behavioral patterns. Middleware can...

1. perform it's discrete logic then pass control to the next middleware.
2. pass control to the next middleware, then perform it's discrete logic.
3. perform it's discrete logic and _short circuit_ the rest of the pipeline by not invoking the `next` delegate.

## Dependency Injection

The simple design allows for effective integration with dependency injection. Consider the following setup that uses [Microsoft Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-6.0):

```csharp
var services = new ServiceCollection();

// TODO: Register singleton services for middleware (e.g. loggers)

services.AddSingleton<IPipelineMiddleware<MyContext>, MiddlewareA>();
services.AddSingleton<IPipelineMiddleware<MyContext>, MiddelwareB>();
services.AddSingleton<IPipelineFactory<MyContext>, PipelineFactory<MyContext>>();
services.AddSingleton<PipelineDelegate<MyContext>>(provider => 
{
    var factory = provider.GetRequiredService<IPipelineFactory<MyContext>>();
    return factory.CreatePipeline();
});
    
var serviceProvider = services.BuildServiceProvider();

// For each request where the pipeline is required...
var pipelineDelegate = serviceProvider.GetRequiredService<PipelineDelegate<MyContext>>();

await pipelineDelegate(context, cancellationToken);
```

If any middleware requires scoped dependencies, register those components and the pipeline factory as scoped.

## Issues or requests

Create an issue [here](https://github.com/verticalsoftware/vertical-pipelines/issues).