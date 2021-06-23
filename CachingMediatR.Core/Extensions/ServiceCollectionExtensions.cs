using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CachingMediatR.Core.Behaviors;
using CachingMediatR.Core.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CachingMediatR.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCachingMediatRCore(this IServiceCollection services, IConfiguration config)
        {

            var assembly = Assembly.GetExecutingAssembly();
            Debug.WriteLine($"{assembly.FullName}");

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssemblyContaining(typeof(Customer));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            return services;
        }

        //IRequest<List<Customer>>
        //IRequestHandler<GetCustomerListQuery, IEnumerable<Customer>>
        public static IEnumerable<Type> FindUnmatchedRequests(Assembly assembly)
        {
            var types = assembly.GetTypes();
            var requests = types.Where(t => t.Name.Contains("Query") && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)))
                            .ToList();

            var handlerInterfaces = types.Where(t => t.Name.Contains("Handler") && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
                            .ToList();            
            // assembly.GetTypes()
            //     .Where(t => t.IsClass && (t.IsAssignableTo(typeof(IRequestHandler<>)) || t.IsAssignableTo(typeof(IRequestHandler<,>))))
            //     .SelectMany(t => t.GetInterfaces())
            //     .ToList();

            foreach (var resultType  in requests)
            {
                var resultTypeInterfaces = resultType.GetInterfaces();
            }

            return (from request in requests
                    let resultType = request.GetInterfaces()
                        .Single(i => i.IsAssignableTo(typeof(IRequest<>)) && i.GetGenericArguments().Any())
                        .GetGenericArguments()
                        .First()
                    let handlerType = resultType == typeof(Unit)
                        ? typeof(IRequestHandler<>).MakeGenericType(request)
                        : typeof(IRequestHandler<,>).MakeGenericType(request, resultType)
                    where handlerInterfaces.Any(t => t == handlerType) == false
                    select request).ToList();
        }
    }

}
