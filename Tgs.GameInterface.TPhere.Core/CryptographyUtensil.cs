using System.Security.Cryptography;
using System.Text;

namespace Tgs.GameInterface.TPhere.Core;

internal class CryptographyUtensil : ICryptographyUtensil
{
	public string Md5Encrypt(string source)
	{
		if (string.IsNullOrEmpty(source))
		{
			throw new ArgumentException("this source is empty.");
		}

		var toEncryptArray = Encoding.UTF8.GetBytes(source);

		var hash = MD5.HashData(toEncryptArray);

		return BitConverter.ToString(hash).Replace("-", String.Empty).ToLowerInvariant();
	}
}
