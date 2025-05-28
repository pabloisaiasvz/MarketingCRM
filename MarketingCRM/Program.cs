using ProjectApprovalSystem.Data;
using ProjectApprovalSystem.ConsoleUI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ProjectApprovalSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("  SISTEMA DE APROBACIÓN DE PROYECTOS");
            Console.WriteLine("=============================================");

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<ProjectApprovalDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            try
            {
                using (var context = new ProjectApprovalDbContext(optionsBuilder.Options))
                {
                    context.Database.EnsureCreated();
                    var manager = new ProjectApprovalManager(context);
                    manager.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }

            Console.WriteLine("Gracias por usar el sistema de aprobación de proyectos!");
            Console.ReadKey();
        }
    }
}