using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace OriginalLib
{
	public class CSVEditor
	{
		/// <summary>
		/// CSVファイルを読み込み、クラスへ格納する
		/// CSVParameterAttributeをもとに情報と変数を紐づける
		/// CSVファイルはStreaminAssetsフォルダに配置すること。
		/// パスワードを設定した場合複合化する
		/// </summary>
		/// <typeparam name="T">CSVの情報を受け取るクラス</typeparam>
		/// <param name="fileName">CSVファイルのパス（StreaminAssets以下）</param>
		/// <param name="password">複合化用パスワード</param>
		/// <returns>格納したクラスリスト</returns>
		public static List<T> ReadCSV<T>(string fileName, string password = null) where T : new()
		{
			List<T> dataList = new List<T>();

			string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

			using (var sr = new StreamReader(filePath, System.Text.Encoding.UTF8))
			{
				//末尾の改行を削除する
				var fileVal = sr.ReadToEnd().TrimEnd('\r', '\n').Split("\r\n");

				//パスワードが設定されている場合は暗号化する
				if (!string.IsNullOrEmpty(password))
				{
					fileVal = Encryption.Decrypt(fileVal[0], password).Split("\r\n");
				}

				//ヘッダー、データで最低2行分のデータが取得できない場合はフォーマットに問題がある
				if (fileVal.Length < 2)
				{
					throw new FormatException("The format of this file is incorrect.");
				}

				// ヘッダー行を読み込み
				//var headerLine = sr.ReadLine();
				var headerLine = fileVal[0];
				var headers = headerLine.Split(',');

				// ヘッダー名と変数名の対応を作成
				var fieldInfoDict = new Dictionary<string, string>();
				var fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				foreach (var field in fields)
				{
					var attr = field.GetCustomAttributes(typeof(CSVParameterAttribute), true);
					if (attr.Length > 0)
					{
						var parameter = (CSVParameterAttribute)attr[0];
						fieldInfoDict.Add(parameter.headerName, field.Name);
					}
				}

				// 各行を読み込み
				foreach (string line in fileVal[1..fileVal.Length])
				{
					//var line = sr.ReadLine();
					var values = line/*.Replace("<br>", "\r\n")*/.Split(',');//<br>を改行コードに差し替える

					T data = new T();

					for (int i = 0; i < headers.Length; i++)
					{
						string fieldName;
						if (fieldInfoDict.TryGetValue(headers[i], out fieldName))
						{
							var field = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
							if (field.FieldType == typeof(int))
							{
								int intValue;
								if (int.TryParse(values[i], out intValue))
								{
									field.SetValue(data, intValue);
								}
								else
								{
									Debug.LogWarning($"Type mismatch:{field.FieldType} != {values[i]}");
								}
							}
							else if (field.FieldType == typeof(float))
							{
								float floatValue;
								if (float.TryParse(values[i], out floatValue))
								{
									field.SetValue(data, floatValue);
								}
								else
								{
									Debug.LogWarning($"Type mismatch:{field.FieldType} != {values[i]}");
								}
							}
							else if (field.FieldType == typeof(bool))
							{
								bool boolValue;
								if (bool.TryParse(values[i], out boolValue))
								{
									field.SetValue(data, boolValue);
								}
								else
								{
									Debug.LogWarning($"Type mismatch:{field.FieldType} != {values[i]}");
								}
							}
							else if (field.FieldType == typeof(string))
							{
								if (values[i].Contains("\r\n") || values[i].Contains("\n"))
								{
									values[i] = values[i].Substring(1, values[i].Length - 2);//最初と最後を削除
								}
								if (values[i].Contains("\"\""))
								{
									values[i] = values[i].Replace("\"\"", "\"");//[""]が使われていたら["]に置換する
								}
								field.SetValue(data, values[i]);
							}
							else if (field.FieldType.IsEnum)
							{
								//var enumValue = Enum.TryParse(field.FieldType, values[i]);
								//if (Enum.IsDefined(field.FieldType, enumValue))
								object enumValue = Activator.CreateInstance(field.FieldType);
								if (Enum.TryParse(field.FieldType, values[i], out enumValue))
								{
									field.SetValue(data, enumValue);
								}
								else
								{
									//Debug.Log("Invalid enum value: " + enumValue);
									Debug.LogWarning($"Type mismatch:{field.FieldType} != {values[i]}");
								}
							}
							else
							{
								Debug.LogWarning("Unsupported field type: " + field.FieldType);
							}
						}
					}

					dataList.Add(data);
				}
			}
			return dataList;
		}


		/// <summary>
		/// 変数情報をCSVに書き込む
		/// CSVParameterAttributeをもとに情報と変数を紐づける
		/// CSVファイルはStreaminAssetsフォルダに配置すること。
		/// パスワード設定した場合は暗号化して保存する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fileName">保存するファイルパス</param>
		/// <param name="data">保存するデータ</param>
		/// <param name="password">暗号化パスワード</param>
		public static bool WriteCSV<T>(string fileName, IEnumerable<T> data, string password = null) where T : new()
		{

			try
			{
				var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(field => Attribute.IsDefined(field, typeof(CSVParameterAttribute)));
				var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(property => Attribute.IsDefined(property, typeof(CSVParameterAttribute)));

				string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

				using (var sw = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
				{
					// ヘッダーを書き込み
					var header = fields.Select(field => GetFieldName(field)).Concat(properties.Select(property => GetPropertyName(property)));
					var s = string.Join(",", header);
					List<string> values = new List<string>();
					values.Add(s);
					// データを書き込み
					foreach (var item in data)
					{
						var line = fields.Select(field => GetFieldValue(field, item)).Concat(properties.Select(property => GetPropertyValue(property, item))).ToList();
						for(int i= 0;i < line.Count();i++)
						{
							if (line[i] is string)
							{
								var str = line[i] as string;
								if (str.Contains("\""))
								{
									//["]は[""]に変更
									str = str.Replace("\"", "\"\"");
								}
								if (str.Contains("\r\n") || str.Contains("\n"))
								{
									//まずは改行コードを変更
									str = str.Replace("\r\n", "\n");
									//前後に["]を追加
									str = "\"" + str + "\"";
								}

								line[i] = (object)str;
							}
						}
						values.Add(string.Join(",", line));
					}

					string st = string.Join("\r\n", values);

					//パスワードが設定されている場合は複合する
					if (!string.IsNullOrEmpty(password))
					{
						st = Encryption.Encrypt(st, password);
					}

					sw.WriteLine(st);
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				return false;
			}

			return true;

		}

		private static string GetFieldName(FieldInfo field)
		{
			var attr = Attribute.GetCustomAttribute(field, typeof(CSVParameterAttribute)) as CSVParameterAttribute;
			return attr.headerName ?? field.Name;
		}

		private static string GetPropertyName(PropertyInfo property)
		{
			var attr = Attribute.GetCustomAttribute(property, typeof(CSVParameterAttribute)) as CSVParameterAttribute;
			return attr.headerName ?? property.Name;
		}

		private static object GetFieldValue(FieldInfo field, object obj)
		{
			return field.GetValue(obj)?.ToString() ?? "";
		}

		private static object GetPropertyValue(PropertyInfo property, object obj)
		{
			return property.GetValue(obj)?.ToString() ?? "";
		}
	}

	/// <summary>
	/// CSVLoaderとセットで使用するアトリビュート
	/// 変数に付与するアトリビュートでcsvで記載したヘッダーの名前と紐づく
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class CSVParameterAttribute : Attribute
	{
		public string headerName;

		public CSVParameterAttribute(string headerName)
		{
			this.headerName = headerName;
		}
	}
}