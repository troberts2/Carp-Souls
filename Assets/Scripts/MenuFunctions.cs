using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuFunctions : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuFirst;
    public GameObject pauseMenu;

    private bool paused = false;

    private DefaultInputActions playerInput;
    private InputAction pauseInput;

    private void Awake()
    {
        playerInput = new DefaultInputActions();
        pauseInput = playerInput.Player.Pause;
    }

    private void OnEnable()
    {

        pauseInput.Enable();
    }

    private void OnDisable()
    {
        pauseInput.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    private void Update()
    {
        if (pauseInput.WasPressedThisFrame() && paused == false)
        {
            Pause();
        }
        else if (pauseInput.WasPressedThisFrame() && paused == true)
        {
            UnPause();
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        paused = true;
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
        Time.timeScale = 0f;
    }

    public void UnPause()
    {
        pauseMenu.SetActive(false);
        paused = false;
        Time.timeScale = 1f;
    }

    public void SceneChange(int sceneID)//sets up scene changing
    {
        Time.timeScale = 1f;
        EventSystem.current.SetSelectedGameObject(null);
        SceneManager.LoadScene(sceneID);
    }

    public static void QuitGame()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Application.Quit();
        /*if (Application.isEditor)
        {
         //   UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }*/

    }

}
