using Piranha.Extend;
using Piranha.Extend.Fields;
using System.Reflection;

namespace PiranhaCMS_DataSelectDefaultValue.Models.Fields
{
    [FieldType(Name = "Custom Data Select", Shorthand = "CDS", Component = "data-select-field")]
    public class CustomDataSelectField<T> : DataSelectFieldBase where T : class
    {
        /// <summary>
        /// Gets the currently selected value.
        /// </summary>
        public T Value { get; set; }

        public async Task Init(IServiceProvider services)
        {
            //if (string.IsNullOrWhiteSpace(Id))
            //    Id = await GetDefaultValue(services);

            if (string.IsNullOrWhiteSpace(Id))
                return;

            Value = await GetItemById(services);
        }

        public async Task InitManager(IServiceProvider services)
        {
            Items = await GetItemList(services);
            if (string.IsNullOrWhiteSpace(Id))
                Id = await GetDefaultValue(services);
        }

        public override string GetTitle()
        {
            if (Value != null)
            {
                return Value.ToString();
            }
            return "Not item selected";
        }

        private async Task<string> GetDefaultValue(IServiceProvider services)
        {
            Guid id = Guid.Empty;
            var getDefaultValue = typeof(T).GetMethod("DefaultValue", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (getDefaultValue != null)
            {
                using (var scope = services.CreateScope())
                {
                    var param = new List<object>();
                    foreach (var p in getDefaultValue.GetParameters())
                    {
                        param.Add(scope.ServiceProvider.GetService(p.ParameterType));
                    }

                    // Check for async
                    if (typeof(Task<Guid>).IsAssignableFrom(getDefaultValue.ReturnType))
                    {
                        id = (await ((Task<Guid>)getDefaultValue.Invoke(null, param.ToArray())).ConfigureAwait(false));
                    }
                    else
                    {
                        await Task.Run(() =>
                        {
                            id = ((Guid)getDefaultValue.Invoke(null, param.ToArray()));
                        });
                    }
                }
            }

            return id.ToString();
        }

        private async Task<IEnumerable<DataSelectFieldItem>> GetItemList(IServiceProvider services)
        {
            IEnumerable<DataSelectFieldItem> items = new List<DataSelectFieldItem>();
            var get = typeof(T).GetMethod("GetList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (get != null)
            {
                using (var scope = services.CreateScope())
                {
                    var param = new List<object>();

                    foreach (var p in get.GetParameters())
                    {
                        param.Add(scope.ServiceProvider.GetService(p.ParameterType));
                    }

                    // Check for async
                    if (typeof(Task<IEnumerable<DataSelectFieldItem>>).IsAssignableFrom(get.ReturnType))
                    {
                        items = (await ((Task<IEnumerable<DataSelectFieldItem>>)get.Invoke(null, param.ToArray())).ConfigureAwait(false)).ToArray();
                    }
                    else
                    {
                        await Task.Run(() =>
                        {
                            items = ((IEnumerable<DataSelectFieldItem>)get.Invoke(null, param.ToArray())).ToArray();
                        });

                    }
                }
            }
            return items;
        }

        private async Task<T> GetItemById(IServiceProvider services)
        {
            T Result = null;
            var get = typeof(T).GetMethod("GetById", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (get != null)
            {
                // Now inject any other parameters
                using (var scope = services.CreateScope())
                {
                    var param = new List<object>();

                    // First add the current id to the params
                    param.Add(Id);

                    foreach (var p in get.GetParameters().Skip(1))
                    {
                        param.Add(scope.ServiceProvider.GetService(p.ParameterType));
                    }

                    // Check for async
                    if (typeof(Task<T>).IsAssignableFrom(get.ReturnType))
                    {
                        Result = await ((Task<T>)get.Invoke(null, param.ToArray())).ConfigureAwait(false);
                    }
                    else
                    {
                        await Task.Run(() =>
                        {
                            Result = (T)get.Invoke(null, param.ToArray());

                        });
                    }
                }
            }
            return Result;
        }
    }
}
