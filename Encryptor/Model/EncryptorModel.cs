using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Encryptor.Model
{
    public class EncryptorModel
    {
        private const string KEY = "$C&F)J@NcRfUjWnZr4u7x!A%D*G-KaPd";

        public static byte[] EncryptFile(string inputPath)
        {
            if (string.IsNullOrEmpty(inputPath))
                throw new InvalidOperationException();

            var str = File.ReadAllText(inputPath);

            using (var container = CreateContainer())
            {
                container.GenerateIV();

                var encryptor = container.CreateEncryptor(container.Key, container.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(str);
                    }

                    return container.IV.Concat(ms.ToArray()).ToArray();
                }
            }
        }

        public static string DecryptFile(string inputPath)
        {
            byte[] cipherText = File.ReadAllBytes(inputPath);

            if (cipherText.Length == 0)
                throw new InvalidOperationException();

            using (var container = CreateContainer())
            {
                var blocSize = container.BlockSize / 8;
                container.IV = cipherText.Take(blocSize).ToArray();

                var decryptor = container.CreateDecryptor(container.Key, container.IV);

                using (var ms = new MemoryStream(cipherText.Skip(blocSize).ToArray()))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static bool CanFileBeDecrypted(string filePath)
        {
            try
            {
                DecryptFile(filePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static AesManaged CreateContainer() =>
            new AesManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 256,
                Key = KEY.Select(x => (byte)(x ^ 64)).ToArray()
            };
    }
}
