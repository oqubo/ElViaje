
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class GUIManager : MonoBehaviour {
	public static GUIManager instance;

	public Text scoreTxt;
	public Text moveCounterTxt;

	public int moveCounter = 5;
	public int moveCounterBonus = 1;
	public int scorePerMatch = 5;

	private int _score;

	public int Score
	{
		get
		{
			return _score;
		}

		set
		{
			_score = value;
			scoreTxt.text = _score.ToString();
		}
	}

	public int MoveCounter
	{
		get
		{
			return moveCounter;
		}

		set
		{
			moveCounter = value;
			moveCounterTxt.text = moveCounter.ToString();

			if (moveCounter <= 0)
			{
				moveCounter = 0;
				GameOver();
			}

		}
	}


	void Awake() {
		moveCounterTxt.text = moveCounter.ToString();
		instance = GetComponent<GUIManager>();
	}

	// Show the game over panel
	public void GameOver() {

		GameManager.instance.gameOver = true;

		PlayerPrefs.SetInt("Score", Score);

		GameManager.instance.LoadScene("GameOver");

	}


}
