using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Twig.Wpf.Example;

public static class AppContainer
{
    private static IServiceProvider? _hostServiceProvider;

    public static T GetService<T>()
        where T : class
    {
        if (_hostServiceProvider == null)
        {
            throw new InvalidOperationException("App Container not init");
        }

        var service = _hostServiceProvider.GetService<T>();

        if (service == null)
        {
            throw new InvalidOperationException($"Cannot get service of {typeof(T).Name}");
        }

        return service;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetServiceProvider(IServiceProvider services)
    {
        _hostServiceProvider = services;
    }
}
