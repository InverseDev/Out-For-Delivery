using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseMenu;

void Start()
{
    pauseMenu.SetActive(false); 
}

public void Pause()
{
    Time.timeScale = 0f;
    pauseMenu.SetActive(true);
    AudioListener.pause = true;
}

public void ResumeGame()
{
    Time.timeScale = 1f;
    pauseMenu.SetActive(false);
    AudioListener.pause = false;
}

}
