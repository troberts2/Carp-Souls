using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossBehavior : MonoBehaviour
{
    public GameObject model;
    private bool fightStarting = true;
    internal float hp;
    public float maxHp = 15;
    public float rotationSpeed = 5f;
    public Transform player;
    public float attackCd;
    private float attackCdTimer;
    private float secondsBetweenAttacks;
    public Image bossHpBar;
    public BossState state;
    internal BulletPatternTemplate currentPattern;
    public BossSettings bossSettings;
    private RandomBossGen randomBossGen;
    private BossAttacks bossAttacks;
    public enum BossState{
        attacking,
        following,
        stunned
    }

    void Start(){
        maxHp = bossSettings.bossMaxHp;
        secondsBetweenAttacks = bossSettings.secondsBetweenAttacks;
        player = FindObjectOfType<PlayerMovement>().transform;
        randomBossGen = FindObjectOfType<RandomBossGen>();
        bossAttacks = GetComponent<BossAttacks>();
        hp = maxHp;
        state = BossState.following;
        StartCoroutine(StartBattleDelay());
    }
    void Update(){
        if (attackCdTimer > 0) attackCdTimer -= Time.deltaTime;
        else if(!fightStarting){
            if(bossAttacks != null && GetComponent<RadialBullets>() != null){
                if(Random.Range(0, 100) > 50){
                    Attack();
                }else{
                    bossAttacks.ChooseAttack();
                }
            }else if(bossAttacks != null){
                bossAttacks.ChooseAttack();
            }else if(GetComponent<RadialBullets>() != null){
                Attack();
            }

        }

        if(state == BossState.following) FollowPlayerOrientation();

        if(state == BossState.attacking && attackCdTimer <= 0){
            if(currentPattern != null){
                attackCdTimer = secondsBetweenAttacks + (currentPattern.secondsPerAttack * currentPattern.repeatTimes) + (currentPattern.attackBreakTime * (currentPattern.repeatTimes -1));
            }else   attackCdTimer = secondsBetweenAttacks;

        }

        if (state == BossState.attacking)
        {
            model.GetComponent<Animator>().SetBool("BubbleAttack", true);
        }
        else
        {
            model.GetComponent<Animator>().SetBool("BubbleAttack", false);
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
            FindObjectOfType<PlayerMovement>().SprayBloodOnHit(collider.transform.position, transform.position);
        }
    }

    IEnumerator TakeDamage(){
        //change to player damage later
        hp--;
        bossHpBar.fillAmount = hp/maxHp;
        if(hp <= 0)
        {
            randomBossGen.lastBoss = bossSettings;

            SceneManager.LoadScene("Win Scene");
        }
        //change later just to show its taking damage
        GetComponent<MeshRenderer>().material.color = Color.magenta;
        yield return new WaitForSeconds(0.1f);
        GetComponent<MeshRenderer>().material.color = Color.red;

        yield return null;
    }

    void Attack(){
        if(bossSettings.bulletPatterns.Length != 0){
            currentPattern = new BulletPatternTemplate(bossSettings.bulletPatterns[Random.Range(0, bossSettings.bulletPatterns.Length)]);
        }
        
        if(GetComponent<RadialBullets>() != null){
            StartCoroutine(GetComponent<RadialBullets>().ShootBullets(currentPattern));
        }

    }
    IEnumerator StartBattleDelay(){
        yield return new WaitForSeconds(5f);
        fightStarting = false;
    }

}
