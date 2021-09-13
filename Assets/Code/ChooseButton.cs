using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseButton : MonoBehaviour, IPointerDownHandler
{
	public TileState myState;
	[SerializeField] private Animator chooseAnimation;
	[SerializeField] private float time;

	[SerializeField] private Animator chooseSectionAnimation;
	[SerializeField] private float timeClose;
	[SerializeField] private GameObject textChoose;

	[SerializeField] private AudioClip clip;
	
	public void OnPointerDown(PointerEventData eventData)
	{
		chooseAnimation.enabled = true;
		textChoose.SetActive(false);
		AudioManager.Instance.PlaySFX(clip);
		
		IEnumerator Do()
		{
			yield return new WaitForSeconds(time);
			chooseSectionAnimation.enabled = true;
			yield return new WaitForSeconds(timeClose);
			GameManager.Instance.OnChoose(myState);
		}

		StartCoroutine(Do());
	}
}