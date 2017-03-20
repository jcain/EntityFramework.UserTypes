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

      public override void OnObjectMaterialized(object entity)
      {
         var backingValue = GetBackingValue(entity);
         if (!string.IsNullOrEmpty(backingValue))
         {
            byte[] cipher = Convert.FromBase64String(backingValue);
            string value = Encryption.DecryptRijndael(cipher);
            SetTargetValue(entity, value);
         }
      }

      public override void OnSavingChanges(object entity)
      {
         string backingValue = null;
         object value = GetTargetValue(entity);
         if (value != null)
         {
            var cipher = Encryption.EncryptRijndael(value.ToString());
            backingValue = Convert.ToBase64String(cipher);
         }
         SetBackingValue(entity, backingValue);
      }
   }
}