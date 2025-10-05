using OriginalLib.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
namespace OriginalLib.DebugTools
{
	internal class LogViewer : Singleton_DontDestroy<LogViewer>
	{
		class LogData
		{
			public DateTime DateTime;
			public string LogString;
			public string StackTrace;
			public LogType Type;
			public bool LogOpen;

			public LogData(DateTime datetime, string logString, string stackTrace, LogType type)
			{
				DateTime = datetime;
				LogString = logString;
				StackTrace = stackTrace;
				Type = type;
				LogOpen = false;
			}
		}

		[field: SerializeField]
		private int maxLogCount { get; } = 100;
		private Queue<LogData> logQueue = new Queue<LogData>();

		private bool ShowNormalLog = true;
		private bool ShowWarningLog = true;
		private bool ShowErrorLog = true;

		private Vector2 scrollPosition;

		private Texture2D TexNormalLog;
		private Texture2D TexWarningLog;
		private Texture2D TexErrorLog;

		private bool ShowLogWindow = false;

		protected override void Init()
		{
			Application.logMessageReceived += HandleLog;

			TexNormalLog = Resources.Load<Texture2D>("Texture/Debug/console_infoicon");
			TexWarningLog = Resources.Load<Texture2D>("Texture/Debug/console_warnicon");
			TexErrorLog = Resources.Load<Texture2D>("Texture/Debug/console_erroricon");
		}
		void HandleLog(string logString, string stackTrace, LogType type)
		{
			if (logQueue.Count >= maxLogCount)
				logQueue.Dequeue();

			logQueue.Enqueue(new(DateTime.Now, logString, stackTrace, type));
		}

		void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			DrawMenu();
			if (ShowLogWindow)
			{
				DrawLogList();
			}
		}

		private void DrawMenu()
		{
			float height = Screen.height * 0.05f;
			float width = height * 2.0f;
			float posX = 10.0f;
			float posY = 10.0f;
			int labelSize = (int)(height * 0.6f);
			Rect buttonRect = new(posX, posY, width, height);
			Rect imageRect = Rect.zero;
			Rect labelRect = Rect.zero;

			GUI.skin.button.fontSize = labelSize;

			if (ShowLogWindow)
			{
				//ログクリアボタン
				if (GUI.Button(buttonRect, "Clear"))
				{
					logQueue.Clear();
				}

				// ノーマルログボタン
				posX += width + 10.0f;
				buttonRect.Set(posX, posY, width, height);
				if (!ShowNormalLog) GUI.color = Color.black;
				if (GUI.Button(buttonRect, ""))
				{
					ShowNormalLog = !ShowNormalLog;
				}
				GUI.color = Color.white;
				imageRect.Set(posX + 10.0f, posY + height * 0.2f, labelSize, labelSize);
				GUI.DrawTexture(imageRect, TexNormalLog, ScaleMode.ScaleToFit, true);
				labelRect.Set(imageRect.x + imageRect.width + 10.0f, imageRect.y, width - labelSize - 10.0f, labelSize);
				GUI.Label(labelRect, logQueue.Count((l) => l.Type == LogType.Log).ToString());

				// 警告ログボタン
				posX += width + 10.0f;
				buttonRect.Set(posX, posY, width, height);
				if (!ShowWarningLog) GUI.color = Color.black;
				if (GUI.Button(buttonRect, ""))
				{
					ShowWarningLog = !ShowWarningLog;
				}
				GUI.color = Color.white;
				imageRect.Set(posX + 10.0f, posY + height * 0.2f, labelSize, labelSize);
				GUI.DrawTexture(imageRect, TexWarningLog, ScaleMode.ScaleToFit, true);
				labelRect.Set(imageRect.x + imageRect.width + 10.0f, imageRect.y, width - labelSize - 10.0f, labelSize);
				GUI.Label(labelRect, logQueue.Count((l) => l.Type == LogType.Warning).ToString());

				// エラーログボタン
				posX += width + 10.0f;
				buttonRect.Set(posX, posY, width, height);
				if (!ShowErrorLog) GUI.color = Color.black;
				if (GUI.Button(buttonRect, ""))
				{
					ShowErrorLog = !ShowErrorLog;
				}
				GUI.color = Color.white;
				imageRect.Set(posX + 10.0f, posY + height * 0.2f, labelSize, labelSize);
				GUI.DrawTexture(imageRect, TexErrorLog, ScaleMode.ScaleToFit, true);
				labelRect.Set(imageRect.x + imageRect.width + 10.0f, imageRect.y, width - labelSize - 10.0f, labelSize);
				GUI.Label(labelRect, logQueue.Count((l) => l.Type == LogType.Error || l.Type == LogType.Assert || l.Type == LogType.Exception).ToString());

			}
			//Closeボタン
			buttonRect.Set(Screen.width - height - 10.0f, posY, height, height);
			if (GUI.Button(buttonRect, ShowLogWindow ? "X" : "L"))
			{
				ShowLogWindow = !ShowLogWindow;
			}
		}

		private void DrawLogList()
		{
			float iconSize = ((Screen.width > Screen.height) ? Screen.width : Screen.height) / 50;
			float margin = 5f;
			float buttonHeight = iconSize + margin * 2;
			float buttonWidth = Screen.width - 20f;
			GUI.skin.label.fontSize = (int)iconSize;


			Rect area = new Rect(10, Screen.height * 0.05f + 20, buttonWidth, Screen.height * 0.95f - 30);
			GUILayout.BeginArea(area, GUI.skin.box);

			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

			foreach (var log in logQueue)
			{

				if (log.Type == LogType.Log && !ShowNormalLog) continue;
				else if (log.Type == LogType.Warning && !ShowWarningLog) continue;
				else if ((log.Type == LogType.Assert || log.Type == LogType.Error || log.Type == LogType.Exception) && !ShowErrorLog) continue;
				// ボタン全体の領域
				Rect buttonRect = GUILayoutUtility.GetRect(buttonWidth - 50, buttonHeight);

				// クリック処理
				if (GUI.Button(buttonRect, GUIContent.none, GUIStyle.none))
				{
					log.LogOpen = !log.LogOpen;
				}

				// アイコン描画
				Texture2D icon = log.Type switch
				{
					LogType.Error => TexErrorLog,
					LogType.Assert => TexErrorLog,
					LogType.Warning => TexWarningLog,
					LogType.Log => TexNormalLog,
					LogType.Exception => TexErrorLog,
					_ => null
				};

				if (icon != null)
				{
					Rect iconRect = new Rect(
						buttonRect.x + margin,
						buttonRect.y + margin,
						iconSize,
						iconSize
					);
					GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true);
				}

				// テキスト描画（アイコンの右側）
				float textX = buttonRect.x + iconSize + margin * 2;
				Rect textRect = new Rect(
					textX,
					buttonRect.y,
					buttonRect.width - (textX - buttonRect.x) - margin,
					iconSize + 10
				);

				GUI.Label(textRect, $"[{log.DateTime:HH:mm:ss}] {log.LogString}");

				// 詳細スタックトレース（開いてたら）
				if (log.LogOpen)
				{
					GUILayout.Space(4);
					GUILayout.BeginHorizontal();
					GUILayout.Space(iconSize); // ← インデント量（ピクセル数）
					if (GUILayout.Button(log.StackTrace, GUI.skin.label)) log.LogOpen = !log.LogOpen;
					GUILayout.EndHorizontal();
				}
			}

			GUILayout.EndScrollView();
			GUILayout.EndArea();

		}
	}
}
#endif
