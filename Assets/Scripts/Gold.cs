using System.Collections;
using Utils;
using UnityEngine;
using static Facade;
using DG.Tweening;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Gold : MonoBehaviour, ISwordTarget
{
	[Header("Animations")]
	[SerializeField] private float flahingDuration = 0.1f;
	[SerializeField] private int flahingLoop = 6;
	[SerializeField, FloatRangeSlider(0f, 5f)] private FloatRange spawningForce = new FloatRange(1f, 3f);
	[SerializeField, FloatRangeSlider(-90f, 90f)] private FloatRange spawningAngleOffset = new FloatRange(-30f, 30f);
	[SerializeField, FloatRangeSlider(0f, 1f)] private FloatRange spawningDuration = new FloatRange(0.5f, 0.8f);
	[SerializeField] private Ease spawningEase;

	[Header("References")]
	[SerializeField] private SpriteRenderer spriteRenderer;

	private bool isInteractible;
	private Coroutine spawning;

	private void Awake()
	{
		Player.OnStartLoop += MakeItInteraticle;
	}

	public void Spawn(Vector2 position)
	{
		this.TryStartCoroutine(SpawnCore(position), ref spawning);
	}

	private IEnumerator SpawnCore(Vector2 position)
	{
		isInteractible = false;
		Tweener flasher = spriteRenderer.DOFade(0f, flahingDuration).SetLoops(flahingLoop, LoopType.Yoyo);

		transform.position = position;
		Vector2 direction = (Player.transform.position - transform.position).normalized;
		Vector2 perpendicular = direction.Rotate(Random.value > 0.5f ? 90f : -90f).normalized;
		perpendicular = perpendicular.Rotate(spawningAngleOffset.RandomValue);
		Vector2 targetPosition = (Vector2)transform.position + perpendicular * spawningForce.RandomValue;

		transform.DOMove(targetPosition, spawningDuration.RandomValue).SetEase(spawningEase);

		yield return flasher.WaitForCompletion();
	}

	private void MakeItInteraticle()
	{
		isInteractible = true;
	}

	public void Interact()
	{
		if (!isInteractible) return;

		spriteRenderer.gameObject.layer = LayerMask.NameToLayer("CardHUD");
		spriteRenderer.sortingLayerName = "CardHUD";
		spriteRenderer.sortingOrder = 10;

		transform.DOMove(CardHUD.Instance.GoldIconPosition, 0.6f).SetEase(Ease.OutSine).OnComplete(() =>
		{
			Player.Gold++;
			Destroy(gameObject);
		});
	}

	private void OnDestroy()
	{
		Player.OnStartLoop -= MakeItInteraticle;
	}
}