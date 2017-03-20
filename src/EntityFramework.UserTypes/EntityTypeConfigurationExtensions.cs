namespace EntityFramework.UserTypes
{
   using System;
   using System.Data.Entity.ModelConfiguration;
   using System.Data.Entity.ModelConfiguration.Configuration;
   using System.Linq.Expressions;

   public static class EntityTypeConfigurationExtensions
   {
      public static StringPropertyConfiguration UserTypeProperty<TEntity, TValue, TType>(this EntityTypeConfiguration<TEntity> mapper, Expression<Func<TEntity, TValue>> expression, string backingPropertyName = null)
         where TEntity : class
         where TType : UserTypeBase<TEntity, TValue>
      {
         var propertyName = ExpressionHelper.GetPropertyName(expression);
         if (string.IsNullOrEmpty(backingPropertyName))
         {
            backingPropertyName = $"{propertyName}Backing";
         }

         var userType = Activator.CreateInstance(typeof(TType), propertyName, backingPropertyName) as IUserType;
         UserTypes.Add<TEntity>(userType);

         mapper.Ignore(expression);

         var backingPropertyExpression = ExpressionHelper.GetPropertyGetter<TEntity, string>(backingPropertyName);
         return mapper.Property(backingPropertyExpression).HasColumnName(propertyName);
      }
   }
}