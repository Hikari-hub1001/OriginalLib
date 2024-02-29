//============================================================================================================================
//
// ���̃N���X�̓V���O���g���̃R���|�[�l���g�ƂȂ�܂��B
// �C�ӂ̃N���X�Ɍp�����邱�Ƃł̂ݎg�p�\�ł��B
// �Q�ȏ�̃C���X�^���X�����������ہA���Ƃ��琶�������C���X�^���X���Q�[���I�u�W�F�N�g���Ɣj������܂��B
// �V�[���J�ڂ��s����^�C�~���O�Ŕj������܂��B
// �V�[�����܂����ŕێ��������ꍇ�́uSingleton_DontDestroy�v�N���X���g�p���Ă��������B
//
// �g����
// TestSingleton�N���X���Ƃ��܂�
// public class TestSingleton : Singleton <TestSingleton>
//
//
// Awake���\�b�h�͊�{�I�Ɏ���������Init���\�b�h���g�p���Ă��������B
// Init���\�b�h��Awake���\�b�h�ŌĂ΂�Ă���
// Awake���\�b�h�Ɠ����̓��������܂��B
//
//============================================================================================================================

using UnityEngine;

namespace OriginalLib.Behaviour
{

	/// <summary>
	/// �V���O���g���x�[�X�N���X
	/// </summary>
	/// <typeparam name="T">�V���O���g���ɂ���N���X</typeparam>
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{

		public static T Instance
		{
			private set;
			get;
		}

		/// <summary>
		/// �C���X�^���X���쐬�ς݂��`�F�b�N����
		/// </summary>
		/// <returns></returns>
		public static bool IsValid() { return Instance != null; }


		protected void Awake()
		{
			if (Instance == null)
			{
				Instance = this.GetComponent<T>();
				Init();
			}
			else if (Instance != this)
			{
				Debug.Log($"2�ڂ̃C���X�^���X���쐬����܂����B�j�����܂��B<br>{this}");
				Destroy(gameObject);
			}
		}

		/// <summary>
		/// Awake�ŏ�������������������
		/// </summary>
		protected virtual void Init() { }

		/// <summary>
		/// �I�u�W�F�N�g�j�����ɃC���X�^���X���j������
		/// </summary>
		private void OnDestroy()
		{
			if (Instance == this)
			{
				Instance = null;
			}
		}
	}
}