using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Core.SPModels.ActivityOfTeachers;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;

namespace TrainingDivisionKedis.Student
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connection);
            services.AddSingleton<IAppDbContextFactory>(
                sp => new AppDbContextFactory(optionsBuilder.Options));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/Login");
                });

            services.AddScoped<IUserService, StudentUserService>();
            services.AddScoped<IYearService, YearService>();
            services.AddScoped<ITermService, TermService>();
            services.AddScoped<IRaspredelenieService, RaspredelenieService>();
            services.AddScoped<IProgressInStudyService, ProgressInStudyService>();
            services.AddScoped<IUmkFileService, UmkFileService>();
            services.AddScoped<ICurriculumService, CurriculumService>();
            services.AddScoped<IChatService<SPTeacherGetAll>, StudentChatService>();
            services.AddScoped<IFileService<ChatFilesConfiguration>, LocalFileService<ChatFilesConfiguration>>();
            services.AddScoped<IFileService<UmkFilesConfiguration>, LocalFileService<UmkFilesConfiguration>>();
            services.AddScoped<ITestClientService, TestClientService>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                //mc.AddProfile(new MappingProfile());
                mc.AddProfile(new MappingProfileBLL());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            var umkFilesConfiguration = Configuration.GetSection("UmkFilesConfiguration");
            var chatFilesConfiguration = Configuration.GetSection("ChatFilesConfiguration");
            services.Configure<UmkFilesConfiguration>(umkFilesConfiguration);
            services.Configure<ChatFilesConfiguration>(chatFilesConfiguration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
