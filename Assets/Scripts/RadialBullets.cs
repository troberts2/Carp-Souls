using System.Collections;
using UnityEngine;

public class RadialBullets : MonoBehaviour
{
    [Header("Projectile Settings")]
    public int numberOfProjectilesPerArray;  
    public int individualArraySpread = 180;  
    public int numOfArrays = 1;   
    public int totalArraySpread = 90;
    [Header("speed Settings")]
    public float projectileSpeed = 7f;               // Speed of the projectile.
    public float acceleration = .1f;
    public AnimationCurve curve;
    public bool useCurve = false;
    public float attackRotateSpeed = 15f;
    public float rotateSpeedChangeRate = .5f;
    public float fireRate = 4f;
    private float timeTillFire;
    bool increase = true;
    public float maxSpinSpeed = 30f;
    public GameObject ProjectilePrefab; 
    public bool radial = false;        // Prefab to spawn.

    [Header("Private Variables")]
    private Vector3 startPoint;                 // Starting position of the bullet.
    private const float radius = 5F;          // Help us find the move direction.
    private BossBehavior bb;
    private void Start() {
        bb = GetComponent<BossBehavior>();
    }



    // Update is called once per frame
    void Update()
    {
        if (timeTillFire <= 0)
        {
            startPoint = transform.position;
            StartCoroutine(ShootBullets());
        }
        else{
            timeTillFire -= Time.deltaTime;
        }

        if(true){ //bb.state == BossBehavior.BossState.attacking
            transform.Rotate(Vector3.up * attackRotateSpeed * Time.deltaTime);
        }
        if(rotateSpeedChangeRate > 0)   SpinSpeedChange();
    }



    private IEnumerator ShootBullets(){
        timeTillFire = fireRate;
        float angleStep;
        float arrayAngleStep;

        float angle = 0f;
        float arrayAngle = 0f;

        if(radial){
            angleStep = 360 / numberOfProjectilesPerArray;
            arrayAngleStep = 360/ numOfArrays;
        }else{
            if(numberOfProjectilesPerArray > 1){
                angleStep = individualArraySpread/(numberOfProjectilesPerArray -1);
            }else{
                angleStep = individualArraySpread/numberOfProjectilesPerArray;
            }
            
            arrayAngleStep = totalArraySpread;
        }

        for(int x = 0; x < numOfArrays; x++){
            for(int i = 0; i < numberOfProjectilesPerArray; i++){
                float projectileDirXposition = transform.position.x + Mathf.Sin(((angle + arrayAngle) * Mathf.PI) / 180) * 5;
                float projectileDirZposition = transform.position.z + Mathf.Cos(((angle + arrayAngle) * Mathf.PI) / 180) * 5;
                
                Vector3 projectileVector = new Vector3(projectileDirXposition, 0, projectileDirZposition);
                Vector3 projectileMoveDirection = (projectileVector - (Vector3)transform.position).normalized * 5;

                var proj = Instantiate(ProjectilePrefab, transform.position, Quaternion.Euler(0, angle - arrayAngle, 0)); //Quaternion.LookRotation(projectileMoveDirection, Vector3.up)
                proj.transform.rotation *= transform.rotation;
                Destroy(proj, 10f);
                angle += angleStep;
            }
            //add wait here if u want wait between attack arrays
            arrayAngle += arrayAngleStep;
            angle = 0;
        }
        yield return null;

    }
    void SpinSpeedChange(){
        if(increase){
            attackRotateSpeed =Mathf.Lerp(attackRotateSpeed, maxSpinSpeed, rotateSpeedChangeRate * Time.deltaTime);
            if(attackRotateSpeed>= maxSpinSpeed - .05f) increase = false;
        }else{
            attackRotateSpeed =Mathf.Lerp(attackRotateSpeed, -maxSpinSpeed, rotateSpeedChangeRate * Time.deltaTime);
            if(attackRotateSpeed <= -maxSpinSpeed + .05f) increase = true;
        }
    }
}