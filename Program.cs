using Microsoft.EntityFrameworkCore;
using Piranha;
using Piranha.AttributeBuilder;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.Data.EF.SQLite;
using Piranha.Manager.Editor;
using PiranhaCMS_DataSelectDefaultValue.Models.Fields;
using PiranhaCMS_DataSelectDefaultValue.Models;
using Piranha.Extend.Serializers;

var builder = WebApplication.CreateBuilder(args);

builder.AddPiranha(options =>
{
    /**
     * This will enable automatic reload of .cshtml
     * without restarting the application. However since
     * this adds a slight overhead it should not be
     * enabled in production.
     */
    options.AddRazorRuntimeCompilation = true;

    options.UseCms();
    options.UseManager();

    options.UseFileStorage(naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
    options.UseImageSharp();
    options.UseTinyMCE();
    options.UseMemoryCache();

    var connectionString = builder.Configuration.GetConnectionString("piranha");
    options.UseEF<SQLiteDb>(db => db.UseSqlite(connectionString));
    options.UseIdentityWithSeed<IdentitySQLiteDb>(db => db.UseSqlite(connectionString));

    /**
     * Here you can configure the different permissions
     * that you want to use for securing content in the
     * application.
    options.UseSecurity(o =>
    {
        o.UsePermission("WebUser", "Web User");
    });
     */

    /**
     * Here you can specify the login url for the front end
     * application. This does not affect the login url of
     * the manager interface.
    options.LoginUrl = "login";
     */
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UsePiranha(options =>
{
    // Initialize Piranha
    App.Init(options.Api);

    // Register DataSelectField With No Default Value
    App.Fields.Register<CustomDataSelectField<PageItemNoDefaultValueModel>>();
    App.Serializers.Register<CustomDataSelectField<PageItemNoDefaultValueModel>>(
        new DataSelectFieldSerializer<CustomDataSelectField<PageItemNoDefaultValueModel>>()
    );

    // Register DataSelectField With Fixed Value
    App.Fields.Register<CustomDataSelectField<PageItemFixedValueModel>>();
    App.Serializers.Register<CustomDataSelectField<PageItemFixedValueModel>>(
        new DataSelectFieldSerializer<CustomDataSelectField<PageItemFixedValueModel>>()
    );

    // Register DataSelectField With ProviderOne
    App.Fields.Register<CustomDataSelectField<PageItemModel<ProviderOne>>>();
    App.Serializers.Register<CustomDataSelectField<PageItemModel<ProviderOne>>>(
        new DataSelectFieldSerializer<CustomDataSelectField<PageItemModel<ProviderOne>>>()
    );

    // Register DataSelectField With ProviderTwo
    App.Fields.Register<CustomDataSelectField<PageItemModel<ProviderTwo>>>();
    App.Serializers.Register<CustomDataSelectField<PageItemModel<ProviderTwo>>>(
        new DataSelectFieldSerializer<CustomDataSelectField<PageItemModel<ProviderTwo>>>()
    );

    // Register DataSelectField With ProviderThree
    App.Fields.Register<CustomDataSelectField<PageItemModel<ProviderThree>>>();
    App.Serializers.Register<CustomDataSelectField<PageItemModel<ProviderThree>>>(
        new DataSelectFieldSerializer<CustomDataSelectField<PageItemModel<ProviderThree>>>()
    );


    // Build content types
    new ContentTypeBuilder(options.Api)
        .AddAssembly(typeof(Program).Assembly)
        .Build()
        .DeleteOrphans();

    // Configure Tiny MCE
    EditorConfig.FromFile("editorconfig.json");

    options.UseManager();
    options.UseTinyMCE();
    options.UseIdentity();

    
});

app.Run();