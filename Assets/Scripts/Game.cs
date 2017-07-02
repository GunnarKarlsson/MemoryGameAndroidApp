using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	public Sprite[] frontSprites;
	public Sprite backSprite;
	List <GameObject> deck;
	List<GameObject> selectedCards;
	List<GameObject> matchedCards;
	public float uncoverTime = 1.0f;
	public int gridPadding = 10;
	private bool isTurning = false;
	public int gridWidth = 4;
	public int numPairs = 8;
	private int cardWidth;
	private int cardHeight;
	public GameObject board;

	//UI
	public Button playButton;
	public Text textMatchedPairs;


	void Start () {
		deck = new List<GameObject> ();
		selectedCards = new List<GameObject> ();
		matchedCards = new List<GameObject> ();
	}

	public void OnClickPlay() {
		playButton.gameObject.SetActive (false);
		board.gameObject.SetActive (true);
		CreateDeck ();
		ShuffleDeck ();
		DealCards ();
		PositionBoard ();
	}

	void ClearAll() {
		Debug.Log ("ClearAll()");


		for (int j = deck.Count; j > 0; j--) {
			DestroyImmediate (deck [j]);
		}

		deck.Clear ();
		selectedCards.Clear ();
		matchedCards.Clear ();
		playButton.gameObject.SetActive (true);

		board.transform.position = new Vector3 (0f, 0f, 0f);
		board.gameObject.SetActive (false);
	}

	void PositionBoard() {
		float offsetX = -(gridWidth * (cardWidth + gridPadding) - (cardWidth + gridPadding))/2.0f;
		//assume square board
		float offsetY = offsetX;

		board.transform.position = new Vector3 (offsetX, offsetY, 0f);
	}

	void CreateDeck() {
		int pair = 2;
		for (int i = 0; i < (numPairs); i++) {
			for (int j = 0; j < pair; j++) {
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
				cardBack.GetComponent<SpriteRenderer> ().sprite = backSprite;
				cardBack.GetComponent<SpriteRenderer> ().sortingOrder = 1;

				card.AddComponent<CardScript> ();
				CardScript cardScript = card.GetComponent<CardScript> ();
				cardScript.Solved = false;
				cardScript.Selected = false;
				cardScript.CardPairName = "CardName: " + i;//two cards will share name

				cardWidth = (int)frontSprites[i].rect.width;
				cardHeight = (int)frontSprites[i].rect.height;

				//Debug.Log (cardWidth);
				//Debug.Log (cardHeight);

				card.tag = "Card";
				card.transform.parent = transform;

				card.AddComponent<BoxCollider2D> ();
				card.GetComponent<BoxCollider2D> ().size = new Vector2 (cardWidth, cardHeight);

				deck.Add (card);
			}
		}
		Debug.Log ("CreateDeck(): " + deck.Count + " cards added to deck");
	}

	void ShuffleDeck() {
		deck.Shuffle ();
	}

	void DealCards() {
		int yCounter = 0;
		for (int i = 0; i < deck.Count; i++) {
			GameObject card = deck [i];

			cardWidth = (int)backSprite.rect.width;
			cardHeight = (int)backSprite.rect.height;
			float x = ((i % gridWidth) * (cardWidth + gridPadding));
			float y = yCounter * (cardHeight + gridPadding);
			//Debug.Log ("x: " + x + " y" + y);
			//Debug.Log("i % gridWidth: " + (i % gridWidth));
			card.transform.position = new Vector3 (x, y, 0f);
			if ((i % gridWidth) == (gridWidth-1)) {
				yCounter++;
			}
		}
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
					StartCoroutine (UncoverCard (hit.collider.gameObject, true));
					StartCoroutine (MatchCard (hit.collider.gameObject));
				}
			}
		}
	}

	IEnumerator UncoverCard(GameObject cardGameObject, bool uncover){

		Debug.Log ("UncoverCard");

		Transform card = cardGameObject.transform;

		float minAngle = uncover ? 0 : 180;
		float maxAngle = uncover ? 180 : 0; 

		float t = 0;
		bool uncovered = false;

		while(t < 1f) {
			t += Time.deltaTime * 10 / uncoverTime;;

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

	IEnumerator MatchCard(GameObject cardGameObject) {

		bool sameCardClicked = CheckIfSameCardClicked (cardGameObject);
		if (sameCardClicked) {
			selectedCards.Clear ();
			Debug.Log ("Exiting MatchCard coroutine");
			yield break;
		}
		Debug.Log ("Not excitng Matchcard coroutine");
		selectedCards.Add (cardGameObject);	

		yield return new WaitForSeconds(1);

		Debug.Log ("selectedCards.Count " + selectedCards.Count);

		if (selectedCards.Count == 2) {
		
			GameObject cardOne = selectedCards [0];
			GameObject cardTwo = selectedCards [1];
			CardScript scriptOne = cardOne.GetComponent<CardScript> ();
			CardScript scriptTwo = cardTwo.GetComponent<CardScript> ();
			if (scriptOne.CardPairName.Equals (scriptTwo.CardPairName)) {
				Debug.Log ("We have a match!");
				matchedCards.Add (cardOne);
				matchedCards.Add (cardTwo);
				textMatchedPairs.text = "Matched Pairs: " + (matchedCards.Count / 2).ToString ();
			} else {
				Debug.Log ("Not a match");
				StartCoroutine (UncoverCard (cardOne, false));
				StartCoroutine (UncoverCard (cardTwo, false));
			}
			selectedCards.Clear ();
		}

		if (matchedCards.Count == numPairs * 2) {
			textMatchedPairs.text = "Matched Pairs: " + (matchedCards.Count / 2).ToString() + ". All cards matched :-)";
			ClearAll ();
		}
		yield return null;
	}

	private bool CheckIfSameCardClicked(GameObject card) {

		if (selectedCards.Count < 1) {
			return false;
		}

		if (selectedCards [0] == card) {
			return true;
		} else {
			return false;
		}
	}
}
