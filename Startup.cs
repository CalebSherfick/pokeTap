using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace pokeTap
{

  public class Startup
  {
    private readonly string _connectionString = "";
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
      _connectionString = configuration.GetSection("DB").GetValue<string>("mySQLConnectionString");
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      //ADD USER AUTH through JWT
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
          options.LoginPath = "/Account/Login";
          options.Events.OnRedirectToLogin = (context) =>
                  {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                  };
        });
      services.AddCors(options =>
          {
            options.AddPolicy("CorsDevPolicy", builder =>
                  {
                    builder
                              .WithOrigins(new string[]{
                                "http://localhost:8080"
                          })
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                  });
          });

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      //Add DB connection service

      //Add Repos here after they're created or else they will break
    }

    private IDbConnection CreateDBContext()
    {
      var connection = new MySqlConnection(_connectionString);
      connection.Open();
      return connection;
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

      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
}
