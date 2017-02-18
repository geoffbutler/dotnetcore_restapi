using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentValidation.AspNetCore;
using ContactsCore.Business.Managers;
using ContactsCore.Api.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContactsCore.Business;

namespace ContactsCore.Api
{
    public class Startup
    {        
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // common
            services.AddSingleton(Configuration);


            // data
            services.AddSingleton<Data.Helpers.IDbExceptionHelper, Data.Helpers.NpgSqlExceptionHelper>();
            services.AddDbContext<Data.ContactsContext>(options =>
            {
                var contactsConnString = Configuration["ConnectionStrings:Contacts"];
                options.UseNpgsql(contactsConnString, npgSqlOptions =>
                {
                    npgSqlOptions.MigrationsAssembly("ContactsCore.Data");
                });
            });
            services.AddScoped<Data.IContactsUnitOfWork, Data.ContactsUnitOfWork>();
            services.AddScoped<Data.Helpers.IAdoHelper, Data.Helpers.AdoHelper>();
            

            // business
            services.AddSingleton<PagingHeaderHelper>();

            services.AddScoped<HealthcheckManager>();
            services.AddScoped<ContactsManager>();
            services.AddScoped<ContactDetailsManager>();
            services.AddScoped<ContactWithDetailsManager>();

            var mapperConfig = MapperConfig.Init();
            services.AddSingleton(mapperConfig);
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService));

            // api
            services.AddCors();

            services.AddMvc()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining(typeof(Model.Validators.ContactValidator)));
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Debug);
            //loggerFactory.AddDebug((x, y) => x == "Micro1soft.Data.Entity.Query.QueryContextFactory");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
            {
                builder.WithOrigins(
                    "http://localhost:3000", 
                    "http://localhost:4000", 
                    "http://localhost:4500"
                )
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseMvc();            
        }
    }
}
