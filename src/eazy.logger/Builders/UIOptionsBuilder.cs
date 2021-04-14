using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace eazy.logger.Builders
{
    public class UIOptionsBuilder : IUIOptionsBuilder
    {
        private readonly IServiceCollection _services;

        public UIOptionsBuilder(IServiceCollection services)
        { 
            _services = services;
        }

        IServiceCollection IUIOptionsBuilder.Services => _services;
    }
}
