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
using static Facade;
using System.Drawing.Text;

[ExecuteAlways]
public class UICamera : MonoBehaviour
{
	[SerializeField] private Camera mainCamera;

	private Camera uiCamera;

	private void Start()
	{
		uiCamera = GetComponent<Camera>();
	}

	private void Update()
	{
		if (mainCamera is not null)
		{
			uiCamera.transform.position = mainCamera.transform.position;
			uiCamera.transform.rotation = mainCamera.transform.rotation;
			uiCamera.orthographicSize = mainCamera.orthographicSize;
		}
	}
}