namespace EntityFramework.UserTypes.Example
{
   public enum CustomerStatus
   {
      Basic,
      Preferred
   }

   public class Customer
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public CustomerStatus Status { get; set; }
      public string Secret { get; set; }
      public CustomerPreferences Preferences { get; set; }

      #region Backing Fields

      private string StatusBacking { get; set; }
      private string SecretBacking { get; set; }
      private string PreferencesBacking { get; set; }

      #endregion Backing Fields
   }

   public class CustomerPreferences
   {
      public bool AllowEmail { get; set; }
      public bool AllowSms { get; set; }

      public override string ToString()
      {
         if (AllowEmail && AllowSms)
            return "Email & SMS";
         else if (AllowEmail)
            return "Email Only";
         else if (AllowSms)
            return "SMS Only";
         else
            return "None";
      }
   }
}