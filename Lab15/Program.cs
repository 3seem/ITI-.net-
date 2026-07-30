using Lab15.Models;
using Lab15_StudentPortalWeb.Services;
using Microsoft.EntityFrameworkCore;
// Lab ID 27 
namespace Lab15
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            //Part C C4 and i comment it in D1
            builder.Services.AddDbContext<StudentPortalContext>(options =>
            {
                options.UseSqlServer("Data Source=DESKTOP-3D31ED4;Initial Catalog=ITI_StudentPortal;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
            });
            // Lab ID 27 -> 27 mod 3 = 0 -> Transient
            builder.Services.AddTransient<IMuhamadStampService, MuhamadStampService>();

            //Part D D5

            //builder.Services.AddDbContext<StudentPortalContext>(options =>
            //     options.UseSqlServer("Data Source=DESKTOP-3D31ED4;Initial Catalog=ITI_StudentPortal;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"),
            //     ServiceLifetime.Singleton);

            var app = builder.Build();
            // Part F , placed first
            //app.Use(async (context, next) =>
            //{
            //    var path = context.Request.Path;
            //    Console.WriteLine($"[START] {path}");

            //    if (path.ToString().Contains("/audit-27"))
            //    {
            //        Console.WriteLine($"[AUDIT] Muhamad Assem ahmed saw a request for {path}");
            //    }

            //    await next();

            //    Console.WriteLine($"[END] {path}");
            //});
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();
            app.Use(async (context, next) =>             // Part F , placed after UseAuthorization

            {
                var path = context.Request.Path;
                Console.WriteLine($"[START] {path}");

                if (path.ToString().Contains("/audit-27"))
                {
                    Console.WriteLine($"[AUDIT] Muhamad Assem ahmed saw a request for {path}");
                }

                await next();

                Console.WriteLine($"[END] {path}");
            });

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
