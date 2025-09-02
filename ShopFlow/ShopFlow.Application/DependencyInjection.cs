using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;
using ShopFlow.Application.UseCases.Users;
using ShopFlow.Application.UseCases.Products;

namespace ShopFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Users
        services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
        services.AddScoped<IGetUserUseCase, GetUserUseCase>();

        // Products
        services.AddScoped<ICreateProductUseCase, CreateProductUseCase>();
        services.AddScoped<IGetProductUseCase, GetProductUseCase>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}