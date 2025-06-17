using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OriginalLib.Hobby
{
	[CustomEditor(typeof(InspectorMineSweeper))]
	public class InspectorMineSweeperEditor : Editor
	{
		bool playing = false;
		DateTime startTime;
		CellData[,] cells = new CellData[10, 10];

		readonly float CELL_SIZE = 30.0f;
		readonly Vector2Int[] vecs =
{
				new Vector2Int(-1,-1), new Vector2Int(0,-1),  new Vector2Int(1,-1),
				new Vector2Int(-1,0),                         new Vector2Int(1,0),
				new Vector2Int(-1,1),  new Vector2Int(0,1),   new Vector2Int(1,1)
			};

		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginHorizontal();
			if (!playing && GUILayout.Button("Start"))
			{
				playing = true;
				startTime = DateTime.Now;
				InisCell();
			}
			else if (playing && GUILayout.Button("ReStart"))
			{
				playing = true;
				startTime = DateTime.Now;
				InisCell();
			}
			else if (playing && GUILayout.Button("Stop"))
			{
				playing = false;
			}
			EditorGUILayout.EndHorizontal();
			DrawCells();
		}

		private void InisCell()
		{
			int[] bombs = RandomUtil.GetUniqueRandomNumbers(0, 100, 10) as int[];
			//bomb
			for (int i = 0; i < cells.GetLength(0); i++)
			{
				for (int j = 0; j < cells.GetLength(1); j++)
				{
					cells[i, j] = new();
					foreach (int bomb in bombs)
					{
						if (bomb / 10 == i && bomb % 10 == j)
						{
							cells[i, j].num = -1;
						}
					}
					cells[i, j].open = false;
					cells[i, j].flag = false;
				}
			}
			//number

			for (int i = 0; i < cells.GetLength(0); i++)
			{
				for (int j = 0; j < cells.GetLength(1); j++)
				{
					if (cells[i, j].num == -1) continue;
					foreach (Vector2Int vec in vecs)
					{
						cells[i, j].num += BombCheck(i + vec.x, j + vec.y) ? 1 : 0;
					}
				}
			}
		}

		private void DrawCells()
		{
			if (!playing)
			{
				if (CrearCheck())
				{
					EditorGUILayout.LabelField("クリアー");
				}
				else
				{
					EditorGUILayout.LabelField("ゲームオーバー");
				}
				//return;
			}
			if (CrearCheck())
			{
				Debug.Log("クリア");
				playing = false;
				//return;
			}

			EditorGUILayout.BeginVertical();
			for (int i = 0; i < cells.GetLength(0); i++)
			{
				EditorGUILayout.BeginHorizontal();
				for (int j = 0; j < cells.GetLength(1); j++)
				{
					bool f = ((cells[i, j]?.open).HasValue && (cells[i, j]?.open).Value);
					bool flag = (cells[i, j]?.flag).HasValue && (cells[i, j]?.flag).Value;
					string str = f && cells[i, j]?.num > 0 ? cells[i, j]?.num.ToString() : (flag ? "P" : "");
					EditorGUI.BeginDisabledGroup(f || !playing);
					if (cells[i, j]?.num != -1 && GUILayout.Button(str, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE)))
					{
						if (Event.current.button == 0)
							OpenCell(i, j);
						else if (Event.current.button == 1)
							SetFlag(i, j);
					}
					if (cells[i, j]?.num == -1 && GUILayout.Button(f ? "*" : (flag ? "P" : ""), GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE)))
					{
						if (Event.current.button == 0)
						{
							if (!cells[i, j].flag)
							{
								cells[i, j].open = true;
								playing = false;
							}
						}
						else if (Event.current.button == 1)
							SetFlag(i, j);
					}
					EditorGUI.EndDisabledGroup();
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}


		private bool BombCheck(int x, int y)
		{
			try
			{
				if (cells[x, y].num == -1) return true;
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}
			return false;
		}

		private void OpenCell(int x, int y)
		{
			try
			{
				if (cells[x, y].open) return;
				if (cells[x, y].flag) return;
				cells[x, y].open = true;

				if (cells[x, y].num > 0) return;

				foreach (var vec in vecs)
				{
					OpenCell(x + vec.x, y + vec.y);
				}
			}
			catch (IndexOutOfRangeException)
			{
				return;
			}
		}

		private void SetFlag(int x, int y)
		{
			try
			{
				if (cells[x, y].open) return;
				cells[x, y].flag = !cells[x, y].flag;
			}
			catch (IndexOutOfRangeException)
			{
				return;
			}
		}

		private bool CrearCheck()
		{
			var result = cells?.Cast<CellData>().Where((f) => !((f?.open).HasValue && (f?.open).Value));
			var bomb = cells?.Cast<CellData>().Where((f) => !((f?.open).HasValue && (f?.open).Value) && f?.num == -1);
			return result?.Count() == bomb?.Count() && bomb?.Count() == 10;
		}

		class CellData
		{
			public int num;
			public bool open;
			public bool flag;
		}
	}
}