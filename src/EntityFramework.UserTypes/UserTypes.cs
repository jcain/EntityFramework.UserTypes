namespace EntityFramework.UserTypes
{
   using System;
   using System.Collections.Concurrent;
   using System.Collections.Generic;
   using System.Linq;

   public static class UserTypes
   {
      private static ConcurrentDictionary<Type, List<IUserType>> registry = new ConcurrentDictionary<Type, List<IUserType>>();

      public static void Add<T>(IUserType property)
      {
         var userTypes = registry.GetOrAdd(typeof(T), _ => new List<IUserType>());
         userTypes.Add(property);
      }

      public static IEnumerable<IUserType> GetUserTypes(Type type)
      {
         foreach (var entityType in registry.Keys)
         {
            if (entityType.IsAssignableFrom(type))
            {
               return registry[entityType];
            }
         }

         return Enumerable.Empty<IUserType>();
      }
   }
}