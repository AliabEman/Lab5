using Azure.Storage.Blobs;
using Lab5.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            /* Define services here */

            builder.Services.AddRazorPages();

            var connection = builder.Configuration.GetConnectionString("AzureDBConnection");
            builder.Services.AddDbContext<PredictionDataContext>(options => options.UseSqlServer(connection));

            var blobConnection = builder.Configuration.GetConnectionString("AzureBlobStorage");
            builder.Services.AddSingleton(new BlobServiceClient(blobConnection)); //BlobServiceClient will maipulate Azure Storage resources and blob containers

            var app = builder.Build();

            app.UseDeveloperExceptionPage(); //clarifying my error

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            /* Define routing here */
            app.UseEndpoints(endpoints =>
            {

                /// Adds endpoints for Razor Pages to the <see cref="IEndpointRouteBuilder"/>.
                endpoints.MapRazorPages();
            });

            app.Run();
        }
    }
}