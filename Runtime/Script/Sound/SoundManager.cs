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
		public float defaultFadeDuration = 0.5f; // フェードイン/アウトの時間

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
			//カスタムグループ
			sePlayerSet = new(SEGroupName, maxPlaySECount);
			sePlayerSet.Initialize(audioMixer);

			foreach (var ap in customPlayerList)
			{
				ap.Initialize(audioMixer);
			}
		}

		/// <summary>
		/// マスターボリューム設定
		/// </summary>
		/// <param name="volume">ボリューム　0～1</param>
		public void SetMasterVolume(float volume) => SetVolume(MasterGroupName, volume);
		/// <summary>
		/// SEボリューム設定
		/// </summary>
		/// <param name="volume">ボリューム　0～1</param>
		public void SetSEVolume(float volume) => SetVolume(SEGroupName, volume);
		/// <summary>
		/// BGMボリューム設定
		/// </summary>
		/// <param name="volume">ボリューム　0～1</param>
		public void SetBGMVolume(float volume) => SetVolume(BGMGroupName, volume);
		/// <summary>
		/// ボリューム設定
		/// </summary>
		/// <param name="groupName">ミキサーグループ名</param>
		/// <param name="volume">ボリューム　0～1</param>
		public void SetVolume(string groupName, float volume)
		{
			float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1)) * 20;
			audioMixer.SetFloat(groupName, dB);
		}

		/// <summary>
		/// マスターボリュームの取得
		/// </summary>
		/// <returns>ボリューム　0～1</returns>
		public float GetMasterVolume() => GetVolume(MasterGroupName);
		/// <summary>
		/// SEボリュームの取得
		/// </summary>
		/// <returns>ボリューム　0～1</returns>
		public float GetSEVolume() => GetVolume(SEGroupName);
		/// <summary>
		/// BGMボリュームの取得
		/// </summary>
		/// <returns>ボリューム　0～1</returns>
		public float GetBGMVolume() => GetVolume(BGMGroupName);
		/// <summary>
		/// ボリュームの取得<br>
		/// 取得できなかった場合は0
		/// </summary>
		/// <param name="groupName">ミキサーグループ名</param>
		/// <returns>ボリューム　0～1</returns>
		public float GetVolume(string groupName)
		{
			if (audioMixer.GetFloat(groupName, out float dB))
			{
				float linear = Mathf.Pow(10f, dB / 20f);
				return Mathf.Clamp01(linear);
			}
			else
			{
				// 取得できなかった場合は0（無音）とする
				return 0f;
			}
		}

		/// <summary>
		/// BGMの再生
		/// </summary>
		/// <param name="clip">再生BGM</param>
		/// <param name="loop">ループ</param>
		/// <param name="volume">ボリューム</param>
		/// <param name="fade">フェード</param>
		/// <param name="fadeDuration">フェード時間</param>
		public void PlayBGM(AudioClip clip, bool loop, float volume, bool fade, float fadeDuration)
		{
			if (bgmFadeInCoroutine != null) StopCoroutine(bgmFadeInCoroutine);

			//既存BGMを止める
			StopBGM(fade, fadeDuration);

			//メインで鳴らすものは0番目
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
		/// BGMの再生
		/// </summary>
		/// <param name="clip">再生BGM</param>
		/// <param name="loop">ループ</param>
		/// <param name="volume">ボリューム</param>
		/// <param name="fade">フェード</param>
		public void PlayBGM(AudioClip clip, bool loop, float volume, bool fade) => PlayBGM(clip, loop, volume, fade, -1.0f);
		/// <summary>
		/// BGMの再生
		/// </summary>
		/// <param name="clip">再生BGM</param>
		/// <param name="loop">ループ</param>
		/// <param name="volume">ボリューム</param>
		public void PlayBGM(AudioClip clip, bool loop, float volume) => PlayBGM(clip, loop, volume, false, -1.0f);
		/// <summary>
		/// BGMの再生
		/// </summary>
		/// <param name="clip">再生BGM</param>
		/// <param name="loop">ループ</param>
		public void PlayBGM(AudioClip clip, bool loop) => PlayBGM(clip, loop, 1.0f, false, -1.0f);
		/// <summary>
		/// BGMの再生
		/// </summary>
		/// <param name="clip">再生BGM</param>
		public void PlayBGM(AudioClip clip) => PlayBGM(clip, true, 1.0f, false, -1.0f);


		/// <summary>
		/// BGMの停止
		/// </summary>
		/// <param name="fade">停止のフェード</param>
		/// <param name="fadeDuration">フェード時間</param>
		public void StopBGM(bool fade = false, float fadeDuration = -1)
		{
			if (bgmFadeOutCoroutine != null) StopCoroutine(bgmFadeOutCoroutine);

			//止めるのは0番目
			if (fade)
			{
				bgmFadeOutCoroutine = StartCoroutine(FadeOut(bgmSource.SourceList[0]));
			}
			else
			{
				bgmSource.SourceList[0].Stop();
			}

			//止める処理を呼んだ後は1番目に移動させる
			(bgmSource.SourceList[0], bgmSource.SourceList[1]) = (bgmSource.SourceList[1], bgmSource.SourceList[0]);
		}

		/// <summary>
		/// SEを再生
		/// </summary>
		/// <param name="clip">再生ファイル</param>
		/// <param name="loop">ループ</param>
		/// <param name="volume">ボリューム</param>
		public void PlaySE(AudioClip clip, bool loop = false, float volume = 1f) => PlayWithSet(sePlayerSet, clip, loop, volume, false, -1);
		/// <summary>
		/// SEを停止
		/// </summary>
		/// <param name="clip">停止ファイル</param>
		/// <returns>停止の結果</returns>
		public bool StopSE(AudioClip clip) => StopWithSet(sePlayerSet, clip);
		/// <summary>
		/// 全SEを停止
		/// </summary>
		public void StopAllSE() => StopAllWithSet(sePlayerSet);

		/// <summary>
		/// BGM イントロありvar
		/// </summary>
		/// <param name="introBGM">イントロBGM</param>
		/// <param name="mainBGM">メインBGM</param>
		/// <param name="volume">ボリューム</param>
		/// <param name="fade">イントロのフェードイン</param>
		/// <param name="fadeDuration">フェード時間</param>
		public void PlayBGMWithIntro(AudioClip introBGM, AudioClip mainBGM, float volume = 1f, bool fade = false, float fadeDuration = -1.0f)
		{
			StartCoroutine(PlayTransitionBGM(introBGM, mainBGM, volume, fade, fadeDuration));
		}

		/// <summary>
		/// オーディオミキサーグループを指定して再生
		/// </summary>
		/// <param name="audioMixerGroup">オーディオミキサー名</param>
		/// <param name="clip">再生ファイル</param>
		/// <param name="loop">ループ</param>
		/// <param name="volume">ボリューム</param>
		/// <param name="fade">フェード有無</param>
		/// <param name="fadeDuration">フェード時間</param>
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
			//再生したら後ろに回す
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
					//再生済みがあれば止めて先頭に持ってくる
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
		private IEnumerator PlayTransitionBGM(AudioClip introBGM, AudioClip mainBGM, float volume, bool fade = false, float fadeDuration = -1.0f)
		{
			PlayBGM(introBGM, false, volume, fade, fadeDuration);

			// 一時BGMが終了するまで待機
			yield return new WaitUntil(() => !bgmSource.SourceList[0].isPlaying);

			// メインBGMにスムーズに切り替え
			PlayBGM(mainBGM, true, volume, false, 0);
		}


		public void Test(AudioClip clip)
		{
			PlaySE(clip);
		}
	}
}
