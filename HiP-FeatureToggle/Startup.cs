using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

using PaderbornUniversity.SILab.Hip.Webservice;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Data;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Managers;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Services;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle
{
    public class Startup
    {
        private const string Version = "v1";
        private const string Name = "HiP Feature Toggle API";

        public Startup(IHostingEnvironment env)
        {
            // load both the appsettings and the appsettings.Development /
            // appsettings.Production files into the Configuration attribute
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = configurationBuilder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Inject a configuration with the properties from AppConfig that
            // match the given Configuration (which was loaded in the constructor).
            services.Configure<AppConfig>(Configuration);

            // Add Cross Orign Requests 
            services.AddCors();

            services.AddDbContext<ToggleDbContext>(
                options => options.UseNpgsql(AppConfig.BuildConnectionString(Configuration))
            );

            // Register the Swagger generator
            services.AddSwaggerGen(c =>
            {
                // Define a Swagger document
                c.SwaggerDoc("v1", new Info() { Title = Name, Version = Version });
                c.OperationFilter<CustomSwaggerOperationFilter>();
            });

            // Add framework services.
            services.AddMvc();

            // Add managers
            services.AddTransient<FeatureGroupsManager>();
            services.AddTransient<FeaturesManager>();
            services.AddTransient<CmsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IOptions<AppConfig> appConfig,
            ToggleDbContext dbContext)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug();
            }

            app.UseCors(builder =>
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin()
            );

            // Configure JWT-based authentication using the configuration values from appsettings*.json
            // TODO: Uncomment to re-enable authentication
            //// Retrieve the AppConfig reference from the IOptions type:
            //var config = appConfig.Value;
            //app.UseJwtBearerAuthentication(new JwtBearerOptions
            //{
            //    Audience = config.CLIENT_ID,
            //    Authority = config.DOMAIN,
            //    AutomaticChallenge = true,
            //    AutomaticAuthenticate = true,
            //    RequireHttpsMetadata = !Convert.ToBoolean(config.ALLOW_HTTP),
            //    Events = new BearerEvents()
            //});

            app.UseMvc();

            // Swagger / Swashbuckle configuration:

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
            });
            // Configure SwaggerUI endpoint
            app.UseSwaggerUI(c =>
            {
                // TODO: Only a hack, if HiP-Swagger is running, SwaggerUI can be disabled for Production
                c.SwaggerEndpoint((env.IsDevelopment() ? "/swagger" : "..") +
                                  "/" + Version + "/swagger.json", Name + Version);
            });

            // Run migrations
            dbContext.Database.Migrate();
            ToggleDbInitializer.Initialize(dbContext);
        }
    }
}
