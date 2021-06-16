# vertical-pipelines

Generic "middleware" pipelines.

![.net](https://img.shields.io/badge/Frameworks-.netstandard21+net50-purple)
![GitHub](https://img.shields.io/github/license/verticalsoftware/vertical-pipelines)
![Package info](https://img.shields.io/nuget/v/vertical-pipelines.svg)

[![Dev build](https://github.com/verticalsoftware/vertical-pipelines/actions/workflows/dev-build.yml/badge.svg)](https://github.com/verticalsoftware/vertical-pipelines/actions/workflows/dev-build.yml)
[![Release](https://github.com/verticalsoftware/vertical-pipelines/actions/workflows/release.yml/badge.svg)](https://github.com/verticalsoftware/vertical-pipelines/actions/workflows/release.yml)
[![codecov](https://codecov.io/gh/verticalsoftware/vertical-pipelines/branch/dev/graph/badge.svg?token=4RNB0XF988)](https://codecov.io/gh/verticalsoftware/vertical-pipelines)

## Motivation

Aspnetcore provides a middleware pipeline to handle HTTP requests. This micro library defines some types that enable you to construct logic pipelines of your own, outside of aspnetcore and `HttpContext`, and control when they are invoked and the contextual data type that is available to the pipeline components.

## Usage

A pipeline component is simply a class that implements the `IPipelineTask<T>` interface. `<T>` is the type of state object that you define to share data with your pipeline tasks. The interface consists of a single method that is invoked when it is that component's turn to run its logic. The `InvokeAsync` method accepts the context, a cancellation token, and a an object that is used to invoke the next component in the pipeline.

```csharp
// Example component that does nothing but log errors that occur in the other parts of the pipeline
public class ErrorHandler : IPipelineTask<Context>
{
    public async Task InvokeAsync(Context context,
        PipelineDelegate<Context> next,
        CancellationToken cancallationToken)
    {
        try
        {
            // Call rest of pipeline
            await next.InokeAsync(context, cancellationToken);
        }
        catch (Excepotion exception)
        {
            logger.LogError(exception, "An error occurred");
            throw;
        }
    }
}
```

Pipelines are run by calling `PipelineDelegate.InvokeAllAsync`. This method requires an enumerable collection of `IPielineTask<T>` instances, an instance of the context object, and optionally a cancellation token. Calling this method will invoke the very first component in the pipeline. It is up to the tasks to invoke the rest of the pipeline.

```csharp
var tasks = new IPipelineTask<Context>[]
{
    // Order is important here
    new PipelineTask1(),
    new PipelineTask2()
};

await PipelineDelegate.InvokeAllAsync(tasks, new Context(...), cancellationToken);
```

## Setup in dependency injection

Since all tasks implement the same interface, you could simply provide a service registration for each task.

```csharp
// Example using Microsoft.Extensions.DependencyInjection - the registration type
// of the tasks will mostly depend on the registration types of the services that
// the tasks have as dependencies. Scoped or Transient registrations typically make
// sense as pipelines tend to handle requests.

services.AddScoped<IPipelineTask<Context>, PipelineTask1>();
services.AddScoped<IPipelineTask<Context>, PipelineTask2>();
services.AddScoped<IPipelineTask<Context>, PipelineTask3>();
// etc...

// Then ask for the pipeline components later - for instance, if we're responding
// to work put on a queue
async Task HandleMessageasync(MessageEvent message, CancellationToken cancellationToken)
{
    await using var scope = serviceProvider.CreateScope();

    var pipeline = scope.ServiceProvider.GetService<IEnumerable<IPipelineTask<Context>>>();

    await PipelineDelegate.InvokeAllAsync(pipeline, new Context(message), cancellationToken); 
}
```

There is a reference application in the [examples](https://github.com/verticalsoftware/vertical-pipelines/tree/dev/examples) folder.

## Issues or requests

What a tiny library... making this README took more effort than the code. What else could we want this to do? I'm sure you can think of something, in which case, create an issue [here](https://github.com/verticalsoftware/vertical-pipelines/issues).