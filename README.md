# vertical-pipelines

Middleware pipelines outside of `HttpContext` and `aspnetcore`.

## Usage

This library mimics the middleware functionality of aspnetcore using the following pattern:

- Pipelines are formed by defining components that implement the `IPipelineTask<TContext>` interface, which consists of a single `InvokeAsync` method. The method is provided a user defined state object (`TContext`), a delegate type object that can be used to call the next pipeline component, and a `CancellationToken`.
- Pipelines are invoked by calling the `PipelineDelegate.InvokeAllAsync` method while supplying the component instances and the state object.

## Example
