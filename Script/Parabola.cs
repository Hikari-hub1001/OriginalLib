//============================================================================================================================
//
// このクラスは放物線に関する計算を行うクラスになります。
// 物理計算が関係しているクラスとなるので、挙動に違和感にある場合、
// 各オブジェクトの大きさや重力加速度の設定をご確認ください。
//
// このクラスで定義されるメソッドは全てstaticとなっているので、利用の際にインスタンスの生成を行う必要はありません。
//
//============================================================================================================================

using UnityEngine;
using System;

namespace OriginalLib
{
	/// <summary>
	/// 放物線算出クラス
	/// </summary>
	public class Parabola
	{
		//重力
		private float gravity = Physics.gravity.y;

		/// <summary>
		/// 目標地点と現在地点と時間から初速度を算出する
		/// </summary>
		/// <param name="target">目標地点</param>
		/// <param name="startPos">射出地点</param>
		/// <param name="time">到達時間</param>
		/// <returns>初速度</returns>
		public static Vector3 CalcV0(Vector3 target, Vector3 startPos, float time)
		{

			Vector3 v0 = new Vector3();

			//目標地点が無ければヌルポ
			if (target == null) { throw new NullReferenceException(); }

			Vector3 pos = target - startPos;

			v0.x = pos.x / time;

			v0.y = (float)(pos.y + 0.5 * Gravity() * Mathf.Pow(time, 2));

			return v0;
		}

		/// <summary>
		/// 目標地点、初速度、時間から射出地点を算出
		/// </summary>
		/// <param name="target">目標地点</param>
		/// <param name="v0">初速度</param>
		/// <param name="time">到達時間</param>
		/// <returns>射出地点</returns>
		public static Vector3 CalcStart(Vector3 target, Vector3 v0, float time)
		{
			Vector3 pos = new Vector3();

			//目標地点が無ければヌルポ
			if (target == null) { throw new NullReferenceException(); }

			pos.x = v0.x * time;

			pos.y = (float)(v0.y * time - 0.5 * Gravity() * Mathf.Pow(time, 2));

			return target - pos;
		}


		/// <summary>
		/// 初速度、発射地点、目標地点から射出角度を算出
		/// </summary>
		/// <param name="startPos">発射地点</param>
		/// <param name="target">目標地点</param>
		/// <param name="V0">初速度</param>
		/// <param name="gScale">重力スケール</param>
		/// <returns>射出角度（ラジアン角度）</returns>
		public static float? CalcAngle(Vector3 startPos, Vector3 target, float V0, float gScale)
		{
			float? result;

			Vector3 distance = target - startPos;

			float gravity = Gravity() * gScale;

			//x=V0*Mathf.Cos(ang)*t
			//y=V0*Mathf.Sin(ang)*t-1/2*g*t^2
			//をtを使わない式に変更
			//結果
			//y=Mathf.Tan(ang)*x-(gx^2/2*V0^2)*(Mathf.Tan(ang)^2+1)となる
			//A=(gx^2/2*V0^2)とするとMathf.Tan(ang)の2次関数になるので
			//解の公式より以下コードを実行できる

			float b = -(2 * Mathf.Pow(V0, 2)) / (gravity * Mathf.Abs(distance.x));
			float c = 1 + (2 * Mathf.Pow(V0, 2) * distance.y) / (gravity * Mathf.Pow(distance.x, 2));

			//解の公式よりDを算出
			float D = b * b - 4 * c;

			if (D >= 0)
			{
				//Dが0以上の場合ルートが成り立つ為
				//θが算出可能
				float result1 = Mathf.Atan((-b - Mathf.Sqrt(D)) / 2);
				float result2 = Mathf.Atan((-b + Mathf.Sqrt(D)) / 2);

				//2つの結果のうちより鈍角な方を射出角度として採用する
				result = Mathf.Max(result1, result2);

				//距離がマイナスの時
				if (distance.x < 0)
				{
					//Y軸に鏡写しになるように修正
					result = (180.0f * Mathf.Deg2Rad) - result;
				}
			}
			else
			{
				//0を下回る場合虚数が発生し目標地点に到達不可能となる
				//到達不可の場合は45度で発射
				result = null;
			}

			return result;
		}

		/// <summary>
		/// CalcAngleのオーバーロード.
		/// 重力スケール無しver
		/// </summary>
		/// <param name="startPos">発射地点</param>
		/// <param name="target">目標地点</param>
		/// <param name="V0">初速度</param>
		/// <returns>射出角度（ラジアン角度）</returns>
		public static float? CalcAngle(Vector3 startPos, Vector3 target, float V0)
		{
			return CalcAngle(startPos, target, V0, 1.0f);
		}

		private static float Gravity()
		{
			return -new Parabola().gravity;
		}

	}
}