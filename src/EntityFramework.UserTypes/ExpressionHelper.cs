namespace EntityFramework.UserTypes
{
   using System;
   using System.Linq;
   using System.Linq.Expressions;
   using System.Reflection;

   public static class ExpressionHelper
   {
      public static Expression<Func<T, K>> CreateExpression<T, K>(string propertyName)
      {
         var type = typeof(T);

         var propertyInfo = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
         if (propertyInfo == null)
         {
            throw new ArgumentException($"Property '{propertyName}' on type '{type.FullName}' cannot be found.");
         }

         var parameter = Expression.Parameter(type, "x");
         var expression = Expression.Property(parameter, propertyInfo);
         LambdaExpression lambda = Expression.Lambda(expression, parameter);
         return (Expression<Func<T, K>>)lambda;
      }

      public static Expression<Func<T, TValue>> GetPropertyGetter<T, TValue>(string property)
      {
         string[] props = property.Split('.');
         Type type = typeof(T);

         ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
         Expression expression = parameter;

         foreach (string prop in props)
         {
            PropertyInfo pi = type.GetProperty(prop, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            expression = Expression.Property(expression, pi);
            type = pi.PropertyType;
         }

         LambdaExpression lambda = Expression.Lambda(expression, parameter);
         return Expression.Lambda<Func<T, TValue>>(expression, parameter);
      }

      public static string GetPropertyName(LambdaExpression expression)
      {
         if (expression.Body is UnaryExpression)
         {
            UnaryExpression unex = (UnaryExpression)expression.Body;
            if (unex.NodeType == ExpressionType.Convert)
            {
               Expression ex = unex.Operand;
               MemberExpression mex = (MemberExpression)ex;
               return mex.Member.Name;
            }
         }

         MemberExpression memberExpression = (MemberExpression)expression.Body;
         MemberExpression memberExpressionOrg = memberExpression;

         string path = "";

         while (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
         {
            var propInfo = memberExpression.Expression.GetType().GetProperty("Member");
            var propValue = propInfo.GetValue(memberExpression.Expression, null) as PropertyInfo;

            path = propValue.Name + "." + path;
            memberExpression = memberExpression.Expression as MemberExpression;
         }

         return path + memberExpressionOrg.Member.Name;
      }

      public static Expression<Action<T, TValue>> GetPropertySetter<T, TValue>(string property)
      {
         string[] props = property.Split('.');
         Type type = typeof(T);

         ParameterExpression arg = Expression.Parameter(typeof(T), "x");
         ParameterExpression valueArg = Expression.Parameter(typeof(TValue), "value");
         Expression exp = arg;

         PropertyInfo pi = null;
         foreach (string prop in props.Take(props.Length - 1))
         {
            pi = type.GetProperty(prop);
            exp = Expression.Property(exp, pi);
            type = pi.PropertyType;
         }

         pi = type.GetProperty(props.Last(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
         MethodInfo setter = pi.GetSetMethod(true);
         exp = Expression.Call(exp, setter, valueArg);
         return Expression.Lambda<Action<T, TValue>>(exp, arg, valueArg);
      }

      public static object GetTarget(this object rootObject, Expression expression)
      {
         switch (expression.NodeType)
         {
            case ExpressionType.Parameter:
               return rootObject;

            case ExpressionType.MemberAccess:
               MemberExpression memberExpression = (MemberExpression)expression;
               PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
               if (propertyInfo == null) throw new ArgumentException();
               object target = GetTarget(rootObject, memberExpression.Expression);
               return propertyInfo.GetValue(rootObject, null);

            default:
               throw new InvalidOperationException();
         }
      }
   }
}