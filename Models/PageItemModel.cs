using Piranha;
using Piranha.Extend.Fields;
using Piranha.Models;
using System.Reflection;

namespace PiranhaCMS_DataSelectDefaultValue.Models
{
    public class PageItemModel<T> where T : class
    {
        // The id of the page
        public Guid Id { get; set; }

        // The model
        public PageInfo Model { get; set; }

        static async Task<Guid> DefaultValue(IServiceProvider services)
        {
            Guid id = Guid.Empty;
            var converter = typeof(T).GetMethod("GetDefaultValue", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (converter != null)
            {
                using (var scope = services.CreateScope())
                {
                    var param = new List<object>();
                    foreach (var p in converter.GetParameters())
                    {
                        param.Add(scope.ServiceProvider.GetService(p.ParameterType));
                    }

                    // Check for async
                    if (typeof(Task<Guid>).IsAssignableFrom(converter.ReturnType))
                    {
                        id = (await ((Task<Guid>)converter.Invoke(null, param.ToArray())).ConfigureAwait(false));
                    }
                    else
                    {
                        await Task.Run(() =>
                        {
                            id = ((Guid)converter.Invoke(null, param.ToArray()));
                        });
                    }
                }
            }
            return id;
        }

        // Gets a single item with the provided id using the
        // injected services you specify.
        static async Task<PageItemModel<T>> GetById(string id, IApi api)
        {
            return new PageItemModel<T>
            {
                Id = new Guid(id),
                Model = await api.Pages.GetByIdAsync<PageInfo>(new Guid(id))
            };
        }

        // Gets all of the available items to choose from using
        // the injected services you specify.
        static async Task<IEnumerable<DataSelectFieldItem>> GetList(IApi api)
        {
            var pages = await api.Pages.GetAllAsync();

            return pages.Select(p => new DataSelectFieldItem
            {
                Id = p.Id.ToString(),
                Name = p.Title
            });
        }
    }

    // Provider One
    public class ProviderOne
    {
        public static async Task<Guid> GetDefaultValue(IApi api)
        {
            var pages = await api.Pages.GetAllAsync();
            var item = (pages.FirstOrDefault(p => p.Title == "Read The Docs"));

            Guid id = item != null ? item.Id : Guid.Empty;

            return id;
        }
    }
    // Provider Two
    public class ProviderTwo
    {
        public static async Task<Guid> GetDefaultValue(IApi api)
        {
            var pages = await api.Pages.GetAllAsync();
            var item = (pages.FirstOrDefault(p => p.Title == "Blog Archive"));

            Guid id = item != null ? item.Id : Guid.Empty;

            return id;
        }
    }

    // Provider Three - example of not found
    public class ProviderThree
    {
        public static async Task<Guid> GetDefaultValue(IApi api)
        {
            var pages = await api.Pages.GetAllAsync();
            var item = (pages.FirstOrDefault(p => p.Title == "Blog Archive2"));

            Guid id = item != null ? item.Id : Guid.Empty;

            return id;
        }
    }
}
