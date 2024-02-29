//============================================================================================================================
//
// ���̃N���X�̓V���O���g�����A�V�[���؂�ւ����ɔj������Ȃ��R���|�[�l���g�ƂȂ�܂��B
// �C�ӂ̃N���X�Ɍp�����邱�Ƃł̂ݎg�p�\�ł��B
// �Q�ȏ�̃C���X�^���X�����������ہA���Ƃ��琶�������C���X�^���X���Q�[���I�u�W�F�N�g���Ɣj������܂��B
// �V�[���؂�ւ����ɔj���������ꍇ�́uSingleton�v�N���X���g�p���Ă��������B
//
// �g����
// TestSingleton�N���X���Ƃ��܂��B
// public class TestSingleton : Singleton_DontDestroy <TestSingleton>
//
//
// Awake���\�b�h�͊�{�I�Ɏ���������Init���\�b�h���g�p���Ă��������B
// Init���\�b�h�͊��N���X�ƂȂ�Singleton�N���X��Awake���\�b�h�ŌĂ΂�Ă���
// Awake���\�b�h�Ɠ����̓��������܂��B
//
//============================================================================================================================

namespace OriginalLib.Behaviour
{
	/// <summary>
	/// �V���O���g���x�[�X�N���X
	/// DontDestroy����
	/// </summary>
	/// <typeparam name="T">�V���O���g���ɂ���N���X</typeparam>
	public abstract class Singleton_DontDestroy<T> : Singleton<T> where T : Singleton_DontDestroy<T>
	{
		protected new void Awake()
		{
			base.Awake();
			DontDestroyOnLoad(gameObject);
		}
	}
}