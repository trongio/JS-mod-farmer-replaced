using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

// The setting and the loaded content are static, as they are in the plugin.
[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]

internal static class GameAssemblies
{
    internal static readonly string ManagedDir = Environment.GetEnvironmentVariable("TFWR_MANAGED")
        ?? typeof(GameAssemblies).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().First(a => a.Key == "ManagedDir").Value;

    [ModuleInitializer]
    internal static void Init()
    {
        var managed = ManagedDir;
        AssemblyLoadContext.Default.Resolving += (ctx, name) =>
        {
            var path = Path.Combine(managed, name.Name + ".dll");
            return File.Exists(path) ? ctx.LoadFromAssemblyPath(path) : null;
        };
    }
}
