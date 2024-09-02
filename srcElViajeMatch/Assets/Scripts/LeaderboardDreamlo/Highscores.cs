using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Highscores : MonoBehaviour
{

	const string privateCode = "xpmVYIv8xUaPpGmQmQ8H2QNpBzRCAXv0a5wsaeA2x0vw";
	const string publicCode = "606f74e48f421366b068e8b8";
	const string webURL = "http://dreamlo.com/lb/";

	DisplayHighscores highscoreDisplay;
	public Highscore[] highscoresList;
	static Highscores instance;

	void Awake()
	{
		highscoreDisplay = GetComponent<DisplayHighscores>();
		instance = this;
	}

	public static void AddNewHighscore(string username, int score)
	{
		instance.StartCoroutine(instance.UploadNewHighscore(username, score));
	}

IEnumerator UploadNewHighscore(string username, int score)
{
    string url = $"{webURL}{privateCode}/add/{UnityWebRequest.EscapeURL(username)}/{score}";
    UnityWebRequest www = UnityWebRequest.Get(url);
    
    // Permitir conexiones HTTP inseguras (solo durante el desarrollo)
    www.certificateHandler = new AcceptAllCertificatesSignedWithASelfSignedCert();
    
    yield return www.SendWebRequest();

    if (www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError)
    {
        Debug.Log("Upload Successful");
        DownloadHighscores();
    }
    else
    {
        Debug.Log("Error uploading: " + www.error);
    }
}

// Clase para aceptar certificados inseguros
class AcceptAllCertificatesSignedWithASelfSignedCert : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Aquí puedes agregar lógica para validar certificados de desarrollo,
        // o simplemente devolver true para aceptar todos los certificados.
        return true;
    }
}

	public void DownloadHighscores()
	{
		StartCoroutine("DownloadHighscoresFromDatabase");
	}
	
IEnumerator DownloadHighscoresFromDatabase()
{
    UnityWebRequest www = UnityWebRequest.Get(webURL + publicCode + "/pipe/");
    
    // Permitir conexiones HTTP inseguras (solo durante el desarrollo)
    www.certificateHandler = new AcceptAllCertificatesSignedWithASelfSignedCert();
    
    yield return www.SendWebRequest();

    if (www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError)
    {
        FormatHighscores(www.downloadHandler.text);
        highscoreDisplay.OnHighscoresDownloaded(highscoresList);
    }
    else
    {
        Debug.Log("Error Downloading: " + www.error);
    }
}



	void FormatHighscores(string textStream)
	{
		string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		highscoresList = new Highscore[entries.Length];

		for (int i = 0; i < entries.Length; i++)
		{
			string[] entryInfo = entries[i].Split(new char[] { '|' });
			string username = entryInfo[0];
			int score = int.Parse(entryInfo[1]);
			highscoresList[i] = new Highscore(username, score);
			print(highscoresList[i].username + ": " + highscoresList[i].score);
		}
	}

}

public struct Highscore
{
	public string username;
	public int score;

	public Highscore(string _username, int _score)
	{
		username = _username;
		score = _score;
	}

}