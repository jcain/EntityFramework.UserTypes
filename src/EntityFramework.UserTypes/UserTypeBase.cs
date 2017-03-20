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

      public abstract void OnObjectMaterialized(object entity);

      public abstract void OnSavingChanges(object entity);

      protected string GetBackingValue(object entity)
      {
         return (string)backingPropertyGetter.DynamicInvoke(entity);
      }

      protected object GetTargetValue(object entity)
      {
         return propertyGetter.DynamicInvoke(entity);
      }

      protected void SetBackingValue(object entity, string value)
      {
         backingPropertySetter.DynamicInvoke(entity, value);
      }

      protected void SetTargetValue(object entity, object value)
      {
         propertySetter.DynamicInvoke(entity, value);
      }
   }
}