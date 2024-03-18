using System.Security.Cryptography;
using System.Text;

namespace DistSysAcwServer.Crypto
{
    public class RsaCryptoService : IDisposable
    {
        private readonly RSACryptoServiceProvider _rsaProvider;

        public RsaCryptoService()
        {
            const int dwKeySize = 2048; // Key size in bits
            const string keyContainerName = "MyKeyContainer"; // Name for the key container

            // Define the CspParameters
            CspParameters cspParams = new CspParameters
            {
                KeyContainerName = keyContainerName,
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            // Check if the key exists
            _rsaProvider = new RSACryptoServiceProvider(cspParams);

            // If no key exists, create a new one
            if (_rsaProvider.PublicOnly || !_rsaProvider.CspKeyContainerInfo.KeyNumber.HasFlag(KeyNumber.Exchange))
            {
                _rsaProvider.PersistKeyInCsp = false; // Do not persist key
                _rsaProvider = new RSACryptoServiceProvider(dwKeySize, cspParams);
                _rsaProvider.PersistKeyInCsp = true; // Now persist the new key
            }
        }

        public byte[] EncryptData(string message)
        {
            byte[] byteMessage = Encoding.UTF8.GetBytes(message);
            return _rsaProvider.Encrypt(byteMessage, RSAEncryptionPadding.OaepSHA1);
        }


        public byte[] DecryptData(byte[] encryptedData)
        {
            return _rsaProvider.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA1);
        }


        public string GetPublicKey()
        {
            return _rsaProvider.ToXmlString(false); // false to get the public key only
        }
        public byte[] SignData(string message)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(message);
            // Sign the data, using SHA1 as the hashing algorithm
            return _rsaProvider.SignData(dataBytes, new SHA1CryptoServiceProvider());
        }

        public bool VerifyData(string originalMessage, byte[] signature)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(originalMessage);
            // Verify the data, using SHA1 as the hashing algorithm
            return _rsaProvider.VerifyData(dataBytes, new SHA1CryptoServiceProvider(), signature);
        }

  // Dispose of the RSA provider
        public void Dispose()
        {
            if (_rsaProvider != null)
            {
                _rsaProvider.PersistKeyInCsp = false; // Remove key from container
                _rsaProvider.Clear();
            }
        }
    }
}
