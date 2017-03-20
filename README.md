# EntityFramework.UserTypes
User types implementation for Entity Framework

This implementation is similar to NHibernate's user types capability. It allows you to create custom types by giving you control 
over how properties are persisted and materialized.

# Model Classes
Your model classes must contain backing properties for each user type property. Unfortunately, this is just a limitation of
entity framework, but don't worry, these properties can be marked as private.

```cshpart
 public class Customer
 {
    public CustomerStatus Status { get; set; }
    public string Secret { get; set; }
    public CustomerPreferences Preferences { get; set; }

    #region Backing Fields

    private string StatusBacking { get; set; }
    private string SecretBacking { get; set; }
    private string PreferencesBacking { get; set; }

    #endregion Backing Fields
 }
```
# Configuration
To enable user types, you must add the following to the constructor of your DbContext.
```csharp
 public class ExampleContext : DbContext
 {
    public ExampleContext()
    {
       this.EnableUserTypes();
    }
}      
```
To configure user type properties, you can use the built-in extenstion method.
```csharp
protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
   modelBuilder.Entity<Customer>()
      .UserTypeProperty<Customer, string, CryptoUserType<Customer, string>>(x => x.Secret);
}
```
Or create your own extension to improve the readability of your code.
```csharp
protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
   modelBuilder.Entity<Customer>().CryptoProperty(x => x.Secret);
}
public static class CryptoUserTypeExtension
{
   public static StringPropertyConfiguration CryptoProperty<TEntity, TValue>(this EntityTypeConfiguration<TEntity> mapper, Expression<Func<TEntity, TValue>> expression, string backingPropertyName = null) where TEntity : class
   {
      return mapper.UserTypeProperty<TEntity, TValue, CryptoUserType<TEntity, TValue>>(expression, backingPropertyName);
   }
}
```
# Examples
Column-level encryption. 
```csharp
protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
   modelBuilder.Entity<Customer>().CryptoProperty(x => x.Secret);
}
```
Persist enums as strings (not currently possible with EF).
```csharp
protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
   modelBuilder.Entity<Customer>().EnumProperty(x => x.Status).IsRequired().HasMaxLength(15);
}
```
Persist complex types as JSON.
```csharp
protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
   modelBuilder.Entity<Customer>().JsonProperty(x => x.Preferences);
}
```
