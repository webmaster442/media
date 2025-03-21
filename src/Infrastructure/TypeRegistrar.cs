// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace Media.Infrastructure;

internal sealed class TypeRegistrar : ITypeRegistrar, IDisposable
{
    private readonly IServiceCollection _builder;
    private IServiceProvider? _provider;

    internal sealed class TypeResolver : ITypeResolver
    {
        private readonly IServiceProvider _provider;

        public TypeResolver(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public object? Resolve(Type? type)
        {
            if (type == null)
            {
                return null;
            }

            return _provider.GetService(type);
        }
    }

    public TypeRegistrar(IServiceCollection builder)
    {
        _builder = builder;
    }

    public ITypeResolver Build()
    {
        _provider = _builder.BuildServiceProvider();
        return new TypeResolver(_provider);
    }

    public void Register(Type service, Type implementation)
    {
        _builder.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _builder.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        _builder.AddSingleton(service, (provider) => func());
    }

    public void Dispose()
    {
        if (_provider != null 
            && _provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}