﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Configuration;
using QuizApp.Models.Entities;

namespace QuizApp.Models
{
    public class Security
    {
        #region Database Entities Declaration
        QuizAppEntities entities = new QuizAppEntities();
        #endregion

        #region Incrypt
        //Encryption
        public string OpenSSLEncrypt(string plainText, string passphrase)
        {
            // generate salt
            byte[] key, iv;
            byte[] salt = new byte[8];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(salt);
            DeriveKeyAndIV(passphrase, salt, out key, out iv);
            // encrypt bytes
            byte[] encryptedBytes = EncryptStringToBytesAes(plainText, key, iv);
            // add salt as first 8 bytes
            byte[] encryptedBytesWithSalt = new byte[salt.Length + encryptedBytes.Length + 8];
            //Buffer.BlockCopy(Encoding.ASCII.GetBytes("Salted__, encryptedBytesWithSalt, 0, 8"));
           
            Buffer.BlockCopy(salt, 0, encryptedBytesWithSalt, 8, salt.Length);
            Buffer.BlockCopy(encryptedBytes, 0, encryptedBytesWithSalt, salt.Length + 8, encryptedBytes.Length);
            // base64 encode
            return Convert.ToBase64String(encryptedBytesWithSalt);
        }
        static byte[] EncryptStringToBytesAes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");
            // Declare the stream used to encrypt to an in memory
            // array of bytes.
            MemoryStream msEncrypt;
            // Declare the RijndaelManaged object
            // used to encrypt the data.
            RijndaelManaged aesAlg = null;
            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged { Mode = CipherMode.CBC, KeySize = 256, BlockSize = 128, Key = key, IV = iv };
                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for encryption.
                msEncrypt = new MemoryStream();
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                        swEncrypt.Flush();
                        swEncrypt.Close();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }
            // Return the encrypted bytes from the memory stream.
            return msEncrypt.ToArray();
        }
        #endregion

        #region Decrypt
        public string OpenSSLDecrypt(string encrypted, string passphrase)
        {
            if (encrypted != null && encrypted != string.Empty)
            {
                // base 64 decode
                byte[] encryptedBytesWithSalt = Convert.FromBase64String(encrypted);
                // extract salt (first 8 bytes of encrypted)
                byte[] salt = new byte[8];
                byte[] encryptedBytes = new byte[encryptedBytesWithSalt.Length - salt.Length - 8];
                Buffer.BlockCopy(encryptedBytesWithSalt, 8, salt, 0, salt.Length);
                Buffer.BlockCopy(encryptedBytesWithSalt, salt.Length + 8, encryptedBytes, 0, encryptedBytes.Length);
                // get key and iv
                byte[] key, iv;
                DeriveKeyAndIV(passphrase, salt, out key, out iv);
                return DecryptStringFromBytesAes(encryptedBytes, key, iv);
            }
            else
            {
                return null;
            }
        }

        private static void DeriveKeyAndIV(string passphrase, byte[] salt, out byte[] key, out byte[] iv)
        {
            // generate key and iv
            List<byte> concatenatedHashes = new List<byte>(48);
            byte[] password = Encoding.UTF8.GetBytes(passphrase);
            byte[] currentHash = new byte[0];
            MD5 md5 = MD5.Create();
            bool enoughBytesForKey = false;
            // See http://www.openssl.org/docs/crypto/EVP_BytesToKey.html#KEY_DERIVATION_ALGORITHM
            while (!enoughBytesForKey)
            {
                int preHashLength = currentHash.Length + password.Length + salt.Length;
                byte[] preHash = new byte[preHashLength];
                Buffer.BlockCopy(currentHash, 0, preHash, 0, currentHash.Length);
                Buffer.BlockCopy(password, 0, preHash, currentHash.Length, password.Length);
                Buffer.BlockCopy(salt, 0, preHash, currentHash.Length + password.Length, salt.Length);
                currentHash = md5.ComputeHash(preHash);
                concatenatedHashes.AddRange(currentHash);
                if (concatenatedHashes.Count >= 48)
                    enoughBytesForKey = true;
            }
            key = new byte[32];
            iv = new byte[16];
            concatenatedHashes.CopyTo(0, key, 0, 32);
            concatenatedHashes.CopyTo(32, iv, 0, 16);
            md5.Clear();
            md5 = null;
        }

        static string DecryptStringFromBytesAes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");
            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;
            // Declare the string used to hold
            // the decrypted text.
            string plaintext;
            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged { Mode = CipherMode.CBC, KeySize = 256, BlockSize = 128, Key = key, IV = iv };
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                            srDecrypt.Close();
                        }
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }
            return plaintext;
        }
        #endregion

        #region CheckDecypt
        public bool CheckDecypt(string Value,string UserId)
        {
            bool status = false;

            if (Value != null && Value != string.Empty)
            {
                //Get Fix Two Value 
                string FirstValue = ConfigurationManager.AppSettings["FirstValue"].ToString();
                string LastValue = ConfigurationManager.AppSettings["LastValue"].ToString();

                //Split Value Who get by Client Side
                string[] Check = Value.Split('-');
                string DeviceId = Check[5];

                var User = UserInfo(UserId);
                CheckDeviceId(User, DeviceId);
                bool AllowedUserId = false;//GetAllowedUser(User.UserID);

                //Check First and Last Value
                if(AllowedUserId)
                {
                    status = true;
                }
                else if (Check[0] == FirstValue && Check[4] == LastValue)
                {
                    if (DeviceId == User.DeviceID)
                    {
                        status = true;
                    }
                }
            }
            return status;
        }
        #endregion

        #region Get User Info
        public User UserInfo(string info)
        {
            var User = new User();
            if (info.Count() == 10)
            {
                var data = entities.AspNetUsers.Where(x => x.PhoneNumber == info).FirstOrDefault();
                    User = entities.Users.Where(x => x.UserID == data.Id).FirstOrDefault();
                return User;
            }
            else
            {
                User = entities.Users.Where(x => x.UserID == info).FirstOrDefault();
                return User;
            }
        }
        #endregion

        #region Get User Info
        public bool GetAllowedUser(string userId)
        {
            bool res = false;
            GeneralFunctions general = new GeneralFunctions();
            string data = general.getAllowedUser();
            string[] UserId = data.Split(',');
            foreach(var item in UserId)
            {
                if(item == userId)
                {
                    res = true;
                }
            }
            return res;
        }
        #endregion

        #region Check Device Id
        public void CheckDeviceId(User model,string DeviceId)
        {
            if(model.DeviceID == null || model.DeviceID == string.Empty)
            {
                model.DeviceID = DeviceId;
                entities.SaveChanges();
            }
        }
        #endregion

        #region Demo CheckDecypt
        public bool DemoCheckDecypt(string Value, string UserId)
        {
            bool status = false;

            if (Value != null && Value != string.Empty)
            {
                //Get Fix Two Value 
                string FirstValue = ConfigurationManager.AppSettings["FirstValue"].ToString();
                string LastValue = ConfigurationManager.AppSettings["LastValue"].ToString();

                //Split Value Who get by Client Side
                string[] Check = Value.Split('-');
                string DeviceId = Check[5];

                var User = UserInfo(UserId);

                string[] ClientYearTime = Check[3].Split(' ');
                var Date = Convert.ToDateTime(Check[1] + "/" + Check[2] + "/" + ClientYearTime[0]);


                DateTime ClientTime = Convert.ToDateTime(ClientYearTime[1]);
                string CT = ClientTime.Hour + ":" + ClientTime.Minute;
                TimeSpan CTClientHourMinutes = TimeSpan.Parse(CT);
                //ClientTime = DateTime.Now;

                //Client Side Add 2 Minutes
                DateTime ClientTimeAdd = ClientTime.AddMinutes(5);

                //Get Client Hour and Minutes after Adding 2 Minutes
                string CTD = ClientTimeAdd.Hour + ":" + ClientTimeAdd.Minute;
                TimeSpan ClientHourMinutes = TimeSpan.Parse(CTD);

                //Check First and Last Value
                if (Check[0] == FirstValue && Check[4] == LastValue)
                {
                    DateTime SystemDateTime = DateTime.UtcNow;

                    string SDT = SystemDateTime.Hour + ":" + SystemDateTime.Minute;
                    TimeSpan SystemHourMinutes = TimeSpan.Parse(SDT);

                    if (SystemHourMinutes <= ClientHourMinutes && Date.Date == SystemDateTime.Date)
                    {
                        if (DeviceId == User.DeviceID)
                        {
                            status = true;
                        }
                    }

                }
            }
            return status;
        }
        #endregion
    }
}