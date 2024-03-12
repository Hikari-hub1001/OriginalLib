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
		/// ����̕�������Í�������
		/// </summary>
		/// <param name="value">�Í������镶����</param>
		/// <param name="password">�p�X���[�h</param>
		/// <returns></returns>
		public static string Encrypt(string value, string password)
		{
			if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(password))
			{
				return value;
			}

			// AES�I�u�W�F�N�g���擾
			var aes = GetAesManaged(password);

			// �Ώۂ̕�������o�C�g�f�[�^�ɕϊ�
			var byteValue = Encoding.UTF8.GetBytes(value);

			// �o�C�g�f�[�^�̒������擾
			var byteLength = byteValue.Length;

			// �Í����I�u�W�F�N�g���擾
			var encryptor = aes.CreateEncryptor();

			// �Í���
			var encryptValue = encryptor.TransformFinalBlock(byteValue, 0, byteLength);

			var base64Value = Convert.ToBase64String(encryptValue);
			return base64Value;
		}


		/// <summary>
		/// �Í������ꂽ������𕡍�����
		/// </summary>
		/// <param name="encryptValue">�Í������ꂽ������</param>
		/// <param name="password">�p�X���[�h</param>
		/// <returns></returns>
		public static string Decrypt(string encryptValue, string password)
		{

			if (string.IsNullOrEmpty(encryptValue) || string.IsNullOrEmpty(password))
			{
				return encryptValue;
			}

			// AES�I�u�W�F�N�g���擾
			var aes = GetAesManaged(password);

			// �Í������ꂽBase64��������o�C�g�f�[�^�ɕϊ����܂��B
			var byteValue = Convert.FromBase64String(encryptValue);

			// �o�C�g�f�[�^�̒������擾���܂��B
			var byteLength = byteValue.Length;

			// �������I�u�W�F�N�g���擾���܂��B
			var decryptor = aes.CreateDecryptor();

			// ������
			var decryptValue = decryptor.TransformFinalBlock(byteValue, 0, byteLength);

			// ���������ꂽ�o�C�g�f�[�^�𕶎���ɕϊ�
			var stringValue = Encoding.UTF8.GetString(decryptValue);

			return stringValue;
		}

		/// <summary>
		/// �Í����p�����[�^�̍쐬
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		private static AesManaged GetAesManaged(string password)
		{
			// AES�I�u�W�F�N�g�𐶐����A�p�����[�^��ݒ肵�܂��B
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
		/// �p�X���[�h�����ƂɈÍ��L�[�A�������x�N�g�����쐬
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		private static (byte[] Key, byte[] IV) GetDeriveFromPass(string password)
		{
			//Rfc2898DeriveBytes�I�u�W�F�N�g���쐬����
			Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
			//���������񐔂��w�肷�� �f�t�H���g��1000��
			deriveBytes.IterationCount = 1000;

			return (deriveBytes.GetBytes(KEY_SIZE / 8), deriveBytes.GetBytes(BLOCK_SIZE / 8));
		}
	}
}
