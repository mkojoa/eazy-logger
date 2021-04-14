using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace eazy.logger.Builders
{
    public interface IUIOptionsBuilder
    {
        IServiceCollection Services { get; }
    }
}
