using Airlock.Authentication;
using AutoMapper;
using Beloning.Data.Sql;
using Beloning.Data.UnitOfWork;
using Beloning.Identity.Provider.Principal;
using Beloning.Services;
using Beloning.Services.Config;
using Beloning.Services.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Infrastructure;
using Swashbuckle.AspNetCore.Swagger;

namespace Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            MapperConfiguration = new MapperConfiguration(cfg => { cfg.AddProfile(new BeloningAutoMapperProfile()); });
        }

        public IConfiguration Configuration { get; }
        private MapperConfiguration MapperConfiguration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Medical NLP", Version = "v1" });
            });
            services.AddDbContext<BeloningContext>(options => options.UseSqlServer(Configuration["BeloningConnectionString"]));
            services.AddScoped<IIdentityProvider, IdentityProviderMock>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(sp => MapperConfiguration.CreateMapper());
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IReferralService, ReferralService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IReferralService, ReferralService>();

            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddAzureAdBearer(options => Configuration.Bind(options));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();

            // Enable CORS:
            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Onyva V1");
                //c.ShowRequestHeaders();
            });
        }
    }   
}
