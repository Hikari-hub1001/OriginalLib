using OriginalLib.Behaviour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace OriginalLib.Sound
{
	public class SoundManager : Singleton_DontDestroy<SoundManager>
	{

		[Header("Audio Mixer")]
		public AudioMixer audioMixer;
		private readonly string MasterGroupName = "Master";
		private readonly string SEGroupName = "SE";
		private readonly string BGMGroupName = "BGM";
		private readonly string VoiceGroupName = "Voice";

		[Header("Audio Sources")]
		private AudioSource bgmSource;
		private AudioSource seSource;
		private AudioSource voiceSource;

		private AudioMixerGroup bgmGroup;
		private AudioMixerGroup seGroup;
		private AudioMixerGroup voiceGroup;

		[Header("Fade Settings")]
		public float defaultFadeDuration = 0.5f; // フェードイン/アウトの時間

		[SerializeField]
		private int maxPlaySECount = 10;
		[SerializeField]
		private int maxPlayVoiceCount = 10;

		private Coroutine bgmFadeCoroutine;

		protected override void Init()
		{
			base.Init();
			bgmGroup = audioMixer.FindMatchingGroups(BGMGroupName)[0];
			seGroup = audioMixer.FindMatchingGroups(SEGroupName)[0];
			voiceGroup = audioMixer.FindMatchingGroups(VoiceGroupName)[0];

			bgmSource = gameObject.AddComponent<AudioSource>();
			bgmSource.outputAudioMixerGroup = bgmGroup;
		}

		// Audio Mixerのボリュームを設定
		public void SetMasterVolume(float volume) => SetVolume(MasterGroupName, volume);
		public void SetSEVolume(float volume) => SetVolume(SEGroupName, volume);
		public void SetBGMVolume(float volume) => SetVolume(BGMGroupName, volume);
		public void SetVoiceVolume(float volume) => SetVolume(VoiceGroupName, volume);
		protected void SetVolume(string groupName, float volume)
		{
			float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1)) * 20;
			audioMixer.SetFloat(groupName, dB);
		}

		// BGMの再生（フェードイン付き）
		public void PlayBGM(AudioClip clip, bool loop, float volume, bool fade, float fadeDuration)
		{
			if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);

			bgmSource.clip = clip;
			bgmSource.loop = loop;
			if (fade)
			{
				bgmFadeCoroutine = StartCoroutine(FadeIn(bgmSource, volume, fadeDuration));
			}
			else
			{
				bgmSource.volume = volume;
				bgmSource.Play();
			}
		}
		public void PlayBGM(AudioClip clip, bool loop, float volume, bool fade) => PlayBGM(clip, loop, volume, fade, -1.0f);

		public void PlayBGM(AudioClip clip, bool loop, float volume) => PlayBGM(clip, loop, volume, false, -1.0f);

		public void PlayBGM(AudioClip clip, bool loop) => PlayBGM(clip, loop, 1.0f, false, -1.0f);

		public void PlayBGM(AudioClip clip) => PlayBGM(clip, true, 1.0f, false, -1.0f);


		// BGMの停止（フェードアウト付き）
		public void StopBGM(bool fade = false, float fadeDuration = -1)
		{
			if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);

			if (fade)
			{
				bgmFadeCoroutine = StartCoroutine(FadeOut(bgmSource));
			}
			else
			{

			}
		}

		// SEの再生
		public void PlaySE(AudioClip clip, bool loop = false, float volume = 1f, bool fade = false, float fadeDuration = -1.0f)
		{
			seSource.clip = clip;
			seSource.loop = loop;
			seSource.volume = volume;
			seSource.Play();
		}

		// ボイスの再生（ループ可能）
		public void PlayVoice(AudioClip clip, float volume = 1f, bool fade = false, float fadeDuration = -1.0f)
		{
			voiceSource.clip = clip;
			voiceSource.volume = volume;
			voiceSource.Play();
		}

		// ボイスの停止
		public void StopVoice()
		{
			voiceSource.Stop();
		}

		// 一時BGM再生後にメインBGMを再生
		public void PlayBGMWithTransition(AudioClip temporaryBGM, AudioClip mainBGM, float volume = 1f, bool fade = false, float fadeDuration = -1.0f)
		{
			StartCoroutine(PlayTransitionBGM(temporaryBGM, mainBGM, volume));
		}

		// フェードイン
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

		// フェードアウト
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

		// 一時BGM再生後メインBGMに切り替える
		private IEnumerator PlayTransitionBGM(AudioClip temporaryBGM, AudioClip mainBGM, float volume, bool fade = false, float fadeDuration = -1.0f)
		{
			PlayBGM(temporaryBGM, false, volume, fade, fadeDuration);

			// 一時BGMが終了するまで待機
			yield return new WaitUntil(() => !bgmSource.isPlaying);

			// メインBGMにスムーズに切り替え
			PlayBGM(mainBGM, true, volume, false, 0);
		}
	}
}
