using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using nebrangu.Data;

namespace nebrangu
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

            var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build();

            string connectionString = configuration.GetConnectionString("MySqlConnection");
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
			builder.Services.AddDbContext<nebranguContext>(options =>
			    options.UseMySQL(builder.Configuration.GetConnectionString("MySqlConnection") ?? throw new InvalidOperationException("Connection string 'nebranguContext' not found.")));

            // Add services to the container.
            builder.Services.AddRazorPages();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapRazorPages();

			app.Run();
		}
	}
}