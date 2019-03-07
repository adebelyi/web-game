using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApi.Models;
using WebApi.Samples;
using WebGame.Domain;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<UserEntity, UserDto>()
                    .ForMember(dest => dest.FullName, opt => opt.MapFrom(expr =>
                        $"{expr.LastName} {expr.FirstName}"));

                cfg.CreateMap<CreateUserDTO, UserEntity>();
                cfg.CreateMap<UpsertUserDTO, UserEntity>();
                cfg.CreateMap<UserEntity, UpsertUserDTO>();
            });
            
            services.AddSwaggerGeneration();

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc(options =>
                {
                    options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                    options.ReturnHttpNotAcceptable = true;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Populate;
                });
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseSwaggerWithUI();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}