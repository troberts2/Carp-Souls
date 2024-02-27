using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyLockOn : MonoBehaviour
{
    Transform currentTarget;
    Animator anim;

    [SerializeField] LayerMask targetLayers;
    [SerializeField] Transform enemyTarget_Locator;

    [Tooltip("StateDrivenMethod for Switching Cameras")]
    [SerializeField] Animator cinemachineAnimator;

    [Header("Settings")]
    [SerializeField] bool zeroVert_Look;
    [SerializeField] float noticeZone = 10;
    [SerializeField] float lookAtSmoothing = 2;
    [Tooltip("Angle_Degree")] [SerializeField] float maxNoticeAngle = 60;
    [SerializeField] float crossHair_Scale = 0.1f;

    
    Transform cam;
    public bool enemyLocked;
    float currentYOffset;
    Vector3 pos;

    [SerializeField] CameraFollow camFollow;
    [SerializeField] Transform lockOnCanvas;
    [SerializeField] Transform playerObj;
    PlayerMovement defMovement;
    private DefaultInputActions playerInput;
    private InputAction lockOn;

    void Start()
    {
        defMovement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        cam = Camera.main.transform;
        lockOnCanvas.gameObject.SetActive(false);
    }
    private void Awake() {
        playerInput = new DefaultInputActions();
        lockOn = playerInput.Player.LockToEnemy;
    }
    void OnEnable(){
        lockOn.Enable();
        lockOn.performed += lockOnStuff;
    }
    private void OnDisable() {
        lockOn.performed -= lockOnStuff;
    }

    void Update()
    {
        camFollow.lockedTarget = enemyLocked;
        defMovement.lockRotation = enemyLocked;

    }
    private void lockOnStuff(InputAction.CallbackContext context){
        if (currentTarget)
        {
            //If there is already a target, Reset.
            ResetTarget();
            return;
        }
        
        if (currentTarget = ScanNearBy()) FoundTarget(); else ResetTarget();
    }
    private void FixedUpdate() {
        if (enemyLocked) {
            if(!TargetOnRange()) ResetTarget();
            LookAtTarget();
        }
    }


    void FoundTarget(){
        lockOnCanvas.gameObject.SetActive(true);
        cinemachineAnimator.Play("TargetFollow");
        enemyLocked = true;
    }

    void ResetTarget()
    {
        lockOnCanvas.gameObject.SetActive(false);
        currentTarget = null;
        enemyLocked = false;
        cinemachineAnimator.Play("FreeLook");
    }


    private Transform ScanNearBy()
    {
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
        float closestAngle = maxNoticeAngle;
        Transform closestTarget = null;
        if (nearbyTargets.Length <= 0) return null;

        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            Vector3 dir = nearbyTargets[i].transform.position - cam.position;
            dir.y = 0;
            float _angle = Vector3.Angle(cam.forward, dir);
            
            if (_angle < closestAngle)
            {
                closestTarget = nearbyTargets[i].transform;
                closestAngle = _angle;      
            }
        }

        if (!closestTarget ) return null;
        float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
        float h2 = closestTarget.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2) / 2;
        currentYOffset = h - half_h;
        if(zeroVert_Look && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
        Vector3 tarPos = closestTarget.position;
        //if(Blocked(tarPos)) return null;
        return closestTarget;
    }

    bool Blocked(Vector3 t){
        RaycastHit hit;
        if(Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit)){
            if(!hit.transform.CompareTag("Enemy")) return true;
        }
        return false;
    }

    bool TargetOnRange(){
        float dis = (transform.position - pos).magnitude;
        if(dis/2 > noticeZone) return false; else return true;
    }


    private void LookAtTarget()
    {
        if(currentTarget == null) {
            ResetTarget();
            return;
        }
        pos = currentTarget.position + new Vector3(0, currentYOffset, 0);
        lockOnCanvas.position = pos - new Vector3(0, currentYOffset, 0);
        lockOnCanvas.localScale = Vector3.one * ((cam.position - pos).magnitude * crossHair_Scale);

        enemyTarget_Locator.position = pos;
        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        if(defMovement.state != PlayerMovement.MovementState.dashing && defMovement.state != PlayerMovement.MovementState.attacking)    transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * lookAtSmoothing);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, noticeZone);   
    }
}
