namespace EntityFramework.UserTypes.Example
{
   using System;
   using System.Data.Entity.ModelConfiguration;
   using System.Data.Entity.ModelConfiguration.Configuration;
   using System.Linq.Expressions;

   public static class CryptoUserTypeExtension
   {
      public static StringPropertyConfiguration CryptoProperty<TEntity, TValue>(this EntityTypeConfiguration<TEntity> mapper, Expression<Func<TEntity, TValue>> expression, string backingPropertyName = null) where TEntity : class
      {
         return mapper.UserTypeProperty<TEntity, TValue, CryptoUserType<TEntity, TValue>>(expression, backingPropertyName);
      }
   }

   /// <summary>
   /// Example user type that enables column-level encryption
   /// </summary>
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
}