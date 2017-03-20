namespace EntityFramework.UserTypes
{
   using System;

   public abstract class UserTypeBase<TEntity, TValue> : IUserType where TEntity : class
   {
      private Delegate propertyGetter;
      private Delegate propertySetter;
      private Delegate backingPropertyGetter;
      private Delegate backingPropertySetter;

      public UserTypeBase(string propertyName, string backingPropertyName)
      {
         propertyGetter = ExpressionHelper.GetPropertyGetter<TEntity, TValue>(propertyName).Compile();
         propertySetter = ExpressionHelper.GetPropertySetter<TEntity, TValue>(propertyName).Compile();
         backingPropertyGetter = ExpressionHelper.GetPropertyGetter<TEntity, string>(backingPropertyName).Compile();
         backingPropertySetter = ExpressionHelper.GetPropertySetter<TEntity, string>(backingPropertyName).Compile();
      }

      protected abstract object GetTargetValue(string backingValue);

      protected abstract string GetBackingValue(object targetValue);

      public void OnObjectMaterialized(object entity)
      {
         var backingValue = (string)backingPropertyGetter.DynamicInvoke(entity);
         if (!string.IsNullOrEmpty(backingValue))
         {
            var targetValue = GetTargetValue(backingValue);
            propertySetter.DynamicInvoke(entity, targetValue);
         }
      }

      public void OnSavingChanges(object entity)
      {
         string backingValue = null;
         object value = propertyGetter.DynamicInvoke(entity);
         if (value != null)
         {
            backingValue = GetBackingValue(value);
         }
         backingPropertySetter.DynamicInvoke(entity, backingValue);
      }
   }
}