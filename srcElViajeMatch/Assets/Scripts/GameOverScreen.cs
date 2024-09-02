using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{

	public GameObject getNamePanel;

	public Text myScore;
	public Text myHighScore;
	public Text nameTxt;

	private int score;

	void Start()
	{

		//PlayerPrefs.DeleteAll();
		// obtener el nombre del jugador
		string name = PlayerPrefs.GetString("name");

		// pedimos el nombre si no lo tenemos
		if (string.IsNullOrEmpty(name)) {
			getNamePanel.SetActive(true);
		}

		else{
			pintarPuntuaciones();
		}

	}


	public void pintarPuntuaciones() {
		// puntuacion actual
		score = PlayerPrefs.GetInt("Score");
		myScore.text = score.ToString();

		// puntuacion maxima
		if (score > PlayerPrefs.GetInt("HighScore"))
		{
			PlayerPrefs.SetInt("HighScore", score);
			anadirDreamlo();
		}

		myHighScore.text =
			PlayerPrefs.GetString("name") +
			" - " +
			PlayerPrefs.GetInt("HighScore").ToString();

	}

	public void anadirDreamlo() {
		Highscores.AddNewHighscore(PlayerPrefs.GetString("name"), score);
	}

	public void saveName()
	{
		PlayerPrefs.SetString("name", nameTxt.text);
		getNamePanel.SetActive(false);
		pintarPuntuaciones();
	}


}
