using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Utils;
using Utils.Dependency;
using UnityEngine;
using UnityEngine.UI;

public class GenericDialoguePopup : MonoBehaviour
{
	public static bool IsActive { get; private set; }

	[SerializeField] private float fadDuration = 0.2f;
	[SerializeField] private float characterDisplaySpeed = 0.02f;
	[SerializeField] private TextMeshProUGUI content;
	[SerializeField] private CanvasGroup group;

	[Header("Audio")]
	[SerializeField] private AudioExpress characterDisplaySound;
	[SerializeField] private AudioExpress clickSound;

	private Coroutine displaying;
	private bool next;

	private void Start()
	{
		group.blocksRaycasts = false;
		group.interactable = false;
	}

	public void Display(List<string> lines)
	{
		IsActive = true;

		group?.DOKill();
		group.blocksRaycasts = true;
		group.interactable = true;
		group.DOFade(1f, fadDuration);

		this.TryStartCoroutine(DisplayCore(lines), ref displaying);
	}

	public void Next()
	{
		next = true;
		clickSound.Play();
	}

	private IEnumerator DisplayCore(List<string> lines)
	{
		content.text = "";
		foreach (string line in lines)
		{
			string formatedLine = line.ToUpper();
			next = false;
			content.text = "";
			foreach (char c in formatedLine)
			{
				if (next) break;

				content.text += c;
				characterDisplaySound.Play();
				yield return new WaitForSeconds(characterDisplaySpeed);
			}

			next = false;
			content.text = formatedLine;

			while (!next) yield return null;
		}

		Close();
	}

	public void Close()
	{
		group?.DOKill();
		group.blocksRaycasts = false;
		group.interactable = false;
		group.DOFade(0f, fadDuration);

		IsActive = false;
	}
}