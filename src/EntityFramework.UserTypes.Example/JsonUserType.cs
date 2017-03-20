namespace EntityFramework.UserTypes.Example
{
   using System;
   using System.Data.Entity.ModelConfiguration;
   using System.Data.Entity.ModelConfiguration.Configuration;
   using System.Linq.Expressions;
   using Newtonsoft.Json;

   public static class EnumUserTypeExtension
   {
      public static StringPropertyConfiguration JsonProperty<TEntity, TValue>(this EntityTypeConfiguration<TEntity> mapper, Expression<Func<TEntity, TValue>> expression, string backingPropertyName = null) where TEntity : class
      {
         return mapper.UserTypeProperty<TEntity, TValue, JsonUserType<TEntity, TValue>>(expression, backingPropertyName);
      }
   }

   /// <summary>
   /// Example user type that persists any value as Json
   /// </summary>
   public class JsonUserType<TEntity, TValue> : UserTypeBase<TEntity, TValue> where TEntity : class
   {
      public JsonUserType(string propertyName, string backingPropertyName)
         : base(propertyName, backingPropertyName)
      {
      }

      protected override string GetBackingValue(object targetValue)
      {
         var json = JsonConvert.SerializeObject(targetValue);
         return json;
      }

      protected override object GetTargetValue(string backingValue)
      {
         object targetValue = JsonConvert.DeserializeObject(backingValue, typeof(TValue));
         return targetValue;
      }
   }
}