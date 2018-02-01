using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecurityDemo.Data;
using SecurityDemo.Models;
using SecurityDemo.Services;
using System.Security.Claims;

namespace SecurityDemo
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
			services.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			services.AddIdentity<ApplicationUser, IdentityRole>()
					.AddEntityFrameworkStores<ApplicationDbContext>()
					.AddDefaultTokenProviders();

			// Add application services.
			services.AddTransient<IEmailSender, EmailSender>();

			services.AddAuthorization(config =>
			{
				config.AddPolicy(ApplicationPolicy.BigCheese, policy =>
				{
					policy.RequireRole(ApplicationRoles.Admin)
						.Build();
				});


				config.AddPolicy("CheeseLovers", policy =>
				{

					policy.RequireClaim(ApplicationClaims.FavoriteCheese)
						.Build();

				});

				config.AddPolicy("ProvoloneFans", policy =>
				{

					policy.RequireClaim(ApplicationClaims.FavoriteCheese, new[] { "provolone" })
						.Build();

				});


			});

			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
			RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
		{
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
									name: "default",
									template: "{controller=Home}/{action=Index}/{id?}");
			});

			/// outside the configuration of the HTTP pipeline
			var t = roleManager.RoleExistsAsync(ApplicationRoles.Admin);
			t.Wait();
			if (!t.Result)
			{
				var createTask = roleManager.CreateAsync(new IdentityRole
				{
					Name = ApplicationRoles.Admin,
					NormalizedName = ApplicationRoles.Admin
				});
				createTask.Wait();
			}

			//var jeffTask = userManager.FindByEmailAsync("jeff@jeffreyfritz.com");
			//jeffTask.Wait();
			//userManager.AddClaimAsync(jeffTask.Result, new Claim(ApplicationClaims.FavoriteCheese, "provolone")).Wait();


		}
	}
}
