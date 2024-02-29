using System.Collections.Generic;
using UnityEditor;
using System.Text;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

namespace OriginalLib
{
	public class ScenesCreator
	{
		/// <summary>
		/// ���s�B
		/// </summary>
		static readonly string NEWLINE = "\n";

		/// <summary>
		/// �^�u�B
		/// </summary>
		static readonly string TAB = "\t";

		/// <summary>
		/// �t�@�C���f�B���N�g���p�X�B
		/// </summary>
		static readonly string FILE_DIRECTORY_PATH = "Assets/Scenes/Script";

		/// <summary>
		/// Scenes�t�@�C���p�X�B
		/// </summary>
		static readonly string SCENS_FILE_PATH = System.IO.Path.Combine(FILE_DIRECTORY_PATH, "Scenes.cs");

		[MenuItem("Menu/CreateScenes")]
		public static void EditorCreateScenes()
		{
			var txt = Resources.Load<TextAsset>("ScenesTemplates").text;

			var sceneNameList = LoadSceneNames();
			if (sceneNameList.Count <= 0)
			{
				return;
			}

			// ���K�\���p�^�[����ݒ�
			string pattern = @"#MenuMethodsStart#(.*?)#MenuMethodsEnd#";

			// ���K�\���Ɉ�v���邷�ׂĂ̕�����������擾
			MatchCollection matches = Regex.Matches(txt, pattern, RegexOptions.Singleline);

			string methodPart = "";
			string capturedTxt = "";
			// ��v���������������\��
			foreach (Match match in matches)
			{
				capturedTxt = match.Value;
				methodPart = match.Groups[1].Value;
			}

			string scenes = NEWLINE;
			string replaceTxt = "";

			foreach (var item in sceneNameList)
			{
				scenes += TAB;
				scenes += item.name + "," + NEWLINE;
				replaceTxt += methodPart.Replace("#SceneName#", item.name)
										.Replace("#ScenePath#", item.path)
										.Replace("#SceneFullPath#", item.fullPath);
			}

			txt = txt.Replace("#ScenesEnum#", scenes);
			txt = txt.Replace(capturedTxt, replaceTxt);

			System.IO.File.WriteAllText(SCENS_FILE_PATH, txt, System.Text.Encoding.UTF8);
		}


		/// <summary>
		/// �V�[�����ꗗ���擾�B
		/// </summary>
		/// <returns>�V�[�����ꗗ�B</returns>
		static List<(string name, string path, string fullPath)> LoadSceneNames()
		{
			var list = new List<(string name, string path, string fullPath)>();
			foreach (var scenes in EditorBuildSettings.scenes)
			{
				// �L���Ȃ��́B
				if (scenes.enabled)
				{
					// �X���b�V������h�b�g�̊Ԃ��擾�B
					var slash = scenes.path.LastIndexOf("/");
					var dot = scenes.path.LastIndexOf(".");
					var name = scenes.path.Substring(slash + 1, dot - slash - 1);
					var path = scenes.path.Replace("Assets/Scenes/", "").Replace(".unity", "");
					list.Add((name, path, scenes.path));
				}
			}
			return list;
		}

	}
}