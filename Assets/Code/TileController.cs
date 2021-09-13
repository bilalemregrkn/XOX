using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileController : MonoBehaviour, IPointerDownHandler
{
	public TileState MyState { get; set; }

	public Vector2 coordinate;

	[SerializeField] private SpriteRenderer mySpriteRenderer;

	[SerializeField] private Sprite xSprite;
	[SerializeField] private Sprite oSprite;

	[SerializeField] private Color xColor;
	[SerializeField] private Color oColor;
	[SerializeField] private Animator setStateAnimation;
	[SerializeField] private AudioClip clip;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (MyState != TileState.None)
			return;

		var manager = GameManager.Instance;
		if (!manager.IsStartGame)
			return;
		
		AudioManager.Instance.PlaySFX(clip);
		var state = manager.MyTileState;
		SetState(state);
		manager.MyTileState = manager.MyTileState == TileState.O ? TileState.X : TileState.O;

		var (hasWinner, tileState) = GameManager.Instance.HasWinner();
		if (hasWinner)
		{
			manager.IsStartGame = false;
			IEnumerator Do()
			{
				yield return new WaitForSeconds(2);
				GameManager.Instance.OnGameOver(tileState);
			}

			StartCoroutine(Do());
		}
		else
		{
			if (!GameManager.Instance.HasNoneTile())
				GameManager.Instance.OnGameOver(TileState.None);
				
		}
	}

	public void SetState(TileState state)
	{
		MyState = state;
		mySpriteRenderer.color = state == TileState.X ? xColor : oColor;
		mySpriteRenderer.sprite = state == TileState.X ? xSprite : oSprite;
		if (state == TileState.None) mySpriteRenderer.sprite = null;

		if (setStateAnimation.enabled)
			setStateAnimation.Play(0);
		else
			setStateAnimation.enabled = true;
	}

	public TileController GetNextTile(Direction direction)
	{
		var nextTileCoordinate = coordinate;
		switch (direction)
		{
			case Direction.Up:
				nextTileCoordinate.y++;
				break;
			case Direction.UpRight:
				nextTileCoordinate.y++;
				nextTileCoordinate.x++;
				break;
			case Direction.Right:
				nextTileCoordinate.x++;
				break;
			case Direction.DownRight:
				nextTileCoordinate.x++;
				nextTileCoordinate.y--;
				break;
			case Direction.Down:
				nextTileCoordinate.y--;
				break;
			case Direction.LeftDown:
				nextTileCoordinate.y--;
				nextTileCoordinate.x--;
				break;
			case Direction.Left:
				nextTileCoordinate.x--;
				break;
			case Direction.UpLeft:
				nextTileCoordinate.x--;
				nextTileCoordinate.y++;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
		}

		return GameManager.Instance.ListTileController.Find(tile => tile.coordinate == nextTileCoordinate);
	}
}

public enum TileState
{
	None,
	X,
	O
}

public enum Direction
{
	Up,
	UpRight,
	Right,
	DownRight,
	Down,
	LeftDown,
	Left,
	UpLeft
}