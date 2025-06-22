using System.Security.Cryptography;
using System.Text;

namespace LaundryCleaning.Service.Download
{
    public class SecureDownloadHelper
    {
        private readonly string _secretKey;

        public SecureDownloadHelper(IConfiguration config)
        {
            _secretKey = config["DownloadSettings:SecretKey"]
                ?? throw new InvalidOperationException("SecretKey not found.");
        }

        public string GenerateToken(string fileName, TimeSpan validFor)
        {
            var expiry = DateTime.UtcNow.Add(validFor);
            var payload = $"{fileName}|{expiry:O}"; // ISO 8601 format
            var signature = ComputeHmac(payload);
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payload}|{signature}"));
            return token;
        }

        public bool TryValidateToken(string token, out string fileName)
        {
            fileName = string.Empty;

            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var parts = decoded.Split('|');
                if (parts.Length != 3) return false;

                var payload = $"{parts[0]}|{parts[1]}";
                var signature = parts[2];

                if (signature != ComputeHmac(payload)) return false;

                if (!DateTime.TryParse(parts[1], out var expiryUtc)) return false;
                if (DateTime.UtcNow > expiryUtc) return false;

                fileName = parts[0];
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string ComputeHmac(string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }
    }

}
