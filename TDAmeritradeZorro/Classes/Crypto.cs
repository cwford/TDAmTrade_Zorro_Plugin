//*****************************************************************************
// File: Crypto.cs
//
// Author: Clyde W. Ford
//
// Date: April 29, 2020
//
// Description: Cryptographic methods used by the TD Ameritrade broker plug-in.
//
// Copright (c) 2020 Clyde W. Ford. All rights reserved.
//
// License: LGPL-3.0 (Non-commercial use only)
//
// DISCLAIMER:
//
// This Zorro plug-in is offered on an AS IS basis with no claims or warranties
// that it is fit or complete for any given purpose. YOU USE THIS PLUG-IN AT
// YOUR OWN RISK.
//
// Since the plug-in may be used as part of a system to trade financial instru-
// ments, the user of this plug-in accepts complete and total responsibility 
// for any damages, monetary or otherwise, that arize from the use of the plug-
// in, and holds harmless the author of the plug-in for any damages, financial
// or otherwise, incurred.
//
// For further information, see the Disclaimer included with this plug-in.
//*****************************************************************************
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TDAmeritradeZorro.Classes
{
    //*************************************************************************
    //  Class: Crypto
    //
    /// <summary>
    /// A static class of cryptographic methods used by this plug-in.
    /// </summary>
    //*************************************************************************
    public class Crypto
    {
        //*********************************************************************
        //  Method: EncryptAndSave
        //
        /// <summary>
        /// Encrypt data and store it in a file.
        /// </summary>
        /// 
        /// <param name="key">
        /// The key used to encrypt/decrypt the data.
        /// </param>
        /// 
        /// <param name="data">
        /// The data to be encrypted and stored.
        /// </param>
        /// 
        /// <param name="filePath">
        /// The full file path for the data to be saved.
        /// </param>
        //*********************************************************************
        public static void 
            EncryptAndSave
            (
            string key, 
            string data, 
            string filePath
            )
        {
            // Encrypt and then save the data
            File.WriteAllText(filePath, EncryptString(key, data));
        }

        //*********************************************************************
        //  Method: RetrieveAndDecrypt
        //
        /// <summary>
        /// Retrieve encrypted data from a file and decrypt it.
        /// </summary>
        /// 
        /// <param name="key">
        /// The key used to encrypt/decrypt the data.
        /// </param>
        /// 
        /// <param name="filePath">
        /// The file path of the data to be encrypted and stored.
        /// </param>
        /// 
        /// <returns>
        /// The decypted data.
        /// </returns>
        //*********************************************************************
        public static string 
            RetrieveAndDecrypt
            (
            string key, 
            string filePath
            )
        {
            // Read the encrypted data from the file
            string encStr = File.ReadAllText(filePath);

            // Return the decrypted data
            return DecryptString(key, encStr);
        }

        //*********************************************************************
        //  Method: EncryptString
        //
        /// <summary>
        /// Encrypt a string of plain text data.
        /// </summary>
        /// 
        /// <param name="key">
        /// The key used to encrypt/decrypt the data.
        /// </param>
        /// 
        /// <param name="plainText">
        /// The plain text data to be encrypted.
        /// </param>
        /// 
        /// <returns>
        /// The encypted data.
        /// </returns>
        //*********************************************************************
        public static string 
            EncryptString
            (
            string key, 
            string plainText
            )
        {
            // Method members
            byte[] iv = new byte[16];
            byte[] array;

            // Get a new AES object
            using (Aes aes = Aes.Create())
            {
                // Get an AES key
                aes.Key = Encoding.UTF8.GetBytes(key);

                // Get an initialization vector
                aes.IV = iv;

                // Create an AES encryptor
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create an in-memory stream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Create a cryptographic stream using the encryptor
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        // Write the plaintext data to the crypto stream 
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            // Write all of the data
                            streamWriter.Write(plainText);
                        }

                        // Create a byte array from the memory stream
                        array = memoryStream.ToArray();
                    }
                }
            }

            // Convert the in-memory byte array to a base-64 character string
            return Convert.ToBase64String(array);
        }

        //*********************************************************************
        //  Method: DecryptString
        //
        /// <summary>
        /// Decrypt a string of encrypted data.
        /// </summary>
        /// 
        /// <param name="key">
        /// The key used to encrypt/decrypt the data.
        /// </param>
        /// 
        /// <param name="cipherText">
        /// The encrypted data as a base-64 character string
        /// </param>
        /// 
        /// <returns>
        /// The decrypted plaint text data.
        /// </returns>
        //*********************************************************************
        public static string 
            DecryptString
            (
            string key, 
            string cipherText
            )
        {
            // Method members
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            // Create an AES object
            using (Aes aes = Aes.Create())
            {
                // Create the AES key from the key used to encrypt the data
                aes.Key = Encoding.UTF8.GetBytes(key);

                // Create an initialization vector
                aes.IV = iv;

                // Create a decryptor from the AES object
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Create an in-memory stream
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    // Create a cryptographic stream using the decryptor
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        // Read the encrypted data through the crypto stream in
                        // order to decrypt it
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            // Return the decrypted plain text data
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}