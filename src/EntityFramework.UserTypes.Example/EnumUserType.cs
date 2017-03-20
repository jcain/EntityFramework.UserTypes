namespace EntityFramework.UserTypes
{
   using System;
   using System.Data.Entity.ModelConfiguration;
   using System.Data.Entity.ModelConfiguration.Configuration;
   using System.Linq.Expressions;

   public static class EnumUserTypeExtension
   {
      public static StringPropertyConfiguration EnumProperty<TEntity, TValue>(this EntityTypeConfiguration<TEntity> mapper, Expression<Func<TEntity, TValue>> expression, string backingPropertyName = null) where TEntity : class
      {
         return mapper.UserTypeProperty<TEntity, TValue, EnumUserType<TEntity, TValue>>(expression, backingPropertyName);
      }
   }

   /// <summary>
   /// Example user type that persists enums as strings
   /// </summary>
   public class EnumUserType<TEntity, TValue> : UserTypeBase<TEntity, TValue> where TEntity : class
   {
      public EnumUserType(string propertyName, string backingPropertyName)
         : base(propertyName, backingPropertyName)
      {
         if (!typeof(Enum).IsAssignableFrom(typeof(TValue)))
         {
            throw new InvalidOperationException($"Type '{typeof(TValue).Name}' is not an enum type.");
         }
      }

      public override void OnObjectMaterialized(object entity)
      {
         var backingValue = GetBackingValue(entity);
         if (!string.IsNullOrEmpty(backingValue))
         {
            object targetValue = Enum.Parse(typeof(TValue), backingValue);
            SetTargetValue(entity, targetValue);
         }
      }

      public override void OnSavingChanges(object entity)
      {
         string backingValue = null;
         object targetValue = GetTargetValue(entity);
         if (targetValue != null)
         {
            backingValue = targetValue.ToString();
         }

         SetBackingValue(entity, backingValue);
      }
   }
}