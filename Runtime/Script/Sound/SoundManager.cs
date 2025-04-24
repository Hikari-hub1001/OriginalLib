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

		[Header("Audio Sources")]
		private AudioSource bgmSource;
		private List<AudioSource> seSourceList;

		private AudioMixerGroup bgmGroup;
		private AudioMixerGroup seGroup;

		[Header("Fade Settings")]
		public float defaultFadeDuration = 0.5f; // フェードイン/アウトの時間

		[SerializeField]
		private int maxPlaySECount = 10;

		private Coroutine bgmFadeCoroutine;


		protected override void Init()
		{
			base.Init();
			var groupes = audioMixer.FindMatchingGroups(BGMGroupName);
			if (groupes != null && groupes.Length > 0)
			{
				bgmGroup = groupes[0];
			}
			groupes = audioMixer.FindMatchingGroups(SEGroupName);
			if (groupes != null && groupes.Length > 0)
			{
				seGroup = groupes[0];
			}

			var bgmobj = new GameObject();
			bgmobj.name = "BGM Object";
			bgmobj.transform.parent = transform;
			bgmSource = bgmobj.AddComponent<AudioSource>();
			bgmSource.outputAudioMixerGroup = bgmGroup;
			var seobj = new GameObject();
			seobj.name = "SE Object";
			seobj.transform.parent = transform;
			seSourceList = new();
			for (int i = 0; i < maxPlaySECount; i++)
			{
				seSourceList.Add(seobj.AddComponent<AudioSource>());
				seSourceList[i].outputAudioMixerGroup = seGroup;
			}
		}

		// Audio Mixerのボリュームを設定
		public void SetMasterVolume(float volume) => SetVolume(MasterGroupName, volume);
		public void SetSEVolume(float volume) => SetVolume(SEGroupName, volume);
		public void SetBGMVolume(float volume) => SetVolume(BGMGroupName, volume);
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
				bgmSource.Stop();
			}
		}

		// SEの再生
		public void PlaySE(AudioClip clip, bool loop = false, float volume = 1f)
		{
			if (!StopSE(clip))
			{
				var source = seSourceList[0];
				seSourceList.RemoveAt(0);
				seSourceList.Add(source);
				source.Stop();
				source.clip = null;
			}
			seSourceList[0].clip = clip;
			seSourceList[0].loop = loop;
			seSourceList[0].volume = volume;
			seSourceList[0].Play();
		}

		public bool StopSE(AudioClip clip)
		{
			for (int i = 0; i < seSourceList.Count; i++)
			{
				if (seSourceList[i].clip == clip)
				{
					var source = seSourceList[i];
					seSourceList.RemoveAt(i);
					seSourceList.Add(source);
					source.Stop();
					source.clip = null;
					return true;
				}
			}
			return false;
		}

		public void StopAllSE()
		{
			for (int i = 0; i < seSourceList.Count; i++)
			{
				if (seSourceList[i].clip != null)
				{
					var source = seSourceList[i];
					seSourceList.RemoveAt(i);
					seSourceList.Add(source);
					source.Stop();
					source.clip = null;
				}
			}
		}

		// 一時BGM再生後にメインBGMを再生
		public void PlayBGMWithIntro(AudioClip introBGM, AudioClip mainBGM, float volume = 1f, bool fade = false, float fadeDuration = -1.0f)
		{
			StartCoroutine(PlayTransitionBGM(introBGM, mainBGM, volume));
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
