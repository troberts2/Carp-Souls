using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossBehavior : MonoBehaviour
{
    private bool fightStarting = true;
    private float hp;
    public float maxHp = 15;
    public float rotationSpeed = 5f;
    public Transform player;
    public float attackCd;
    private float attackCdTimer = 60;
    private float secondsBetweenAttacks;
    public Image bossHpBar;
    public BossState state;
    internal BulletPatternTemplate currentPattern;
    public BossSettings bossSettings;
    public enum BossState{
        attacking,
        following,
        stunned
    }

    void Start(){
        maxHp = bossSettings.bossMaxHp;
        secondsBetweenAttacks = bossSettings.secondsBetweenAttacks;
        hp = maxHp;
        state = BossState.following;
        StartCoroutine(StartBattleDelay());
    }
    void Update(){
        if (attackCdTimer > 0) attackCdTimer -= Time.deltaTime;
        else if(!fightStarting) Attack();

        if(state == BossState.following) FollowPlayerOrientation();

        if(state == BossState.attacking && attackCdTimer <= 0){
            attackCdTimer = secondsBetweenAttacks + (currentPattern.secondsPerAttack * currentPattern.repeatTimes) + (currentPattern.attackBreakTime * (currentPattern.repeatTimes -1));

        }
        
        if(hp == 0)
        {
            SceneManager.LoadScene("Win Scene");
        }
    }

    void FollowPlayerOrientation(){
        Vector3 lookDir = player.position - transform.position;
        lookDir = new Vector3(lookDir.x, 0, lookDir.z);
        transform.forward = Vector3.Slerp(transform.forward, lookDir.normalized, Time.deltaTime * rotationSpeed);
    }

    void OnTriggerEnter(Collider collider){
        if(collider.CompareTag("playerBobber")){
            StartCoroutine(TakeDamage());
        }
    }

    IEnumerator TakeDamage(){
        //change to player damage later
        hp--;
        Debug.Log("hit" + hp);
        bossHpBar.fillAmount = hp/maxHp;
        //change later just to show its taking damage
        GetComponent<MeshRenderer>().material.color = Color.magenta;
        yield return new WaitForSeconds(0.1f);
        GetComponent<MeshRenderer>().material.color = Color.red;

        yield return null;
    }

    void Attack(){
        currentPattern = new BulletPatternTemplate(bossSettings.bulletPatterns[Random.Range(0, bossSettings.bulletPatterns.Length)]);
        if(GetComponent<RadialBullets>() != null){
            StartCoroutine(GetComponent<RadialBullets>().ShootBullets(currentPattern));
        }

    }
    IEnumerator StartBattleDelay(){
        yield return new WaitForSeconds(5f);
        fightStarting = false;
    }

}
