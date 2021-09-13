using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public TileState MyTileState { get; set; }

	public List<TileController> ListTileController => listTileController;
	[SerializeField] private List<TileController> listTileController;

	public bool IsStartGame { get; set; }

	public Canvas canvasChoose;
	[SerializeField] private GameObject chooseSection;
	[SerializeField] private GameObject gameSection;

	[SerializeField] private Canvas canvas;
	[SerializeField] private TextMeshProUGUI textStart;
	[SerializeField] private TextMeshProUGUI textWinner;
	[SerializeField] private Button buttonRestart;

	[SerializeField] private GameObject endSection;
	[SerializeField] private GameObject draw;
	[SerializeField] private GameObject winX;
	[SerializeField] private GameObject winO;
	[SerializeField] private GameObject winText;
	[SerializeField] private GameObject drawText;
	[SerializeField] private Animator endXOXAnimation;
	[SerializeField] private Animator endTextAnimation;

	[SerializeField] private Animator gridOpening;

	[SerializeField] private LineRenderer _lineRenderer;

	[SerializeField] private AudioClip winClip;
	[SerializeField] private AudioClip drawClip;

	private void Awake()
	{
		Instance = this;
	}

	private void PopulateEndSection(TileState winner)
	{
		gameSection.SetActive(false);
		endSection.SetActive(true);
		
		if (endXOXAnimation.enabled)
			endXOXAnimation.Play(0);
		else
			endXOXAnimation.enabled = true;

		if (endTextAnimation.enabled)
			endTextAnimation.Play(0);
		else
			endTextAnimation.enabled = true;
		
		winX.SetActive(winner == TileState.X);
		winO.SetActive(winner == TileState.O);
		draw.SetActive(winner == TileState.None);
		winText.SetActive(winner != TileState.None);
		drawText.SetActive(winner == TileState.None);

		AudioManager.Instance.PlaySFX(winner == TileState.None ? drawClip : winClip);
	}

	public void OnChoose(TileState state)
	{
		MyTileState = state;
		canvasChoose.enabled = false;
		chooseSection.SetActive(false);
		gameSection.SetActive(true);
		IsStartGame = true;

		if (gridOpening.enabled)
			gridOpening.Play(0);
		else gridOpening.enabled = true;
	}

	public void OnStartGame()
	{
		IsStartGame = true;
		canvas.enabled = false;

		buttonRestart.gameObject.SetActive(false);
		textWinner.enabled = false;

		endSection.SetActive(false);
	}

	public void OnClick_RestartButton()
	{
		SceneManager.LoadScene(0);
	}

	public void OnGameOver(TileState tileState)
	{
		IsStartGame = false;
		canvas.enabled = true;

		PopulateEndSection(tileState);

		canvas.enabled = true;
	}

	public (bool, TileState) HasWinner()
	{
		foreach (var tile in listTileController)
		{
			if (tile.MyState == TileState.None) continue;

			foreach (var direction in Enum.GetValues(typeof(Direction)))
			{
				var next = tile.GetNextTile((Direction) direction);
				if (!next) continue;

				if (next.MyState != tile.MyState) continue;

				var lastTile = next.GetNextTile((Direction) direction);
				if (!lastTile) continue;

				if (lastTile.MyState != tile.MyState) continue;
				
				LineAnimation(new []{tile.transform.position,lastTile.transform.position});
				return (true, tile.MyState);
			}
		}

		return (false, TileState.None);
	}
	
	private void LineAnimation(Vector3[] points)
	{
		IEnumerator Do()
		{
			float passed = 0;
			float time = 1;
			var init = points[0];
			var target = points[1];
			_lineRenderer.SetPosition(0,init);
			while (passed<time)
			{
				passed += Time.deltaTime;
				var current = Vector3.Lerp(init, target, passed / time);
				_lineRenderer.SetPosition(1, current);	
				yield return null;
			}
		}

		StartCoroutine(Do());
	}

	public bool HasNoneTile()
	{
		return listTileController.Exists(x => x.MyState == TileState.None);
	}
}