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
		if (Level.State == SceneState.BuildingPhase && currentBuilding is null)
		{
			spriteRenderer.sprite = selectedSprite;
		}
	}

	private void OnMouseEnter()
	{
		if (Level.State == SceneState.BuildingPhase && currentBuilding is null)
		{
			spriteRenderer.sprite = selectedSprite;
		}
	}

	private void OnMouseExit()
	{
		if (Level.State == SceneState.BuildingPhase && currentBuilding is null)
		{
			spriteRenderer.sprite = defaultSprite;
		}
	}

	private void OnMouseDown()
	{
		if (Level.State == SceneState.BuildingPhase && currentBuilding is null)
		{
			spriteRenderer.sprite = selectedSprite;
		}
	}

	private void OnMouseUp()
	{
		if (Level.State == SceneState.BuildingPhase && currentBuilding is null)
		{
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