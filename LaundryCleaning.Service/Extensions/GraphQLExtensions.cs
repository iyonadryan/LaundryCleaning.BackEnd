using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using HotChocolate;
using HotChocolate.Execution.Configuration;

namespace LaundryCleaning.Service.Extensions
{
    public static class GraphQLExtensions
    {
        public static IRequestExecutorBuilder AddTypeExtensionsFromAssembly(
            this IRequestExecutorBuilder builder, Assembly assembly)
        {
            var types = assembly
                .GetTypes()
                .Where(t => t.IsClass
                            && !t.IsAbstract
                            && t.GetCustomAttributes(typeof(ExtendObjectTypeAttribute), true).Any());

            foreach (var type in types)
            {
                builder.AddTypeExtension(type);
            }

            return builder;
        }
    }

}
