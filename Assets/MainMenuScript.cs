using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void loadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void cowardOption()
    {
        Application.Quit();
    }
}
