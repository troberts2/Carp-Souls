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
    public bool retract = false;
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
        if(retract){
            RetractLine();
        }
        if(bobber.position == bobberResetPos.position){
            retract = false;
            elapsedRetractTime = 0;
        }
    }

    private void StartAttack(){
        if(attackCdTimer > 0) return;

        freezePlayer = true;

        attackCdTimer = attackCd;


        RaycastHit hit;
        
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxAttackDistance, attackable)){
            attackPoint = hit.point;
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
                forceDir = (cam.position - bobber.position + Vector3.up * 20 + Vector3.left * 20f).normalized;
                break;
            case 1:
                forceDir = (cam.position - bobber.position + Vector3.up * 20 + Vector3.right * 20f).normalized;
                break;
            case 2:
                forceDir = (cam.position - bobber.position + Vector3.up * 15).normalized;
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

        elapsedTime += Time.deltaTime;
        float percentComplete = elapsedTime / attackDelayTime;

        bobber.position = Vector3.Lerp(bobberStartPos, attackPoint, percentComplete);

        //Invoke(nameof(StopAttack), 2f);
    }
    void StopAttack(){
        Invoke(nameof(UnfreezePlayer), .2f);
        attacking = false;
        Invoke(nameof(Retract), .1f);
        elapsedTime = 0;
    }
    float elapsedRetractTime;
    void RetractLine(){
        elapsedRetractTime += Time.deltaTime;
        float percentComplete = elapsedRetractTime / .1f;

        bobber.position = Vector3.Lerp(bobber.position, bobberResetPos.position, percentComplete);
    }
    private IEnumerator AttackAfter(){
        yield return new WaitForSeconds(.5f);
        attacking = true;
        bobberStartPos = bobber.position;
        bobberRb.isKinematic = true;
        Invoke(nameof(StopAttack), attackDelayTime);
    }
    private void UnfreezePlayer(){
        freezePlayer = false;
    }
    private void Retract(){
        retract = true;
    }

    private IEnumerator ComboNum(){
        yield return new WaitForSeconds(2f);
        //comboNum--;
    }
}
