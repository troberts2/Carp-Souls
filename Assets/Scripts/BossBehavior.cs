using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossBehavior : MonoBehaviour
{
    private float hp;
    public float maxHp = 15;
    public float rotationSpeed = 5f;
    public Transform player;
    public float attackCd;
    private float attackCdTimer;
    public Image bossHpBar;
    public BossState state;
    public enum BossState{
        attacking,
        following,
        stunned
    }

    void Start(){
        hp = maxHp;
        state = BossState.following;
    }
    void Update(){
        if (attackCdTimer > 0) attackCdTimer -= Time.deltaTime;

        if(state == BossState.following) FollowPlayerOrientation();

        if(state == BossState.attacking){
            attackCdTimer = attackCd;

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

}
