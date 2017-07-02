using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour {

	string cardPairName = "";
	public string CardPairName
	{
		get
		{
			return cardPairName;
		}
		set
		{
			cardPairName = value;
		}
	}

	bool solved = false;
	public bool Solved
	{
		get
		{
			return solved;
		}
		set
		{
			solved = value;
		}
	}

	bool selected = false;
	public bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
		}
	}
}
