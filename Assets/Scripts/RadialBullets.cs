using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadialBullets : MonoBehaviour
{
    public BulletPattern bulletPattern1;

    internal BulletPatternTemplate currentPattern;

    private float timeTillFire;
    private bool increase = true;
    public GameObject ProjectilePrefab; 
    public bool radial = false;        // Prefab to spawn.

    [Header("Private Variables")]
    private Vector3 startPoint;                 // Starting position of the bullet.
    private const float radius = 5F;          // Help us find the move direction.
    private BossBehavior bb;
    private void Start() {
        bb = GetComponent<BossBehavior>();
        currentPattern = new BulletPatternTemplate(bulletPattern1);
    }



    // Update is called once per frame
    void Update()
    {
        if (timeTillFire <= 0 && Input.GetKey(KeyCode.Space))
        {
            startPoint = transform.position;
            StartCoroutine(ShootBullets());
        }
        else{
            timeTillFire -= Time.deltaTime;
        }

        if(true){ //bb.state == BossBehavior.BossState.attacking
            transform.Rotate(Vector3.up * currentPattern.attackRotateSpeed * Time.deltaTime);
        }
        if(currentPattern.rotateSpeedChangeRate > 0)   SpinSpeedChange();
    }



    private IEnumerator ShootBullets(){
        currentPattern = new BulletPatternTemplate(bulletPattern1);;
        timeTillFire = currentPattern.fireRate;
        float angleStep;
        float arrayAngleStep;

        float angle = 0f;
        float arrayAngle = 0f;

        if(radial){
            angleStep = 360 / currentPattern.numberOfProjectilesPerArray;
            arrayAngleStep = 360/ currentPattern.numOfArrays;
        }else{
            if(currentPattern.numberOfProjectilesPerArray > 1){
                angleStep = currentPattern.individualArraySpread/(currentPattern.numberOfProjectilesPerArray -1);
            }else{
                angleStep = currentPattern.individualArraySpread/currentPattern.numberOfProjectilesPerArray;
            }
            
            arrayAngleStep = currentPattern.totalArraySpread;
        }

        for(int x = 0; x < currentPattern.numOfArrays; x++){
            for(int i = 0; i < currentPattern.numberOfProjectilesPerArray; i++){
                float projectileDirXposition = transform.position.x + Mathf.Sin(((angle + arrayAngle) * Mathf.PI) / 180) * 5;
                float projectileDirZposition = transform.position.z + Mathf.Cos(((angle + arrayAngle) * Mathf.PI) / 180) * 5;
                
                Vector3 projectileVector = new Vector3(projectileDirXposition, 0, projectileDirZposition);
                Vector3 projectileMoveDirection = (projectileVector - (Vector3)transform.position).normalized * 5;

                var proj = ObjectPool.instance.GetPooledObject();//Instantiate(ProjectilePrefab, transform.position, Quaternion.Euler(0, angle - arrayAngle, 0)); //Quaternion.LookRotation(projectileMoveDirection, Vector3.up)
                //if(proj == null) yield break;
                proj.transform.position = transform.position;
                proj.transform.rotation = Quaternion.Euler(0, angle -  arrayAngle, 0);
                proj.transform.rotation *= transform.rotation;
                proj.SetActive(true);
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
            currentPattern.attackRotateSpeed =Mathf.Lerp(currentPattern.attackRotateSpeed, currentPattern.maxSpinSpeed, currentPattern.rotateSpeedChangeRate * Time.deltaTime);
            if(currentPattern.attackRotateSpeed>= currentPattern.maxSpinSpeed - .05f) increase = false;
        }else{
            currentPattern.attackRotateSpeed =Mathf.Lerp(currentPattern.attackRotateSpeed, -currentPattern.maxSpinSpeed, currentPattern.rotateSpeedChangeRate * Time.deltaTime);
            if(currentPattern.attackRotateSpeed <= -currentPattern.maxSpinSpeed + .05f) increase = true;
        }
    }
}
public class BulletPatternTemplate{
    internal int numberOfProjectilesPerArray;  
    internal int individualArraySpread;  
    internal int numOfArrays;   
    internal int totalArraySpread;
    internal float projectileSpeed;               // Speed of the projectile.
    internal float acceleration;
    internal AnimationCurve curve;
    internal bool useCurve;
    internal float attackRotateSpeed;
    internal float rotateSpeedChangeRate;
    internal float fireRate;
    internal float maxSpinSpeed;

    public BulletPatternTemplate(BulletPattern bulletPattern){
        numberOfProjectilesPerArray = bulletPattern.numberOfProjectilesPerArray;
        individualArraySpread = bulletPattern.individualArraySpread;
        numOfArrays = bulletPattern.numOfArrays;
        totalArraySpread = bulletPattern.totalArraySpread;
        projectileSpeed = bulletPattern.projectileSpeed;
        acceleration = bulletPattern.acceleration;
        curve = bulletPattern.curve;
        useCurve = bulletPattern.useCurve;
        attackRotateSpeed = bulletPattern.attackRotateSpeed;
        rotateSpeedChangeRate = bulletPattern.rotateSpeedChangeRate;
        fireRate = bulletPattern.fireRate;
        maxSpinSpeed = bulletPattern.maxSpinSpeed;
    }
}
