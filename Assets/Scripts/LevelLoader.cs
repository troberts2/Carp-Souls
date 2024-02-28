using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator animator;
    public float transTime = 1f;
    public string sceneNameToLoad;

    // Update is called once per frame
    void Update()
    {
    }

    public void LoadNextLevel(){
        StartCoroutine(LoadLevel(sceneNameToLoad));
    }

    IEnumerator LoadLevel(string name){
        animator.SetTrigger("Start");

        yield return new WaitForSeconds(transTime);

        SceneManager.LoadScene(name);
    }
}
