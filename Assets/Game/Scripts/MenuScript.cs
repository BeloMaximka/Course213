using Assets.Game.Global;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private GameObject content;

    void Start()
    {
        content = transform.Find("MenuContent").gameObject;
        content.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerState.HealthChanged += CheckDeath;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            content.SetActive(!content.activeInHierarchy);
            UpdateGameState();
        }
    }

    public void OnClose()
    {
        content.SetActive(false);
        UpdateGameState();
    }

    public void CheckDeath(int health)
    {
        if(health <= 0)
        {
            OnBackToMenu();
        }
    }
    public void OnBackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        PlayerState.Reset();
        GameEntities.Clear();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }

    public void UpdateGameState()
    {
        PlayerState.IsPaused = content.activeInHierarchy;
        Cursor.visible = content.activeInHierarchy;
        Cursor.lockState = content.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = content.activeInHierarchy ? 0 : 1f;
    }

    private void OnDestroy()
    {
        PlayerState.HealthChanged -= CheckDeath;
    }
}
