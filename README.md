# EntityFramework.UserTypes
User types implementation for Entity Framework

Use this framework if you want to overcome some of Entity Frameworks limitations without leaking too many persistence implementation details into your domain model. This implementation is similar to NHibernate's user types capability. It allows you to create custom types by giving you control over how properties are persisted and materialized. 

# Usage Scenarios
* Column level encryption
* Persist enums as strings
* Persist complex types as JSON/XML
* Persist lists as delimited strings

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
# Implementation
To create a user type, your class must implement the IUserType interface.
```csharp
public interface IUserType
{
   void OnObjectMaterialized(object entity);
   void OnSavingChanges(object entity);
}
```
Or your class must derive from the UserTypeBase class. The UserTypeBase class handles most of the work for you and only requires that you implement 2 abstract methods.
```csharp
public class CryptoUserType<TEntity, TValue> : UserTypeBase<TEntity, TValue> where TEntity : class
{
   public CryptoUserType(string propertyName, string backingPropertyName)
      : base(propertyName, backingPropertyName)
   {
   }

   protected override string GetBackingValue(object targetValue)
   {
      var cipher = Encryption.EncryptRijndael(targetValue.ToString());
      return Convert.ToBase64String(cipher);
   }

   protected override object GetTargetValue(string backingValue)
   {
      byte[] cipher = Convert.FromBase64String(backingValue);
      string targetValue = Encryption.DecryptRijndael(cipher);
      return targetValue;
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
