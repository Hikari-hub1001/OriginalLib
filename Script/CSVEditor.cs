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
		/// CSV�t�@�C����ǂݍ��݁A�N���X�֊i�[����
		/// CSVParameterAttribute�����Ƃɏ��ƕϐ���R�Â���
		/// CSV�t�@�C����StreaminAssets�t�H���_�ɔz�u���邱�ƁB
		/// �p�X���[�h��ݒ肵���ꍇ����������
		/// </summary>
		/// <typeparam name="T">CSV�̏����󂯎��N���X</typeparam>
		/// <param name="fileName">CSV�t�@�C���̃p�X�iStreaminAssets�ȉ��j</param>
		/// <param name="password">�������p�p�X���[�h</param>
		/// <returns>�i�[�����N���X���X�g</returns>
		public static List<T> ReadCSV<T>(string fileName, string password = null) where T : new()
		{
			List<T> dataList = new List<T>();

			string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

			using (var sr = new StreamReader(filePath, System.Text.Encoding.UTF8))
			{
				//�����̉��s���폜����
				var fileVal = sr.ReadToEnd().TrimEnd('\r', '\n').Split("\r\n");

				//�p�X���[�h���ݒ肳��Ă���ꍇ�͈Í�������
				if (!string.IsNullOrEmpty(password))
				{
					fileVal = Encryption.Decrypt(fileVal[0], password).Split("\r\n");
				}

				//�w�b�_�[�A�f�[�^�ōŒ�2�s���̃f�[�^���擾�ł��Ȃ��ꍇ�̓t�H�[�}�b�g�ɖ�肪����
				if (fileVal.Length < 2)
				{
					throw new FormatException("The format of this file is incorrect.");
				}

				// �w�b�_�[�s��ǂݍ���
				//var headerLine = sr.ReadLine();
				var headerLine = fileVal[0];
				var headers = headerLine.Split(',');

				// �w�b�_�[���ƕϐ����̑Ή����쐬
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

				// �e�s��ǂݍ���
				foreach (string line in fileVal[1..fileVal.Length])
				{
					//var line = sr.ReadLine();
					var values = line/*.Replace("<br>", "\r\n")*/.Split(',');//<br>�����s�R�[�h�ɍ����ւ���

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
									values[i] = values[i].Substring(1, values[i].Length - 2);//�ŏ��ƍŌ���폜
								}
								if (values[i].Contains("\"\""))
								{
									values[i] = values[i].Replace("\"\"", "\"");//[""]���g���Ă�����["]�ɒu������
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
		/// �ϐ�����CSV�ɏ�������
		/// CSVParameterAttribute�����Ƃɏ��ƕϐ���R�Â���
		/// CSV�t�@�C����StreaminAssets�t�H���_�ɔz�u���邱�ƁB
		/// �p�X���[�h�ݒ肵���ꍇ�͈Í������ĕۑ�����
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fileName">�ۑ�����t�@�C���p�X</param>
		/// <param name="data">�ۑ�����f�[�^</param>
		/// <param name="password">�Í����p�X���[�h</param>
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
					// �w�b�_�[����������
					var header = fields.Select(field => GetFieldName(field)).Concat(properties.Select(property => GetPropertyName(property)));
					var s = string.Join(",", header);
					List<string> values = new List<string>();
					values.Add(s);
					// �f�[�^����������
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
									//["]��[""]�ɕύX
									str = str.Replace("\"", "\"\"");
								}
								if (str.Contains("\r\n") || str.Contains("\n"))
								{
									//�܂��͉��s�R�[�h��ύX
									str = str.Replace("\r\n", "\n");
									//�O���["]��ǉ�
									str = "\"" + str + "\"";
								}

								line[i] = (object)str;
							}
						}
						values.Add(string.Join(",", line));
					}

					string st = string.Join("\r\n", values);

					//�p�X���[�h���ݒ肳��Ă���ꍇ�͕�������
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
	/// CSVLoader�ƃZ�b�g�Ŏg�p����A�g���r���[�g
	/// �ϐ��ɕt�^����A�g���r���[�g��csv�ŋL�ڂ����w�b�_�[�̖��O�ƕR�Â�
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