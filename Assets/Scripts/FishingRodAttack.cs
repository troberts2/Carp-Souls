using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FishingRodAttack : MonoBehaviour
{
    public Transform bobber;
    public Transform rodTip;
    public float maxAttackDistance;
    public float toCamForce;
    public Vector3 attackPoint;
    LineRenderer lr;
    public bool attacking = false;
    public bool freezePlayer = false;
    private int comboNum = 0;
    private float attackCdTimer;
    public float attackDelayTime;
    public LayerMask attackable;
    public float attackCd;
    private Transform cam;
    public Rigidbody bobberRb;
    public Transform bobberResetPos;

    private void Start(){
        cam = Camera.main.transform;
    }
    Vector3 bobberStartPos;
    private void Update(){
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            StartAttack();
        }
        if (attackCdTimer > 0) attackCdTimer -= Time.deltaTime;

        if(attacking){      
            Attack();
        } 
    }

    private void StartAttack(){
        if(attackCdTimer > 0) return;


        RaycastHit hit;
        
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxAttackDistance, attackable)){
            attackPoint = hit.point;
            freezePlayer = true;
            Debug.DrawRay(cam.position, cam.forward, Color.red, 5f);
            //cast back
            Invoke(nameof(ArcToCam), .025f);

            //cast to target
            StartCoroutine(AttackAfter());
        }
        else{
            attackPoint = cam.position + cam.forward * maxAttackDistance;
            Invoke(nameof(StopAttack), attackDelayTime);
        }
    }

    void ArcToCam(){
        bobberRb.isKinematic = false;
        
        Vector3 forceDir = (cam.position - bobber.position + Vector3.up * 15).normalized;
        switch(comboNum){
            case 0:
                forceDir = (cam.position - bobber.position + Vector3.up * 15 + Vector3.left * 15).normalized;
                break;
            case 1:
                forceDir = (cam.position - bobber.position + Vector3.up * 15 + Vector3.right * 15).normalized;
                break;
            default:
                forceDir = (cam.position - bobber.position + Vector3.up * 15).normalized;
                break;
        }
        bobberRb.AddForce(forceDir * toCamForce, ForceMode.Impulse);
        if(comboNum < 2){
            comboNum++;
        }else{
            comboNum = 0;
        }
        StopCoroutine(ComboNum());
        StartCoroutine(ComboNum());
    }
    float elapsedTime;

    void Attack(){
        attackCdTimer = attackCd;

        elapsedTime += Time.deltaTime;
        float percentComplete = elapsedTime / .25f;

        bobber.position = Vector3.Lerp(bobberStartPos, attackPoint, percentComplete);

        //Invoke(nameof(StopAttack), 2f);
    }
    void StopAttack(){
        freezePlayer = false;
        attacking = false;
        bobber.position = bobberResetPos.position;
        elapsedTime = 0;
    }
    private IEnumerator AttackAfter(){
        yield return new WaitForSeconds(.5f);
        attacking = true;
        bobberStartPos = bobber.position;
        bobberRb.isKinematic = true;
        Invoke(nameof(StopAttack), attackDelayTime);
    }

    private IEnumerator ComboNum(){
        yield return new WaitForSeconds(2f);
        //comboNum--;
    }
}
