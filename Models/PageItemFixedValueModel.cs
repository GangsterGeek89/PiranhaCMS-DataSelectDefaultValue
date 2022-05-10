using Piranha;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace PiranhaCMS_DataSelectDefaultValue.Models
{
    public class PageItemFixedValueModel
    {
        // The id of the page
        public Guid Id { get; set; }

        // The model
        public PageInfo Model { get; set; }

        static async Task<Guid> DefaultValue(IApi api)
        {
            var pages = await api.Pages.GetAllAsync();
            var item = (pages.FirstOrDefault(p => p.Title == "Welcome To Piranha CMS"));

            Guid id = item != null ? item.Id : Guid.Empty;

            return id;
        }

        // Gets a single item with the provided id using the
        // injected services you specify.
        static async Task<PageItemFixedValueModel> GetById(string id, IApi api)
        {
            return new PageItemFixedValueModel
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
}
