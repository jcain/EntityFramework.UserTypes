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

         var customer2 = new Customer
         {
            Name = "Sarah",
            Status = CustomerStatus.Basic,
            Secret = "Sarah's Secret",
            Preferences = new CustomerPreferences
            {
               AllowEmail = true,
               AllowSms = true
            }
         };

         context.Customers.Add(customer2);
         context.SaveChanges();

         customer2 = context.Customers.OrderByDescending(x => x.Id).First();
         Console.WriteLine($"Customer: {customer2.Name}, Status: {customer2.Status}, Secret: {customer2.Secret}, Preferences: {customer2.Preferences}");

         Console.WriteLine("Press any key to quit");
         Console.ReadKey();
      }
   }
}