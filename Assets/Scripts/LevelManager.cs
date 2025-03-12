using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject levelCompletePanel;
    public TMP_Text levelScoreText;  // Changed to TMP_Text
    public TMP_Text totalScoreText;  // Changed to TMP_Text
    public Button playAgainButton; 

    private float levelStartTime;
    private int totalScore;
    private bool levelCompleted = false;

    void Start()
    {
        if (levelCompletePanel == null || levelScoreText == null || totalScoreText == null)
        {
            Debug.LogError("UI elements are not assigned in the Inspector!");
            return;
        }

        levelCompletePanel.SetActive(false);
        levelStartTime = Time.time;
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
    }

    public void CompleteLevel()
    {
        if (levelCompleted) return;
        levelCompleted = true;

        float timeTaken = Time.time - levelStartTime;
        int levelScore = Mathf.Max(1000 - (int)(timeTaken * 10), 100);

        totalScore += levelScore;
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.Save();

        levelScoreText.text = "Level Score: " + levelScore;
        totalScoreText.text = "Total Score: " + totalScore;

        levelCompletePanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
