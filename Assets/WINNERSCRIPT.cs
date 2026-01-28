using UnityEngine;
using UnityEngine.SceneManagement; 
public class WinZone : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject winPanel; 

    private bool hasWon = false;

    void OnTriggerEnter(Collider other)
    {
        // checkingplayer reaches end of maze
        if (other.CompareTag("Player") && !hasWon)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        hasWon = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        //stop time so no accidental death
        Time.timeScale = 0f;
    }

    
    void Update()
    {
        if (hasWon)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                // unfreeze time
                Time.timeScale = 1f; 
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
              
                Debug.Log("Quit Game"); 
            }
        }
    }
}