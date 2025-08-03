using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Earth earth; // Reference to the Earth object
    [SerializeField] private DayNightManager dayNightManager; // Reference to the DayNightManager
    [SerializeField] private Sun sun;
    
    [SerializeField] private GameObject gameOverPanel; // Reference to the Game Over panel
    [SerializeField] private TextMeshProUGUI gameOverText; // Reference to the Game Over text
    
    private bool isGameStarted = false;
    private bool isGameOver = false;

    public void StartGame()
    {
        isGameStarted = true;
    }
    
    public bool IsGameStarted()
    {
        var result = isGameStarted;
        isGameStarted = false;
        return result;
    }
    
    public bool IsGameOver()
    {
        return isGameOver;
    }
    
    private void EarthDestroyed()
    {
        Debug.Log("Earth has been destroyed!");
        isGameOver = true;
        
        var text = "Game Over!\nYou survived for " + sun.RotationCount + " days.";
        gameOverText.text = text;
        
        gameOverPanel.SetActive(true); // Show the Game Over panel
        
    }
    
    public void RestartGame()
    {
        // load the scene again
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
