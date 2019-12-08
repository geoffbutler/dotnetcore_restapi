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
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace ContactsCore.Api
{
    public class Startup
    {        
        public Startup(IWebHostEnvironment env)
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

            // logging
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

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
            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining(typeof(Model.Validators.ContactValidator)));
            services.AddCors();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
            {
                builder.WithOrigins(
                    "http://localhost:30000",
                    "http://localhost:40000",
                    "http://localhost:45000"
                )
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
