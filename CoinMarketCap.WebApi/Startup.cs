using System.Collections.Generic;
using System.Globalization;
using CoinMarketCap.Dal;
using CoinMarketCap.WebApi.Auth;
using CoinMarketCap.WebApi.Middleware;
using CoinMarketCap.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace CoinMarketCap.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, level) => level > LogLevel.Information, true) });


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            // auth
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthenticationOptions.Issuer,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthenticationOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                });

            services.Configure<FormOptions>(
                options =>
                {
                    options.ValueLengthLimit = int.MaxValue;
                    options.MultipartBodyLengthLimit = int.MaxValue;
                    options.MultipartHeadersLengthLimit = int.MaxValue;
                });


            // db
            services.AddDbContextPool<CoinMarketCapContext>(options =>
            {
                options.UseLoggerFactory(MyLoggerFactory);
                options.UseMySql(Configuration.GetConnectionString("MySql"));
            });
            services.AddTransient<ICoinMarketCapContext, CoinMarketCapContext>();

            // _settings
            services.Configure<AuthenticationSettings>(Configuration.GetSection("AuthenticationSettings"));
            services.Configure<Settings>(Configuration.GetSection("Settings"));
            services.Configure<QuoteUpdaterSettings>(Configuration.GetSection("QuoteUpdaterSettings"));
            var coinMarketCapSection = Configuration.GetSection("CoinMarketCapSettings");
            services.Configure<CoinMarketCapSettings>(coinMarketCapSection);


            // localization
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            
            // services
            services.AddSingleton<CoinMarketCapProvider>();
            services.AddTransient<CoinMarketCapManager>();
            services.AddSingleton<IHostedService, QuoteUpdaterService>();

            // coinMarketCap Calculators
            var coinMarketCapSettings = new CoinMarketCapSettings();
            coinMarketCapSection.Bind(coinMarketCapSettings);
            services.AddPairCalculators(coinMarketCapSettings);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile(Configuration.GetSection("Logging"));


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<CoinMarketCapContext>();
                    context.Database.Migrate();
                }
            }


            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("ru"), new CultureInfo("en") },
                DefaultRequestCulture = new RequestCulture("en")
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.UseMiddleware(typeof(ErrorHandler));
            app.UseMvc();
        }
    }
}
