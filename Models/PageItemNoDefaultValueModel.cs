using Piranha;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace PiranhaCMS_DataSelectDefaultValue.Models
{
    public class PageItemNoDefaultValueModel
    {
        // The id of the page
        public Guid Id { get; set; }

        // The model
        public PageInfo Model { get; set; }

        // Gets a single item with the provided id using the
        // injected services you specify.
        static async Task<PageItemNoDefaultValueModel> GetById(string id, IApi api)
        {
            return new PageItemNoDefaultValueModel
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
