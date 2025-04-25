using OriginalLib.Behaviour;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace OriginalLib.Sound
{
	public class SoundManager : Singleton_DontDestroy<SoundManager>
	{
		[Serializable]
		private class AudioPlayerSet
		{
			public string GroupName;
			public int MaxCount = 5;
			private AudioMixerGroup mixerGroup;
			[HideInInspector]
			public List<AudioSource> SourceList;

			private GameObject audioObj;

			public AudioPlayerSet(string groupName, int maxCount)
			{
				GroupName = groupName;
				MaxCount = maxCount;
			}

			public void Initialize(AudioMixer mixer)
			{
				var groupes = mixer.FindMatchingGroups(GroupName);
				if (groupes != null && groupes.Length > 0)
				{
					mixerGroup = groupes[0];
				}

				audioObj = new() { name = GroupName + " Obj" };
				audioObj.transform.parent = Instance.transform;
				SourceList = new();
				for (int i = 0; i < MaxCount; i++)
				{
					SourceList.Add(audioObj.AddComponent<AudioSource>());
					SourceList[i].outputAudioMixerGroup = mixerGroup;
				}
			}
		}

		[Header("Audio Mixer")]
		public AudioMixer audioMixer;
		private readonly string MasterGroupName = "Master";
		private readonly string SEGroupName = "SE";
		private readonly string BGMGroupName = "BGM";

		[Header("Audio Sources")]
		private AudioPlayerSet bgmSource;
		private AudioPlayerSet sePlayerSet;
		[SerializeField]
		private List<AudioPlayerSet> customPlayerList;

		private AudioMixerGroup bgmGroup;

		[Header("Fade Settings")]
		public float defaultFadeDuration = 0.5f; // �t�F�[�h�C��/�A�E�g�̎���

		[SerializeField]
		private int maxPlaySECount = 10;

		private Coroutine bgmFadeInCoroutine;
		private Coroutine bgmFadeOutCoroutine;


		protected override void Init()
		{
			base.Init();
			var groupes = audioMixer.FindMatchingGroups(BGMGroupName);
			if (groupes != null && groupes.Length > 0)
			{
				bgmGroup = groupes[0];
			}
			//�J�X�^���O���[�v
			sePlayerSet = new(SEGroupName, maxPlaySECount);
			sePlayerSet.Initialize(audioMixer);

			foreach (var ap in customPlayerList)
			{
				ap.Initialize(audioMixer);
			}
		}

		/// <summary>
		/// �}�X�^�[�{�����[���ݒ�
		/// </summary>
		/// <param name="volume">�{�����[���@0�`1</param>
		public void SetMasterVolume(float volume) => SetVolume(MasterGroupName, volume);
		/// <summary>
		/// SE�{�����[���ݒ�
		/// </summary>
		/// <param name="volume">�{�����[���@0�`1</param>
		public void SetSEVolume(float volume) => SetVolume(SEGroupName, volume);
		/// <summary>
		/// BGM�{�����[���ݒ�
		/// </summary>
		/// <param name="volume">�{�����[���@0�`1</param>
		public void SetBGMVolume(float volume) => SetVolume(BGMGroupName, volume);
		/// <summary>
		/// �{�����[���ݒ�
		/// </summary>
		/// <param name="groupName">�~�L�T�[�O���[�v��</param>
		/// <param name="volume">�{�����[���@0�`1</param>
		public void SetVolume(string groupName, float volume)
		{
			float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1)) * 20;
			audioMixer.SetFloat(groupName, dB);
		}

		/// <summary>
		/// �}�X�^�[�{�����[���̎擾
		/// </summary>
		/// <returns>�{�����[���@0�`1</returns>
		public float GetMasterVolume() => GetVolume(MasterGroupName);
		/// <summary>
		/// SE�{�����[���̎擾
		/// </summary>
		/// <returns>�{�����[���@0�`1</returns>
		public float GetSEVolume() => GetVolume(SEGroupName);
		/// <summary>
		/// BGM�{�����[���̎擾
		/// </summary>
		/// <returns>�{�����[���@0�`1</returns>
		public float GetBGMVolume() => GetVolume(BGMGroupName);
		/// <summary>
		/// �{�����[���̎擾<br>
		/// �擾�ł��Ȃ������ꍇ��0
		/// </summary>
		/// <param name="groupName">�~�L�T�[�O���[�v��</param>
		/// <returns>�{�����[���@0�`1</returns>
		public float GetVolume(string groupName)
		{
			if (audioMixer.GetFloat(groupName, out float dB))
			{
				float linear = Mathf.Pow(10f, dB / 20f);
				return Mathf.Clamp01(linear);
			}
			else
			{
				// �擾�ł��Ȃ������ꍇ��0�i�����j�Ƃ���
				return 0f;
			}
		}

		/// <summary>
		/// BGM�̍Đ�
		/// </summary>
		/// <param name="clip">�Đ�BGM</param>
		/// <param name="loop">���[�v</param>
		/// <param name="volume">�{�����[��</param>
		/// <param name="fade">�t�F�[�h</param>
		/// <param name="fadeDuration">�t�F�[�h����</param>
		public void PlayBGM(AudioClip clip, bool loop, float volume, bool fade, float fadeDuration)
		{
			if (bgmFadeInCoroutine != null) StopCoroutine(bgmFadeInCoroutine);

			//����BGM���~�߂�
			StopBGM(fade, fadeDuration);

			//���C���Ŗ炷���̂�0�Ԗ�
			bgmSource.SourceList[0].clip = clip;
			bgmSource.SourceList[0].loop = loop;
			if (fade)
			{
				bgmFadeInCoroutine = StartCoroutine(FadeIn(bgmSource.SourceList[0], volume, fadeDuration));
			}
			else
			{
				bgmSource.SourceList[0].volume = volume;
				bgmSource.SourceList[0].Play();
			}
		}
		/// <summary>
		/// BGM�̍Đ�
		/// </summary>
		/// <param name="clip">�Đ�BGM</param>
		/// <param name="loop">���[�v</param>
		/// <param name="volume">�{�����[��</param>
		/// <param name="fade">�t�F�[�h</param>
		public void PlayBGM(AudioClip clip, bool loop, float volume, bool fade) => PlayBGM(clip, loop, volume, fade, -1.0f);
		/// <summary>
		/// BGM�̍Đ�
		/// </summary>
		/// <param name="clip">�Đ�BGM</param>
		/// <param name="loop">���[�v</param>
		/// <param name="volume">�{�����[��</param>
		public void PlayBGM(AudioClip clip, bool loop, float volume) => PlayBGM(clip, loop, volume, false, -1.0f);
		/// <summary>
		/// BGM�̍Đ�
		/// </summary>
		/// <param name="clip">�Đ�BGM</param>
		/// <param name="loop">���[�v</param>
		public void PlayBGM(AudioClip clip, bool loop) => PlayBGM(clip, loop, 1.0f, false, -1.0f);
		/// <summary>
		/// BGM�̍Đ�
		/// </summary>
		/// <param name="clip">�Đ�BGM</param>
		public void PlayBGM(AudioClip clip) => PlayBGM(clip, true, 1.0f, false, -1.0f);


		/// <summary>
		/// BGM�̒�~
		/// </summary>
		/// <param name="fade">��~�̃t�F�[�h</param>
		/// <param name="fadeDuration">�t�F�[�h����</param>
		public void StopBGM(bool fade = false, float fadeDuration = -1)
		{
			if (bgmFadeOutCoroutine != null) StopCoroutine(bgmFadeOutCoroutine);

			//�~�߂�̂�0�Ԗ�
			if (fade)
			{
				bgmFadeOutCoroutine = StartCoroutine(FadeOut(bgmSource.SourceList[0]));
			}
			else
			{
				bgmSource.SourceList[0].Stop();
			}

			//�~�߂鏈�����Ă񂾌��1�ԖڂɈړ�������
			(bgmSource.SourceList[0], bgmSource.SourceList[1]) = (bgmSource.SourceList[1], bgmSource.SourceList[0]);
		}

		/// <summary>
		/// SE���Đ�
		/// </summary>
		/// <param name="clip">�Đ��t�@�C��</param>
		/// <param name="loop">���[�v</param>
		/// <param name="volume">�{�����[��</param>
		public void PlaySE(AudioClip clip, bool loop = false, float volume = 1f) => PlayWithSet(sePlayerSet, clip, loop, volume, false, -1);
		/// <summary>
		/// SE���~
		/// </summary>
		/// <param name="clip">��~�t�@�C��</param>
		/// <returns>��~�̌���</returns>
		public bool StopSE(AudioClip clip) => StopWithSet(sePlayerSet, clip);
		/// <summary>
		/// �SSE���~
		/// </summary>
		public void StopAllSE() => StopAllWithSet(sePlayerSet);

		/// <summary>
		/// BGM �C���g������var
		/// </summary>
		/// <param name="introBGM">�C���g��BGM</param>
		/// <param name="mainBGM">���C��BGM</param>
		/// <param name="volume">�{�����[��</param>
		/// <param name="fade">�C���g���̃t�F�[�h�C��</param>
		/// <param name="fadeDuration">�t�F�[�h����</param>
		public void PlayBGMWithIntro(AudioClip introBGM, AudioClip mainBGM, float volume = 1f, bool fade = false, float fadeDuration = -1.0f)
		{
			StartCoroutine(PlayTransitionBGM(introBGM, mainBGM, volume, fade, fadeDuration));
		}

		/// <summary>
		/// �I�[�f�B�I�~�L�T�[�O���[�v���w�肵�čĐ�
		/// </summary>
		/// <param name="audioMixerGroup">�I�[�f�B�I�~�L�T�[��</param>
		/// <param name="clip">�Đ��t�@�C��</param>
		/// <param name="loop">���[�v</param>
		/// <param name="volume">�{�����[��</param>
		/// <param name="fade">�t�F�[�h�L��</param>
		/// <param name="fadeDuration">�t�F�[�h����</param>
		public void PlaySoundWithGroup(string audioMixerGroup, AudioClip clip, bool loop = false, float volume = 1f, bool fade = false, float fadeDuration = -1.0f)
		{
			foreach (var item in customPlayerList)
			{
				if (item.GroupName == audioMixerGroup)
				{
					PlayWithSet(item, clip, loop, volume, fade, fadeDuration);
					return;
				}
			}
		}

		private void PlayWithSet(AudioPlayerSet audio, AudioClip clip, bool loop, float volume, bool fade, float fadeDuration)
		{
			if (!StopWithSet(audio, clip))
				sePlayerSet.SourceList[0].Stop();
			sePlayerSet.SourceList[0].clip = clip;
			sePlayerSet.SourceList[0].loop = loop;
			if (fade)
			{
				StartCoroutine(FadeIn(sePlayerSet.SourceList[0], volume, fadeDuration));
			}
			else
			{
				sePlayerSet.SourceList[0].volume = volume;
				sePlayerSet.SourceList[0].Play();
			}
			//�Đ���������ɉ�
			var source = audio.SourceList[0];
			audio.SourceList.RemoveAt(0);
			audio.SourceList.Add(source);
		}
		private bool StopWithSet(AudioPlayerSet audio, AudioClip clip)
		{
			for (int i = 0; i < audio.SourceList.Count; i++)
			{
				if (audio.SourceList[i].clip == clip)
				{
					//�Đ��ς݂�����Ύ~�߂Đ擪�Ɏ����Ă���
					var source = audio.SourceList[i];
					audio.SourceList.RemoveAt(i);
					audio.SourceList.Insert(0, source);
					source.Stop();
					source.clip = null;
					return true;
				}
			}
			return false;
		}
		private void StopAllWithSet(AudioPlayerSet audio)
		{
			for (int i = 0; i < audio.SourceList.Count; i++)
			{
				if (audio.SourceList[i].clip != null)
				{
					audio.SourceList[i].Stop();
					audio.SourceList[i].clip = null;
				}
			}
		}

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
		private IEnumerator FadeOut(AudioSource source)
		{
			float startVolume = source.volume;

			float elapsedTime = 0f;
			while (elapsedTime < defaultFadeDuration)
			{
				source.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / defaultFadeDuration);
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			source.volume = 0f;
			source.Stop();
		}

		// �ꎞBGM�Đ��チ�C��BGM�ɐ؂�ւ���
		private IEnumerator PlayTransitionBGM(AudioClip introBGM, AudioClip mainBGM, float volume, bool fade = false, float fadeDuration = -1.0f)
		{
			PlayBGM(introBGM, false, volume, fade, fadeDuration);

			// �ꎞBGM���I������܂őҋ@
			yield return new WaitUntil(() => !bgmSource.SourceList[0].isPlaying);

			// ���C��BGM�ɃX���[�Y�ɐ؂�ւ�
			PlayBGM(mainBGM, true, volume, false, 0);
		}


		public void Test(AudioClip clip)
		{
			PlaySE(clip);
		}
	}
}
