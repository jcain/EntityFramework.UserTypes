namespace EntityFramework.UserTypes
{
   using System;
   using System.Data.Entity;
   using System.Data.Entity.Core.Objects;
   using System.Data.Entity.Infrastructure;

   public static class DbContextExtensions
   {
      public static void EnableUserTypes(this DbContext context)
      {
         var objectContext = ((IObjectContextAdapter)context).ObjectContext;
         objectContext.ObjectMaterialized += OnObjectMaterialized;
         objectContext.SavingChanges += OnSavingChanges;
      }

      private static void OnObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
      {
         foreach (var userType in UserTypes.GetUserTypes(e.Entity.GetType()))
         {
            userType.OnObjectMaterialized(e.Entity);
         }
      }

      private static void OnSavingChanges(object sender, EventArgs e)
      {
         var context = (ObjectContext)sender;
         foreach (var entry in context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified))
         {
            foreach (var userType in UserTypes.GetUserTypes(entry.Entity.GetType()))
            {
               userType.OnSavingChanges(entry.Entity);
            }
         }
      }
   }
}