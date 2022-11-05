using Microsoft.Extensions.DependencyInjection;
using RomTools.Services.FileFilters;

namespace RomTools.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFileFilters(this IServiceCollection serviceCollection)
        {
            var fileFilterAssembly = typeof(IFileFilter).Assembly;
            var allFileFilters = fileFilterAssembly.GetTypes().Where(x => typeof(IFileFilter).IsAssignableFrom(x) && !x.IsInterface).ToList();
            foreach (var fileFilter in allFileFilters)
            {
                serviceCollection.AddScoped(typeof(IFileFilter), fileFilter);
            }

            return serviceCollection;
        }
    }
}
