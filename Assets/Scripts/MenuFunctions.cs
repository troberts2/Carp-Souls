using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuFunctions : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuFirst;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    public void SceneChange(int sceneID)//sets up scene changing
    {
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
