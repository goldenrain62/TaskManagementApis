using System.Security.Cryptography;
using System.Text;

namespace TaskManagementApis.Services;

public class HasherService {
  public string ComputeMD5Hash(string input) {
    using (var md5 = MD5.Create()) {
      byte[] inputBytes = Encoding.UTF8.GetBytes(input);
      byte[] hashBytes = md5.ComputeHash(inputBytes);
      var sb = new StringBuilder();
      foreach (byte b in hashBytes)
        sb.Append(b.ToString("x2"));
      return sb.ToString();
    }
  }

  public string HashSha256(string input) {
    using var sha = SHA256.Create();
    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
    return Base64UrlEncode(bytes);
  }

  public string Base64UrlEncode(byte[] bytes) =>
    Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}

