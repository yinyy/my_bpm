using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Toolkit
{
    public class Aes
    {
        private const string KEY = "L6i0M9zIiVaS5&0qp!BRTjuEHV9#mDm9";
        private const string VECTOR = "ELQ7bUkC2OIR$D@H";
        /// <summary>
        /// AES加密算法
        /// </summary>
        /// <param name="data">待加密的数据</param>
        /// <param name="key">秘钥</param>
        /// <param name="vector">向量</param>
        /// <returns></returns>
        public static string Encrypt(string data, string key=KEY, string vector=VECTOR)
        {
            byte[] dt = Encoding.UTF8.GetBytes(data);

            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key), bKey, bKey.Length);
            byte[] bVector = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(vector), bVector, bVector.Length);
            byte[] Cryptograph = null; // 加密后的密文  

            Rijndael Aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流  
                using (MemoryStream Memory = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象  
                    using (CryptoStream Encryptor = new CryptoStream(Memory,Aes.CreateEncryptor(bKey, bVector),CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流  
                        Encryptor.Write(dt, 0, data.Length);
                        Encryptor.FlushFinalBlock();

                        Cryptograph = Memory.ToArray();
                    }
                }
            }
            catch
            {
                Cryptograph = null;
            }

            return Cryptograph.Select(a => string.Format("{0:X2}", a)).Aggregate((s, b) => s + b);
        }

        public static string Decrypt(string data, string key=KEY, string vector=VECTOR)
        {
            byte[] dt = new byte[data.Length / 2];
            for(int i = 0; i < data.Length; i+=2)
            {
                dt[i / 2] = Convert.ToByte(data.Substring(i, 2), 16);
            }

            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key), bKey, bKey.Length);
            byte[] bVector = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(vector), bVector, bVector.Length);

            byte[] original = null; // 解密后的明文  

            Rijndael Aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流，存储密文  
                using (MemoryStream Memory = new MemoryStream(dt))
                {
                    // 把内存流对象包装成加密流对象  
                    using (CryptoStream Decryptor = new CryptoStream(Memory, Aes.CreateDecryptor(bKey, bVector),CryptoStreamMode.Read))
                    {
                        // 明文存储区  
                        using (MemoryStream originalMemory = new MemoryStream())
                        {
                            byte[] Buffer = new byte[1024];
                            Int32 readBytes = 0;
                            while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                            {
                                originalMemory.Write(Buffer, 0, readBytes);
                            }

                            original = originalMemory.ToArray();
                        }
                    }
                }
            }
            catch
            {
                original = null;
            }
            return Encoding.UTF8.GetString(original);
        }
    }
}
