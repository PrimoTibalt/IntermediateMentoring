// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;

var salt = Encoding.ASCII.GetBytes("MyabsolutelySecureIncomprehensiveStringSalt");
for (int i = 0; i < 10; i++)
{
    var hash = GeneratePasswordHashUsingSalt("myExtraLongPasswordTo4ek{}7it", salt);
    Console.WriteLine(hash);
}

string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
{
    var iterate = 10000;
    var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
    byte[] hash = pbkdf2.GetBytes(20);

    byte[] hashBytes = new byte[36];
    Array.Copy(salt, 0, hashBytes, 0, 16);
    Array.Copy(hash, 0, hashBytes, 16, 20);

    var passwordHash = Convert.ToBase64String(hashBytes);
    return passwordHash;
}