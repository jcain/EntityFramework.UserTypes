namespace EntityFramework.UserTypes.Example
{
   using System;
   using System.Data.Entity;
   using System.Linq;

   internal class Program
   {
      private static void Main(string[] args)
      {
         Console.WriteLine("Initializing database...");
         Database.SetInitializer(new MigrateDatabaseToLatestVersion<ExampleContext, MigrationsConfiguration>());

         var context = new ExampleContext();
         Console.WriteLine("Fetching first customer");
         var customer = context.Customers.First();
         Console.WriteLine($"Customer: {customer.Name}, Status: {customer.Status}, Secret: {customer.Secret}, Preferences: {customer.Preferences}");
         Console.WriteLine("Press any key to quit");
         Console.ReadKey();
      }
   }
}