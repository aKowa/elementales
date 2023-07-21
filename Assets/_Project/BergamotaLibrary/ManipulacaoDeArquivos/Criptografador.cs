using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace BergamotaLibrary
{
    public static class Criptografador
    {
        // Key for reading and writing encrypted data.
        static byte[] savedKey = { 0x12, 0x16, 0x13, 0x11, 0x17, 0x18, 0x15, 0x12, 0x18, 0x16, 0x13, 0x12, 0x19, 0x15, 0x14, 0x17 };

        /// <summary>
        /// Le um arquivo, descriptografa ele e o retorna como uma string.
        /// </summary>
        /// <param name="caminhoDoArquivo">O caminho do arquivo.</param>
        /// <returns>Uma string.</returns>
        public static string ReadFile(string caminhoDoArquivo)
        {
            if (File.Exists(caminhoDoArquivo))
            {
                // Create FileStream for opening files.
                using (FileStream dataStream = new FileStream(caminhoDoArquivo, FileMode.Open))
                {
                    // Create new AES instance.
                    Aes oAes = Aes.Create();

                    // Create an array of correct size based on AES IV.
                    byte[] outputIV = new byte[oAes.IV.Length];

                    // Read the IV from the file.
                    dataStream.Read(outputIV, 0, outputIV.Length);

                    // Create CryptoStream, wrapping FileStream
                    using (CryptoStream oStream = new CryptoStream(dataStream, oAes.CreateDecryptor(savedKey, outputIV), CryptoStreamMode.Read))
                    {
                        // Create a StreamReader, wrapping CryptoStream
                        using(StreamReader reader = new StreamReader(oStream))
                        {
                            // Read the entire file into a String value.
                            string text = reader.ReadToEnd();
                            return text;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Criptografa uma string e salva ela em um arquivo.
        /// </summary>
        /// <param name="caminhoDoArquivo">O caminho para salvar o arquivo.</param>
        /// <param name="arquivo">String para ser criptografada e salva.</param>
        public static void WriteFile(string caminhoDoArquivo, string arquivo)
        {
            // Create new AES instance.
            Aes iAes = Aes.Create();

            // Create a FileStream for creating files.
            using(FileStream dataStream = new FileStream(caminhoDoArquivo, FileMode.Create))
            {
                // Save the new generated IV.
                byte[] inputIV = iAes.IV;

                // Write the IV to the FileStream unencrypted.
                dataStream.Write(inputIV, 0, inputIV.Length);

                // Create CryptoStream, wrapping FileStream.
                using(CryptoStream iStream = new CryptoStream(dataStream, iAes.CreateEncryptor(savedKey, iAes.IV), CryptoStreamMode.Write))
                {
                    // Create StreamWriter, wrapping CryptoStream.
                    using(StreamWriter sWriter = new StreamWriter(iStream))
                    {
                        // Write to the innermost stream (which will encrypt).
                        sWriter.Write(arquivo);
                    }
                }
            }
        }
    }
}
