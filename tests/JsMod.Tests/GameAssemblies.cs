using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

internal static class GameAssemblies
{
    [ModuleInitializer]
    internal static void Init()
    {
        var managed = Environment.GetEnvironmentVariable("TFWR_MANAGED")
            ?? typeof(GameAssemblies).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().First(a => a.Key == "ManagedDir").Value;
        AssemblyLoadContext.Default.Resolving += (ctx, name) =>
        {
            var path = Path.Combine(managed, name.Name + ".dll");
            return File.Exists(path) ? ctx.LoadFromAssemblyPath(path) : null;
        };
    }
}
