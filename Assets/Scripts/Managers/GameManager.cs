using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Earth earth; // Reference to the Earth object
    [SerializeField] private DayNightManager dayNightManager; // Reference to the DayNightManager
    
    private bool isGameStarted = false;

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
}
