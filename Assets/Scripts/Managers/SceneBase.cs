using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using Utils;
using Utils.Dependency;
using static Facade;

public class SceneBase : Singleton<SceneBase>
{
	public event Action OnBuildingPhase;

	public event Action OnLoopingPhase;

	public event Action OnEnd;

	public event Action OnPause;

	[SerializeField] private LevelLoader levelLoader;
	[SerializeField] private MusicPlayer music;
	[SerializeField] private VisualEffectsHandler effectHandler;

	private SceneState state;

	public SceneState State
	{
		get => state;
		set
		{
			state = value;

			switch (value)
			{
				case SceneState.BuildingPhase:
					OnBuildingPhase?.Invoke();
					break;

				case SceneState.LoopingPhase:
					OnLoopingPhase?.Invoke();
					break;

				case SceneState.GameOver:
					OnEnd?.Invoke();
					break;

				case SceneState.Pause:
					OnPause?.Invoke();
					break;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();

		Application.targetFrameRate = 60;

		DOTween.Init();
		DOTween.defaultAutoPlay = AutoPlay.All;

		// Disable screen dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	private void Update()
	{
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR
		if (Input.GetButtonDown("Quit"))
		{
			Level.Quit();
		}
		if (Input.GetButtonDown("Mute"))
		{
			Level.Mute();
		}
		if (Input.GetButtonDown("Fullscreen"))
		{
			Screen.fullScreen = !Screen.fullScreen;
		}
#endif
	}

	#region Level Loading

	public void ReloadScene() => levelLoader.Reload();

	public void LoadNextScene() => levelLoader.LoadNextScene();

	public void LoadPrevious() => levelLoader.LoadPrevious();

	public void LoadMenu() => levelLoader.LoadMenu();

	public void LoadSceneTransition(SceneLoading loading) => levelLoader.LoadSceneTransition(loading);

	public void Quit() => levelLoader.Quit();

	#endregion Level Loading

	#region Post-Processing and Effects

	public void GenerateImpulse()
	{
		effectHandler.GenerateImpulse();
	}

	public void BoostTime(float startValue, float duration)
	{
		effectHandler.BoostTime(startValue, duration);
	}

	public void FreezeTime(float duration = 0.5f)
	{
		effectHandler.FreezeTime(duration);
	}

	public void InverseColor(float duration = 0.05f)
	{
		effectHandler.InverseColor(duration);
	}

	public void ResetShaders()
	{
		effectHandler.ResetShaders();
	}

	#endregion Post-Processing and Effects

	#region Audio

	public bool IsMuted => music.IsMuted;

	public void Mute()
	{
		music.Mute();
	}

	#endregion Audio
}