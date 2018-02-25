using PCLCrypto;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Veganko.Services
{
    class Helper
    {
        public static string CalculateBase64Sha256Hash(string input)
        {
            input += "cG9Z,GD-LgcZ^#X/";
            var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            byte[] inputBytes = Encoding.Unicode.GetBytes(input);

            byte[] hash = hasher.HashData(inputBytes);
            string base64hash = Convert.ToBase64String(hash);

            return base64hash.Substring(0, base64hash.Length - 1); // last character is always =
        }
    }
}
