﻿using System;

namespace Vertical.Pipelines
{
    /// <summary>
    /// Represents the interface of an object that is used to construct a pipeline
    /// of activities that compose a unit of work.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of object that carries the contextual state of the activity
    /// through all of the middleware components in the pipeline. 
    /// </typeparam>
    public interface IPipelineBuilder<TContext>
    {
        /// <summary>
        /// Registers a delegate that is used as an implementation of middleware. 
        /// </summary>
        /// <param name="middleware">
        /// A function that implements the actions of the middleware component and
        /// routes control flow to the rest of the pipeline.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        IPipelineBuilder<TContext> Use(Func<PipelineDelegate<TContext>, PipelineDelegate<TContext>> middleware);

        /// <summary>
        /// Registers a middleware component that uses an application defined class
        /// implementation.
        /// </summary>
        /// <param name="type">The middleware implementation type.</param>
        /// <param name="args">Arguments to pass to the types constructor during activation.</param>
        /// <typeparam name="TContext">
        /// The type of object that carries the contextual state of the activity
        /// to all of the middleware components in the pipeline. 
        /// </typeparam>
        /// <returns>A reference to the builder instance.</returns>
        IPipelineBuilder<TContext> UseMiddleware(Type type, params object[] args);

        /// <summary>
        /// Registers a middleware component that uses an application defined class
        /// implementation.
        /// </summary>
        /// <typeparam name="T">The type of class that implements the middleware behavior.</typeparam>
        /// <param name="args">Arguments to pass to the types constructor during activation.</param>
        /// <typeparam name="TContext">
        /// The type of object that carries the contextual state of the activity
        /// to all of the middleware components in the pipeline. 
        /// </typeparam>
        /// <returns>A reference to the builder instance.</returns>
        IPipelineBuilder<TContext> UseMiddleware<T>(params object[] args);

        /// <summary>
        /// Builds the pipeline.
        /// </summary>
        /// <returns>The delegate that is used to invoke the first task in the pipeline.</returns>
        PipelineDelegate<TContext> Build();
    }
}