using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomBossGen : MonoBehaviour
{
    public GameObject[] bossList;
    public Transform bossSpawnPos;
    private Vector3 ogBossSpawnPos;
    private BossSettings currentBoss;
    public BossSettings lastBoss;
    private float currentMultiplier = .8f;
    public float hpScaleAmount = .2f;
    void Start()
    {
        
        ogBossSpawnPos = bossSpawnPos.position;
    }
    void OnEnable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void SpawnBossRandom(){
        if(SceneManager.GetActiveScene().name == "ArenaTest"||SceneManager.GetActiveScene().name == "BattleArenaCircle")
        {
            bossSpawnPos.position = ogBossSpawnPos;
            if(bossList.Length > 0){
                GameObject bossToSpawn = bossList[Random.Range(0, bossList.Length)];
                while(bossToSpawn.GetComponent<BossBehavior>().bossSettings == lastBoss){
                    bossToSpawn = bossList[Random.Range(0, bossList.Length)];
                }
                bossSpawnPos.position = new Vector3(bossSpawnPos.position.x, bossSpawnPos.position.y + bossToSpawn.GetComponent<BossBehavior>().bossSettings.bossSpawnYOffset, bossSpawnPos.position.z);
                GameObject theSpawnedBoss = Instantiate(bossToSpawn, bossSpawnPos.position, Quaternion.identity);
                currentBoss = bossToSpawn.GetComponent<BossBehavior>().bossSettings;
                currentMultiplier += hpScaleAmount;
                theSpawnedBoss.GetComponent<BossBehavior>().hp *= currentMultiplier;

                if(lastBoss == null){
                    lastBoss = currentBoss;
                }
            }
        }

    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        SpawnBossRandom();
    }
}
