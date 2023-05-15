// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

var pasword = "myExtraLongPasswordTo4ek{}7it";
var salt = Encoding.UTF8.GetBytes("MyabsolutelySecureIncomprehensiveStringSalt");
var stopwatch = new Stopwatch();
stopwatch.Start();
for (int i = 0; i < 10000; i++)
    GeneratePasswordHashUsingSalt(pasword, salt);

stopwatch.Stop();
Console.WriteLine("The End. " + stopwatch.ElapsedMilliseconds);
Console.ReadLine();

string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
{
    // Use const values as they won't be changed during execution
    const int iterate = 10000;
    const int bytesCount = 20;

    var source = Encoding.UTF8.GetBytes(passwordText);
    // Added HashAlgorithmName as using default's is not secure. Base SHA1 algorithm is performance expensive.
    var pbkdf2 = new Rfc2898DeriveBytes(source, salt, iterate, HashAlgorithmName.SHA256);
    // Salt should not appear like that in stored hash as it would be simplification for
    // bruteforce checking but to stay consistent with password hashes you already have
    // we don't just change it. If something like that was approved - you should ask users to
    // change thier password via email identification for instance.
    var hash = pbkdf2.GetBytes(bytesCount);
    return Convert.ToBase64String(hash, 0, bytesCount, Base64FormattingOptions.None);
}