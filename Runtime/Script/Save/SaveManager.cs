
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace OriginalLib.SaveLoad
{
	public static class SaveManager
	{
		private const string SaveFolderName = "Save";
		private readonly static Dictionary<string, Saveable> s_saveableDic = new();

		private static string s_password = "OriginalLib@SaveManager";
		private static bool s_isInitilizePassword = false;

		public static event Action StartSaveEvent;
		public static event Action FinishSaveEvent;
		private static bool s_isAllSave = false;

		public enum SaveDataIndex
		{ SaveType, MainSaveData }

		/// <summary>
		/// セーブパスワードを再セット
		/// ただし一度きり
		/// </summary>
		/// <param name="newPassword">新規パスワード</param>
		/// <exception cref="SystemException">既に変更があった場合</exception>
		public static void SetPassword(string newPassword)
		{
			if (s_isInitilizePassword)
			{
#if UNITY_EDITOR
				throw new SystemException($"Tried to reset password. old [{s_password}] => new [{newPassword}]");
#else
				throw new SystemException("Tried to reset password.");
#endif
			}
			s_password = newPassword;
			s_isInitilizePassword = true;
		}

		/// <summary>
		/// JSONデータを保存する。
		/// </summary>
		public static async Task Save<T>(T data) where T : Saveable
		{
			if(string.IsNullOrEmpty(data.FileName))
			{
				throw new NullReferenceException();
			}

			if (!s_isAllSave) StartSaveEvent?.Invoke();
			string saveFolderPath = GetSaveFolderPath();
			if (!Directory.Exists(saveFolderPath))
			{
				Directory.CreateDirectory(saveFolderPath);
			}

			string saveVal = JsonUtility.ToJson(data,true);

			if (data.Encrypt)
			{
				// 暗号化処理
				saveVal = Encryption.Encrypt(saveVal, s_password);
			}

			string filePath = Path.Combine(saveFolderPath, data.FileName + (data.Encrypt ? ".olson" : ".json"));
			await Task.Run(() => File.WriteAllText(filePath, saveVal));
			Debug.Log($"Data saved to: {filePath}");
			if (!s_isAllSave) FinishSaveEvent?.Invoke();
		}

		/// <summary>
		/// JSONデータを非同期で保存する。
		/// 1ファイル保存ごとに次のフレームに持ち越します。
		/// </summary>
		public static async Task SaveAllAsync()
		{
			StartSaveEvent?.Invoke();
			s_isAllSave = true;

			foreach (var saveable in s_saveableDic)
			{
				await Save(saveable.Value);

				// 次のフレームまで待機
				await Task.Yield();
			}
			Debug.Log("All data saved.");
			s_isAllSave = false;
			FinishSaveEvent?.Invoke();
		}

		/// <summary>
		/// 1つのファイルを読み込む。
		/// </summary>
		/// <typeparam name="T">読み込むオブジェクトの型。</typeparam>
		/// <param name="fileName">読み込むファイルの名前。</param>
		/// <param name="decrypt">復号化する場合は true。</param>
		/// <returns>読み込んだオブジェクト。</returns>
		public static T Load<T>(string fileName, bool decrypt = false) where T : Saveable
		{
			string saveFolderPath = GetSaveFolderPath();
			string filePath = Path.Combine(saveFolderPath, fileName + (decrypt ? ".olson" : ".json"));

			if (!File.Exists(filePath))
			{
				Debug.LogWarning($"File not found: {filePath}");
				return default;
			}

			string saveVal = File.ReadAllText(filePath);

			if (decrypt)
			{
				saveVal = Encryption.Decrypt(saveVal, s_password);
			}

			T obj = JsonUtility.FromJson<T>(saveVal);
			Debug.Log($"File loaded: {filePath}");
			return obj;
		}


		/// <summary>
		/// 保存フォルダのパスを取得する。
		/// </summary>
		private static string GetSaveFolderPath()
		{
#if UNITY_EDITOR
			return Path.Combine(Application.dataPath, SaveFolderName);
#else
#if DEVELOPMENT_BUILD
			return Path.Combine(Application.persistentDataPath, SaveFolderName,"Debug");
#else
			return Path.Combine(Application.persistentDataPath, SaveFolderName);
#endif
#endif
		}

		public static void AddSaveable(Saveable saveable)
		{
			s_saveableDic[saveable.FileName] = saveable;
		}

	}
}
