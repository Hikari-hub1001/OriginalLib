using System;
using System.Security.Cryptography;
using System.Text;

namespace OriginalLib
{
	public class Encryption
	{
		private const int KEY_SIZE = 256;
		private const int BLOCK_SIZE = 128;

		/// <summary>
		/// 特定の文字列を暗号化する
		/// </summary>
		/// <param name="value">暗号化する文字列</param>
		/// <param name="password">パスワード</param>
		/// <returns></returns>
		public static string Encrypt(string value, string password)
		{
			if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(password))
			{
				return value;
			}

			// AESオブジェクトを取得
			var aes = GetAesManaged(password);

			// 対象の文字列をバイトデータに変換
			var byteValue = Encoding.UTF8.GetBytes(value);

			// バイトデータの長さを取得
			var byteLength = byteValue.Length;

			// 暗号化オブジェクトを取得
			var encryptor = aes.CreateEncryptor();

			// 暗号化
			var encryptValue = encryptor.TransformFinalBlock(byteValue, 0, byteLength);

			var base64Value = Convert.ToBase64String(encryptValue);
			return base64Value;
		}


		/// <summary>
		/// 暗号化された文字列を複合する
		/// </summary>
		/// <param name="encryptValue">暗号化された文字列</param>
		/// <param name="password">パスワード</param>
		/// <returns></returns>
		public static string Decrypt(string encryptValue, string password)
		{

			if (string.IsNullOrEmpty(encryptValue) || string.IsNullOrEmpty(password))
			{
				return encryptValue;
			}

			// AESオブジェクトを取得
			var aes = GetAesManaged(password);

			// 暗号化されたBase64文字列をバイトデータに変換します。
			var byteValue = Convert.FromBase64String(encryptValue);

			// バイトデータの長さを取得します。
			var byteLength = byteValue.Length;

			// 復号化オブジェクトを取得します。
			var decryptor = aes.CreateDecryptor();

			// 復号化
			var decryptValue = decryptor.TransformFinalBlock(byteValue, 0, byteLength);

			// 復号化されたバイトデータを文字列に変換
			var stringValue = Encoding.UTF8.GetString(decryptValue);

			return stringValue;
		}

		/// <summary>
		/// 暗号化パラメータの作成
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		private static AesManaged GetAesManaged(string password)
		{
			// AESオブジェクトを生成し、パラメータを設定します。
			var aes = new AesManaged();
			aes.KeySize = KEY_SIZE;
			aes.BlockSize = BLOCK_SIZE;
			aes.Mode = CipherMode.CBC;
			var derive = GetDeriveFromPass(password);
			aes.Key = derive.Key;
			aes.IV = derive.IV;
			aes.Padding = PaddingMode.PKCS7;

			return aes;
		}

		/// <summary>
		/// パスワードをもとに暗号キー、初期化ベクトルを作成
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		private static (byte[] Key, byte[] IV) GetDeriveFromPass(string password)
		{
			//Rfc2898DeriveBytesオブジェクトを作成する
			Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
			//反復処理回数を指定する デフォルトで1000回
			deriveBytes.IterationCount = 1000;

			return (deriveBytes.GetBytes(KEY_SIZE / 8), deriveBytes.GetBytes(BLOCK_SIZE / 8));
		}
	}
}
