using Piranha;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace PiranhaCMS_DataSelectDefaultValue.Models
{
    public class PageItemHardCodedIdModel
    {
        // The id of the page
        public Guid Id { get; set; }

        // The model
        public PageInfo Model { get; set; }

        static Guid DefaultValue()
        {
            Guid id = new Guid("{567bfc38-cf39-4f59-94ab-dc6bc87ba6e2}");

            return id;
        }

        // Gets a single item with the provided id using the
        // injected services you specify.
        static async Task<PageItemHardCodedIdModel> GetById(string id, IApi api)
        {
            return new PageItemHardCodedIdModel
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
