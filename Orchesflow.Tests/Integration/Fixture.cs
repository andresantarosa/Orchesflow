using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orchesflow.Example.Persistence;

namespace Orchesflow.Tests.Integration
{
    public class Fixture
    {
        protected HttpClient HttpClient;
        
        public Fixture()
        {
            var application = new WebApplicationFactory<Program>();
            HttpClient = application.CreateClient();

            using (var scope = application.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.ExecuteSqlRaw("DROP TABLE Customer");
                context.Database.Migrate();
            }
        }
    }
}