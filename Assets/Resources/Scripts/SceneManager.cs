using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{

    string levelName = "Island";
    Text levelBtnText;

    void Awake()
    {
        DontDestroyOnLoad(this);
        foreach (Button b in FindObjectOfType<Canvas>().GetComponentsInChildren<Button>())
        {
            if (b.name == "ChangeLevelBtn")
                levelBtnText = b.GetComponentInChildren<Text>();
        }
    }

    void Update()
    {
        if (levelName == "Island")
            levelBtnText.text = "Level: Island";
        else if (levelName == "PlayableScenarioScene")
            levelBtnText.text = "Level: Canyon";

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu" && Input.GetKey(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }

    public void ChangeLevel()
    {
        if (levelName == "Island") levelName = "PlayableScenarioScene";
        else levelName = "Island";
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {

    }


}