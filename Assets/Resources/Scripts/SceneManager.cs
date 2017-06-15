using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

    void Awake ()
    {
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start () {
		
	}

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("PlayableScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {

    }


}
