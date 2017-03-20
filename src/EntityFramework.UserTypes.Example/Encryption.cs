namespace EntityFramework.UserTypes.Example
{
   using System.IO;
   using System.Security.Cryptography;

   public static class Encryption
   {
      private static byte[] _SECRET_KEY_ = new byte[] {
       203, 188, 138, 224, 187, 101, 39, 164, 229, 31, 54, 58, 173, 41, 118, 171,
       96, 102, 39, 184, 156, 15, 110, 84, 184, 110, 113, 191, 118, 49, 105, 80 };

      private static byte[] _SALT_ = new byte[] { 84, 104, 105, 115, 32, 105, 115, 32, 109, 121, 32, 99, 114, 97, 122, 121, 32, 115, 97, 108, 116 };

      public static string DecryptRijndael(byte[] cipherText)
      {
         PasswordDeriveBytes pdb = new PasswordDeriveBytes(_SECRET_KEY_, _SALT_);
         return DecryptRijndael(cipherText, pdb.GetBytes(32), pdb.GetBytes(16));
      }

      public static string DecryptRijndael(byte[] cipherText, byte[] key, byte[] iv)
      {
         string plainText = null;
         using (RijndaelManaged rijAlg = new RijndaelManaged())
         {
            rijAlg.Key = key;
            rijAlg.IV = iv;

            ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
               using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
               {
                  using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                  {
                     plainText = srDecrypt.ReadToEnd();
                  }
               }
            }
         }

         return plainText;
      }

      public static byte[] EncryptRijndael(string plainText)
      {
         PasswordDeriveBytes pdb = new PasswordDeriveBytes(_SECRET_KEY_, _SALT_);
         return EncryptRijndael(plainText, pdb.GetBytes(32), pdb.GetBytes(16));
      }

      public static byte[] EncryptRijndael(string plainText, byte[] key, byte[] iv)
      {
         byte[] cipher;
         using (RijndaelManaged rijAlg = new RijndaelManaged())
         {
            rijAlg.Key = key;
            rijAlg.IV = iv;

            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
               using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
               {
                  using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                  {
                     swEncrypt.Write(plainText);
                  }
                  cipher = msEncrypt.ToArray();
               }
            }

            return cipher;
         }
      }
   }
}