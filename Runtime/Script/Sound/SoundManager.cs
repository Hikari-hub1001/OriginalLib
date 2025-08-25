using OriginalLib.Behaviour;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
#if AVAILABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace OriginalLib.Sound
{
	public class SoundManager : Singleton_DontDestroy<SoundManager>
	{
		public enum ResourceType
		{
			Resources,
#if AVAILABLE_ADDRESSABLES
			Addressable,
			Both
#endif
		}

		[SerializeField]
		private ResourceType resourceType;

		private Dictionary<string, AudioClip> audioclipDic = new();

		[Header("Audio Mixer")]
		public AudioMixer audioMixer;
		[SerializeField]
		private string masterGroupName = "Master";
		[SerializeField]
		private string seGroupName = "SE";
		[SerializeField]
		private string bgmGroupName = "BGM";
		[SerializeField]
		private string voiceGroupName = "Voice";

		[Header("Audio Sources")]
		private Dictionary<string, AudioSource> bgmSourceList;
		private AudioSource seSource;
		private AudioSource voiceSource;
		private ObjectPool<AudioSource> bgmSourcePool;

		private AudioMixerGroup bgmGroup;
		private AudioMixerGroup seGroup;
		private AudioMixerGroup voiceGroup;

		[Header("Fade Settings")]
		public float defaultFadeDuration = 0.5f; // �t�F�[�h�C��/�A�E�g�̎���

		[SerializeField]
		private int maxPlaySECount = 10;
		[SerializeField]
		private int maxPlayVoiceCount = 10;

		private Coroutine bgmFadeCoroutine;

		private GameObject bgmObject;
		private GameObject seObject;
		private GameObject voiceObject;

		protected override void Init()
		{
			base.Init();

			var group = audioMixer.FindMatchingGroups(bgmGroupName);
			if (group != null && group.Length > 0)
			{
				bgmGroup = group[0];
			}
			group = audioMixer.FindMatchingGroups(seGroupName);
			if (group != null && group.Length > 0)
			{
				seGroup = group[0];
			}
			group = audioMixer.FindMatchingGroups(voiceGroupName);
			if (group != null && group.Length > 0)
			{
				voiceGroup = group[0];
			}

			//=============================================================
			// BGM�pAudioSorce�ݒ�
			//=============================================================
			bgmObject = new GameObject();
			bgmObject.name = "BGM";
			bgmObject.transform.SetParent(transform);
			bgmSourceList = new();
			bgmSourcePool = new(
				() =>
				{
					var result = bgmObject.AddComponent<AudioSource>();
					result.outputAudioMixerGroup = bgmGroup;
					result.playOnAwake = false;
					return result;
				}
				,
				(get) => get.enabled = true,
				(release) => release.enabled = false,
				(destroy) => Destroy(destroy),
				true, 2, 5);

			seObject = new GameObject();
			seObject.name = "SE";
			seObject.transform.SetParent(transform);
			voiceObject = new GameObject();
			voiceObject.name = "VOICE";
			voiceObject.transform.SetParent(transform);
		}

		// Audio Mixer�̃{�����[����ݒ�
		/// <summary>
		/// �}�X�^�[���ʐݒ�
		/// </summary>
		/// <param name="volume">�{�����[�� 0�`1</param>
		public void SetMasterVolume(float volume) => SetVolume(masterGroupName, volume);
		/// <summary>
		/// SE���ʐݒ�
		/// </summary>
		/// <param name="volume">�{�����[�� 0�`1</param>
		public void SetSEVolume(float volume) => SetVolume(seGroupName, volume);
		/// <summary>
		/// BGM���ʐݒ�
		/// </summary>
		/// <param name="volume">�{�����[�� 0�`1</param>
		public void SetBGMVolume(float volume) => SetVolume(bgmGroupName, volume);
		/// <summary>
		/// �{�C�X���ʐݒ�
		/// </summary>
		/// <param name="volume">�{�����[�� 0�`1</param>
		public void SetVoiceVolume(float volume) => SetVolume(voiceGroupName, volume);
		/// <summary>
		/// �S�{�����[���ݒ�
		/// </summary>
		/// <param name="master">�}�X�^�[����</param>
		/// <param name="bgm">BGM����</param>
		/// <param name="se">SE����</param>
		/// <param name="voice">�{�C�X����</param>
		public void SetVolume(float master, float bgm, float se, float voice)
		{
			SetMasterVolume(master);
			SetBGMVolume(bgm);
			SetSEVolume(se);
			SetVoiceVolume(voice);
		}

		protected void SetVolume(string groupName, float volume)
		{
			float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1)) * 20;
			audioMixer.SetFloat(groupName, dB);
		}

		// BGM�̍Đ��i�t�F�[�h�C���t���j
		public async void PlayBGM(string name, bool loop, float volume, bool fade, float fadeDuration)
		{
			//�Đ��ς݂Ȃ�I��
			if (bgmSourceList.ContainsKey(name)) return;

			var audio = bgmSourcePool.Get();
			bgmSourceList.Add(name, audio);

			audio.clip = await GetAudioClip(name);
			audio.loop = loop;
			if (fade)
			{
				bgmFadeCoroutine = StartCoroutine(FadeIn(audio, volume, fadeDuration));
			}
			else
			{
				audio.volume = volume;
				audio.Play();
			}

		}
		public void PlayBGM(string name, bool loop, float volume, bool fade) => PlayBGM(name, loop, volume, fade, -1.0f);

		public void PlayBGM(string name, bool loop, float volume) => PlayBGM(name, loop, volume, false, -1.0f);

		public void PlayBGM(string name, bool loop) => PlayBGM(name, loop, 1.0f, false, -1.0f);

		public void PlayBGM(string name) => PlayBGM(name, true, 1.0f, false, -1.0f);


		// BGM�̒�~�i�t�F�[�h�A�E�g�t���j
		public void StopBGM(string name, bool fade = false, float fadeDuration = -1)
		{
			if (!bgmSourceList.TryGetValue(name, out var source)) return;

			if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);

			if (fade)
			{
				bgmFadeCoroutine = StartCoroutine(FadeOut(name, source,fadeDuration));
			}
			else
			{
				source.Stop();
				bgmSourcePool.Release(source);
				bgmSourceList.Remove(name);
			}
		}

		// SE�̍Đ�
		public void PlaySE(AudioClip clip, bool loop = false, float volume = 1f, bool fade = false, float fadeDuration = -1.0f)
		{
			seSource.clip = clip;
			seSource.loop = loop;
			seSource.volume = volume;
			seSource.Play();
		}

		// �{�C�X�̍Đ��i���[�v�\�j
		public void PlayVoice(AudioClip clip, float volume = 1f, bool fade = false, float fadeDuration = -1.0f)
		{
			voiceSource.clip = clip;
			voiceSource.volume = volume;
			voiceSource.Play();
		}

		// �{�C�X�̒�~
		public void StopVoice()
		{
			voiceSource.Stop();
		}

		// �ꎞBGM�Đ���Ƀ��C��BGM���Đ�
		//public void PlayBGMWithIntro(string introName, string mainName, bool loop, float volume = 1f, bool fade = false, float fadeDuration = -1.0f)
		//{
		//	StartCoroutine(PlayTransitionBGM(introName, mainName, loop, volume, fade, fadeDuration));
		//}

		// �t�F�[�h�C��
		private IEnumerator FadeIn(AudioSource source, float targetVolume, float fadeDuration = -1.0f)
		{
			source.volume = 0f;
			source.Play();

			float elapsedTime = 0f;
			float duration = fadeDuration <= 0.0f ? this.defaultFadeDuration : fadeDuration;
			while (elapsedTime < duration)
			{
				source.volume = Mathf.Lerp(0f, targetVolume, elapsedTime / duration);
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			source.volume = targetVolume;
		}

		// �t�F�[�h�A�E�g
		private IEnumerator FadeOut(string name, AudioSource source, float fadeDuration = -1.0f)
		{
			float startVolume = source.volume;

			float elapsedTime = 0f;
			float duration = fadeDuration <= 0.0f ? this.defaultFadeDuration : fadeDuration;
			while (elapsedTime < duration)
			{
				source.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			source.volume = 0f;
			source.Stop();

			bgmSourcePool.Release(source);
			bgmSourceList.Remove(name);
		}

		// �ꎞBGM�Đ��チ�C��BGM�ɐ؂�ւ���
		//private IEnumerator PlayTransitionBGM(string introName, string mainName, bool loop, float volume, bool fade = false, float fadeDuration = -1.0f)
		//{
		//	//�C���g���Đ�
		//	PlayBGM(introName, false, volume, fade, fadeDuration);
		//	//���C���̏���
		//	PlayBGM(mainName, loop, volume, false, 0);
		//	bgmSourceList[mainName].Stop();
		//
		//	// �ꎞBGM���I������܂őҋ@
		//	yield return new WaitUntil(() => !bgmSourceList[introName].isPlaying);
		//	Debug.Log("���C��BGM�Ɉڍs");
		//	//���C�����Đ�
		//	bgmSourceList[mainName].Play();
		//	//�C���g���͔j��
		//	bgmSourcePool.Release(bgmSourceList[introName]);
		//	bgmSourceList.Remove(introName);
		//}

		private async Task<AudioClip> GetAudioClip(string name)
		{
			if (!audioclipDic.TryGetValue(name, out AudioClip clip))
			{
				if (resourceType == ResourceType.Resources)
				{
					clip = Resources.Load<AudioClip>(name);
				}
#if AVAILABLE_ADDRESSABLES
				else if (resourceType == ResourceType.Addressable)
				{
					AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(name);
					await handle.Task;
					// ���������ꍇ�A���ʂ�Ԃ�
					if (handle.Status == AsyncOperationStatus.Succeeded)
					{
						clip = handle.Result;
					}
					else
					{
						Debug.LogError($"Failed to load sound: {name}");
					}
				}
				else if (resourceType == ResourceType.Both)
				{
					//Addressable�D��
					AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(name);
					await handle.Task;
					// ���������ꍇ�A���ʂ�Ԃ�
					if (handle.Status == AsyncOperationStatus.Succeeded)
					{
						clip = handle.Result;
					}
					else
					{
						Debug.LogError($"Failed to load sound: {name}");
					}

					if (clip == null)
					{
						clip = Resources.Load<AudioClip>(name);
					}
				}
#endif
				if (clip != null)
				{
					audioclipDic.Add(name, clip);
				}
			}
			return clip;
		}
	}
}
