using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project_service.Models;
using Microsoft.EntityFrameworkCore;
using Project_service.Service;
using AutoMapper;
using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using Project_service.PagingFIlteringSorting;
using System.Reflection;
using test_project.Models.ViewModels;

namespace test_project
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        
        public ILifetimeScope AutofacContainer { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllersWithViews();

            services.AddDbContext<VehicleContext>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("DBConnection")));

        }

        //Container Builder for Autofac 
        public void ConfigureContainer(ContainerBuilder builder)
        {

            //builder.RegisterAssemblyTypes(Assembly.Load(nameof(Project_service)))
            //    .Where(t => t.Namespace.Contains("Service"))
            //    .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name));

            builder.RegisterType<VehicleMakeService>().As<IVehicleMakeService>();
            builder.RegisterType<VehicleModelService>().As<IVehicleModelService>();
            builder.RegisterType<Filtering>().As<IFiltering>();
            builder.RegisterType<Sorting>().As<ISorting>();
            builder.RegisterType<PaginatedList<VehicleMakeVM>>().As<IPaginatedList<VehicleMakeVM>>();
            builder.RegisterType<PaginatedList<VehicleModelVM>>().As<IPaginatedList<VehicleModelVM>>();

            builder.AddAutoMapper(typeof(Startup).Assembly);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=VehicleMake}/{action=Index}/{id?}");
            });

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

        }
    }
}
