using System;

namespace Vertical.Pipelines
{
    /// <summary>
    /// Represents an object that exposes <see cref="IServiceProvider"/>
    /// </summary>
    public interface IApplicationServices
    {
        /// <summary>
        /// Gets a reference to a contextual service provider.
        /// </summary>
        IServiceProvider ApplicationServices { get; }
    }
}