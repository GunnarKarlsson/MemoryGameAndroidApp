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
	public int gridWidth = 4;
	public int numPairs = 8;
	private int cardWidth;
	private int cardHeight;
	public GameObject board;

	//UI
	public Button playButton;
	public Text textMatchedPairs;

	bool isTouching = false;
	bool isUncovering = false;
	bool isGameOver = false;

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
		Debug.Log("deck.Count: " + deck.Count);
		for (int j = deck.Count-1; j == 0; j--) {
			Debug.Log("j: " + j);
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

		if (isGameOver) {
			return;
		}

		//if ((Input.GetMouseButtonDown (0) || Input.touchCount > 0)) {
		if ((Input.GetMouseButtonDown (0) || Input.touchCount > 0) && !isTouching && !isUncovering && selectedCards.Count < 2) {

			isTouching = true;

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction);
			// we hit a card
			if (hit.collider != null) {
				
				Debug.Log (hit.collider.gameObject.name);

				if (!hit.collider.GetComponent<CardScript> ().Selected) {

					if (!hit.collider.GetComponent<CardScript> ().Solved) {
						StartUncoverCard (hit.collider.gameObject);
						selectedCards.Add (hit.collider.gameObject);
						hit.collider.gameObject.GetComponent<CardScript> ().Selected = true;

					}
				}
			}
		} else {
			isTouching = false;
		}
	}



	void StartUncoverCard(GameObject card) {
		StartCoroutine (UncoverCard(card, true));
	}

	IEnumerator UncoverCard(GameObject cardGameObject, bool uncover){

		isUncovering = true;

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
					Transform c = card.GetChild(i);
					c.GetComponent<SpriteRenderer>().sortingOrder *= -1;
					yield return null;
				}
			}
			yield return null;
		}

		if (selectedCards.Count == 2) {
			
			GameObject cardOne = selectedCards [0];
			GameObject cardTwo = selectedCards [1];
			CardScript scriptOne = cardOne.GetComponent<CardScript> ();
			CardScript scriptTwo = cardTwo.GetComponent<CardScript> ();
			if (!scriptOne.CardPairName.Equals (scriptTwo.CardPairName)) {
				yield return new WaitForSeconds (0.5f);
				StartUncoverCard (cardOne);
				StartUncoverCard(cardTwo);
				//StartCoroutine (UncoverCard (selectedCards [0], true));
				//StartCoroutine (UncoverCard (selectedCards [1], true));
			} else {
				matchedCards.Add (cardOne);
				matchedCards.Add (cardTwo);
				textMatchedPairs.text = "Matched Pairs: " + (matchedCards.Count / 2).ToString ();
				selectedCards[0].GetComponent<CardScript>().Solved = true;
				selectedCards[1].GetComponent<CardScript>().Solved = true;

				Debug.Log ("matchedCards.Count: " + matchedCards.Count);
				Debug.Log ("numPairds * 2: " + (numPairs * 2));
				if (matchedCards.Count == numPairs * 2) {
						yield return new WaitForSeconds (1.0f);
						textMatchedPairs.text = "Matched Pairs: " + (matchedCards.Count / 2).ToString() + ". All cards matched :-)";
						isGameOver = true;
						ClearAll ();
					}
				}
			if (selectedCards.Count > 0) {
				selectedCards [0].GetComponent<CardScript> ().Selected = false;
			}
			if (selectedCards.Count > 1) {
				selectedCards [1].GetComponent<CardScript> ().Selected = false;
			}
			selectedCards.Clear ();
			}
			yield return new WaitForSeconds(0.1f);

			isUncovering = false;
		}
}
