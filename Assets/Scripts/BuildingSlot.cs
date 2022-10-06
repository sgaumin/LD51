using DG.Tweening;
using TMPro;
using UnityEngine;
using static Facade;

[RequireComponent(typeof(SpriteRenderer))]
public class BuildingSlot : MonoBehaviour
{
	private const int MAX_LEVEL = 2;

	[Header("Animations")]
	[SerializeField] private Sprite defaultSprite;
	[SerializeField] private Sprite selectedSprite;
	[SerializeField] private float fadeDuration = 0.2f;

	[Header("Audio")]
	[SerializeField] private AudioExpress selectionSound;

	[Header("References")]
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Canvas canvas;
	[SerializeField] private TextMeshProUGUI levelIndicator;
	[SerializeField] private BuildingController buildingPrefab;
	[SerializeField] private Transform itemSpawn;

	private BuildingController currentBuilding;
	private int currentLevel;

	public int CurrentLevel
	{
		get => currentLevel;
		private set
		{
			currentLevel = Mathf.Min(value, MAX_LEVEL);
			UpdateLevelIndicator(currentLevel);
		}
	}

	private bool ConditionsMet => Level.State == SceneState.BuildingPhase &&
									currentBuilding is null &&
									!GenericDialoguePopup.IsActive;

	private void Awake()
	{
		Level.OnBuildingPhase += FadeIn;
		Level.OnLoopingPhase += FadeOut;
	}

	public void SetItemSpawn(Vector2 position)
	{
		itemSpawn.transform.position = position;
	}

	public void SetCanvas(Vector2 position)
	{
		canvas.transform.position = position;
	}

	private void OnMouseOver()
	{
		if (ConditionsMet)
		{
			spriteRenderer.sprite = selectedSprite;
		}
	}

	private void OnMouseEnter()
	{
		if (ConditionsMet)
		{
			selectionSound.Play();
			spriteRenderer.sprite = selectedSprite;
		}
	}

	private void OnMouseExit()
	{
		if (ConditionsMet)
		{
			spriteRenderer.sprite = defaultSprite;
		}
	}

	private void OnMouseDown()
	{
		if (ConditionsMet)
		{
			spriteRenderer.sprite = selectedSprite;
		}
	}

	private void OnMouseUp()
	{
		if (ConditionsMet)
		{
			selectionSound.Play();
			spriteRenderer.enabled = false;

			currentBuilding = Instantiate(buildingPrefab, transform);
			currentBuilding.transform.position = transform.position;
			currentBuilding.OnKill += OnBuildingKill;
			currentBuilding.Init(itemSpawn.position, CurrentLevel);

			Level.State = SceneState.LoopingPhase;
		}
	}

	private void OnBuildingKill()
	{
		CurrentLevel++;
		spriteRenderer.enabled = true;
		currentBuilding = null;
		spriteRenderer.sprite = defaultSprite;
	}

	private void FadeIn()
	{
		spriteRenderer.DOKill();
		spriteRenderer.DOFade(1f, fadeDuration);
		levelIndicator.DOKill();
		levelIndicator.DOFade(1f, fadeDuration);
	}

	private void FadeOut()
	{
		spriteRenderer.DOKill();
		spriteRenderer.DOFade(0f, fadeDuration);
		levelIndicator.DOKill();
		levelIndicator.DOFade(0f, fadeDuration);
	}

	private void UpdateLevelIndicator(int value)
	{
		levelIndicator.text = value.ToString();
	}
}