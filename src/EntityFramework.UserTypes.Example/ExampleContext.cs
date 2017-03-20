namespace EntityFramework.UserTypes.Example
{
   using System.Data.Entity;
   using System.Data.Entity.Migrations;

   public class MigrationsConfiguration : DbMigrationsConfiguration<ExampleContext>
   {
      public MigrationsConfiguration()
      {
         AutomaticMigrationsEnabled = true;
         AutomaticMigrationDataLossAllowed = true;
      }

      protected override void Seed(ExampleContext context)
      {
         var customer = new Customer
         {
            Name = "John Doe",
            Status = CustomerStatus.Preferred,
            Secret = "John's Secret",
            Preferences = new CustomerPreferences
            {
               AllowEmail = true,
               AllowSms = false
            }
         };

         context.Customers.Add(customer);
         context.SaveChanges();
      }
   }

   public class ExampleContext : DbContext
   {
      public ExampleContext()
         : base("name=EntityFramework.UserTypes.Example.ExampleContext")
      {
         this.EnableUserTypes();
      }

      public DbSet<Customer> Customers { get; set; }

      protected override void OnModelCreating(DbModelBuilder modelBuilder)
      {
         var customerMap = modelBuilder.Entity<Customer>();

         // Normal property mapping
         customerMap.Property(x => x.Name).IsRequired().HasMaxLength(50);

         // Enum property mapping with value persisted as a string
         customerMap.EnumProperty(x => x.Status).IsRequired().HasMaxLength(15);

         // Complex type property mapping with the value persisted as Json
         customerMap.JsonProperty(x => x.Preferences);

         // Encrypted property mapping. Value is encrypted at rest.
         customerMap.CryptoProperty(x => x.Secret);
      }
   }
}