using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ShopFlow.Infrastructure.Persistence;

public static class ModelBuilderExtensions
{
    public static void RegisterAllEntities<TInterface>(this ModelBuilder modelBuilder, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(TInterface).IsAssignableFrom(t));
        foreach (var type in types)
        {
            modelBuilder.Entity(type);
        }
    }
}
