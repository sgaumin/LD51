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

	private void UpdateLevelIndicator(int value)
	{
		levelIndicator.text = value.ToString();
	}
}