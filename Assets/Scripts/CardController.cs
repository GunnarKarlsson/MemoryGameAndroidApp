using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour {

	public Sprite[] frontSprites;
	public Sprite[] backSprites;
	List<GameObject> cards;
	public float uncoverTime = 12.0f;
	public int gridPadding = 10;
	private bool isTurning = false;
	public int gridWidth;
	public int gridHeight;
	private int cardWidth;
	private int cardHeight;
	public GameObject board;

	// Use this for initialization
	void Start () {
		cards = new List<GameObject> ();
		initCards ();
		Debug.Log ("Start done");
	}

	public void initCards() {
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridHeight; j++) {
					GameObject card = new GameObject ("Card"); // parent object
					GameObject cardFront = new GameObject ("CardFront");
					GameObject cardBack = new GameObject ("CardBack");

					cardFront.transform.parent = card.transform; // make front child of card
					cardBack.transform.parent = card.transform; // make back child of card

					// front (motive)
					cardFront.AddComponent<SpriteRenderer> ();
					cardFront.GetComponent<SpriteRenderer> ().sprite = frontSprites[i];
					cardFront.GetComponent<SpriteRenderer> ().sortingOrder = -1;

					// back
					cardBack.AddComponent<SpriteRenderer> ();
					cardBack.GetComponent<SpriteRenderer> ().sprite = backSprites[i];
					cardBack.GetComponent<SpriteRenderer> ().sortingOrder = 1;

					cardWidth = (int)frontSprites[i].rect.width;
					cardHeight = (int)frontSprites[i].rect.height;

					Debug.Log (cardWidth);
					Debug.Log (cardHeight);

					card.tag = "Card";
					card.transform.parent = transform;

					card.AddComponent<BoxCollider2D> ();
					card.GetComponent<BoxCollider2D> ().size = new Vector2 (cardWidth, cardHeight);
					float x = i * (cardWidth + gridPadding);
					float y = j * (cardHeight + gridPadding);
					card.transform.position = new Vector3 (x, y, 0f);
					cards.Add (card);
			}
		}

		float offsetX = -(gridWidth/2.0f)*(cardWidth - gridPadding) + cardWidth/2.0f;
		float offsetY = -(gridHeight/2.0f)*(cardHeight - gridPadding) + cardHeight/2.0f;

		board.transform.position = new Vector3 (offsetX, offsetY, 0f);
	}

	// Update is called once per frame
	void Update () {
		if((Input.GetMouseButtonDown(0) || Input.touchCount > 0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
			// we hit a card
			if (hit.collider != null) {
				Debug.Log(hit.collider.gameObject.name);
				if (!isTurning) {
					isTurning = true;
					StartCoroutine (uncoverCard (hit.collider.gameObject.transform, true));
				}
			}
		}
	}

	IEnumerator uncoverCard(Transform card, bool uncover){

		float minAngle = uncover ? 0 : 180;
		float maxAngle = uncover ? 180 : 0; 

		float t = 0;
		bool uncovered = false;

		while(t < 1f) {
			t += Time.deltaTime * uncoverTime;;

			float angle = Mathf.LerpAngle(minAngle, maxAngle, t);
			card.eulerAngles = new Vector3(0, angle, 0);

			if( ( (angle >= 90 && angle < 180) || (angle >= 270 && angle < 360) ) && !uncovered) {
				uncovered = true;
				for(int i = 0; i < card.childCount; i++) {
					// reverse sorting order to show the otherside of the card
					// otherwise you would still see the same sprite because they are sorted 
					// by order not distance (by default)
					Transform c = card.GetChild(i);
					c.GetComponent<SpriteRenderer>().sortingOrder *= -1;

					yield return null;
				}
			}

			yield return null;
		}
		isTurning = false;
		yield return 0;
	}
}
