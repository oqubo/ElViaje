using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyHighscore : MonoBehaviour
{
    public Text myHighScore;

    void Start()
    {
        myHighScore.text = "";
        myHighScore.text =
            PlayerPrefs.GetString("name") +
            " - "+
            PlayerPrefs.GetInt("HighScore").ToString();   
    }

}
