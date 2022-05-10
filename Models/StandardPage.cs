using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using PiranhaCMS_DataSelectDefaultValue.Models.Fields;

namespace PiranhaCMS_DataSelectDefaultValue.Models
{
    [PageType(Title = "Standard page")]
    public class StandardPage  : Page<StandardPage>
    {
        [Region(Title = "Data Select Fields")]
        public SelectFieldsRegion DataSelectFields { get; set; }

        public class SelectFieldsRegion
        {
            [Field(Title = "No Default Value")]
            public CustomDataSelectField<PageItemNoDefaultValueModel> DataListProviderNoValue { get; set; }

            [Field(Title = "Fixed Default Value")]
            public CustomDataSelectField<PageItemFixedValueModel> DataListProviderFixedValue { get; set; }

            [Field(Title = "Provider One Default Value")]
            public CustomDataSelectField<PageItemModel<ProviderOne>> DataListProviderOne { get; set; }

            [Field(Title = "Provider Two Default Value")]
            public CustomDataSelectField<PageItemModel<ProviderTwo>> DataListProviderTwo { get; set; }

            [Field(Title = "Provider Three Default Value - No Value Found Example")]
            public CustomDataSelectField<PageItemModel<ProviderThree>> DataListProviderThree { get; set; }

        }
    }
}