namespace EntityFramework.UserTypes
{
   public interface IUserType
   {
      void OnObjectMaterialized(object entity);

      void OnSavingChanges(object entity);
   }
}