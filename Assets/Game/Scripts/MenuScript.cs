using Assets.Game.Global;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    private GameObject content;

    void Start()
    {
        content = transform.Find("MenuContent").gameObject;
        content.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    public void UpdateGameState()
    {
        PlayerState.IsPaused = content.activeInHierarchy;
        Cursor.visible = content.activeInHierarchy;
        Cursor.lockState = content.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = content.activeInHierarchy ? 0 : 1f;
    }
}
