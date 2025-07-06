using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using managers;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject deathMenu;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject sleepPanel;
    [SerializeField]
    private GameObject winMenu;

    private const float SlowDownTimeScale = 0.3f;
    private const float SlowDownDuration = 2f;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            PauseGame();
    }

    public void ResetGame()
    {
        LoadCurrentScene();
        Time.timeScale = 1;
    }

    private void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1;
    }

    public void OnDeath()
    {
        deathMenu.SetActive(true);
    }

    public void Win()
    {
        winMenu.SetActive(true);
    }

    public void SlowDownGame()
    {
        StartCoroutine(SleepSlowDown());
    }

    private IEnumerator SleepSlowDown()
    {
        sleepPanel.SetActive(true);
        Time.timeScale = SlowDownTimeScale;
        yield return new WaitForSeconds(SlowDownDuration);
        Time.timeScale = 1;
        sleepPanel.SetActive(false);
        MainGameManager.Instance.Player.Sleep(false);
    }
}
