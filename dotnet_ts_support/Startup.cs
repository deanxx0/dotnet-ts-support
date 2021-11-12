using dotnet_ts_support.Models;
using dotnet_ts_support.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using tusdotnet;
using tusdotnet.Helpers;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using tusdotnet.Stores;

namespace dotnet_ts_support
{
    public class Startup
    {
        string _ConnectionString;
        string _DBName;
        string _tusDir;
        string _tempDir;
        string _datasetDir;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbSettings = Configuration.GetSection(nameof(DBSettings));
            _ConnectionString = dbSettings.GetValue<string>("ConnectionString");
            _DBName = dbSettings.GetValue<string>("DBName");
            _tusDir = Configuration.GetValue<string>("TUS_DIR");
            _tempDir = Configuration.GetValue<string>("TEMP_DIR");
            _datasetDir = Configuration.GetValue<string>("DATASET_DIR");

            services.AddCors();

            var key = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaa");

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // todo
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.Configure<DBSettings>(Configuration.GetSection(nameof(DBSettings)));
            services.AddSingleton<IDBSettings>(sp => sp.GetRequiredService<IOptions<DBSettings>>().Value);

            services.AddSingleton<UserService>();
            services.AddSingleton<DirectoryService>();
            services.AddSingleton<TrainSettingService>();
            services.AddSingleton<TrainService>();
            services.AddSingleton<TrainServerInfoService>();

            services.AddHostedService<QueueService>(serviceProvider => new QueueService(_ConnectionString, _DBName, "convert_items"));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "dotnet_ts_support", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use((context, next) =>
            {
                // Default limit was changed some time ago. Should work by setting MaxRequestBodySize to null using ConfigureKestrel but this does not seem to work for IISExpress.
                // Source: https://github.com/aspnet/Announcements/issues/267
                context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = null;
                return next.Invoke();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "dotnet_ts_support v1"));
            }

            app.UseRouting();

            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials()
                .WithExposedHeaders("access_token", "file-name")
                .WithExposedHeaders(CorsHelper.GetExposedHeaders())
                );

            app.UseTus(httpContext => new DefaultTusConfiguration
            {
                Store = new TusDiskStore(_tusDir),
                UrlPath = "/files",
                Events = new Events
                {
                    OnFileCompleteAsync = async eventContext =>
                    {
                        System.Console.WriteLine("upload complete!");
                        // eventContext.FileId is the id of the file that was uploaded.
                        // eventContext.Store is the data store that was used (in this case an instance of the TusDiskStore)

                        // A normal use case here would be to read the file and do some processing on it.
                        ITusFile file = await eventContext.GetFileAsync();

                        Dictionary<string, Metadata> metadata = await file.GetMetadataAsync(eventContext.CancellationToken);
                        string zipfileName = System.IO.Path.GetFileNameWithoutExtension(metadata["filename"].GetString(System.Text.Encoding.UTF8));

                        var orgPath = System.IO.Path.Combine(_tusDir, file.Id);
                        var dstPath = $"{orgPath}.zip";
                        if (System.IO.File.Exists(orgPath))
                        {
                            System.IO.File.Move(orgPath, dstPath);
                        }

                        var collection = new MongoDB.Driver.MongoClient(_ConnectionString)
                                            .GetDatabase(_DBName)
                                            .GetCollection<Item>("convert_items");

                        collection.InsertOne(new Item()
                        {
                            filePath = dstPath,
                            tempDir = System.IO.Path.Combine(_tempDir, zipfileName), // before converting directory
                            outputDir = System.IO.Path.Combine(_datasetDir, zipfileName), // result directory
                            status = Status.Ready,
                        });
                        System.Console.WriteLine("DB status ready!");

                        //var result = await DoSomeProcessing(file, eventContext.CancellationToken).ConfigureAwait(false);

                        //if (!result)
                        //{
                        //    //throw new MyProcessingException("Something went wrong during processing");
                        //}
                    }
                }
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
