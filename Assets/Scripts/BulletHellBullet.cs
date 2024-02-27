using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellBullet : MonoBehaviour
{
    private Rigidbody rb;
    private RadialBullets radialBullets;
    private float bulletSpeed = 0;
    private bool useTheCurve;
    float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        radialBullets = FindObjectOfType<RadialBullets>();

    }
    private void Awake() {
        radialBullets = FindObjectOfType<RadialBullets>();
    }
    private void OnEnable() {
        Invoke(nameof(NoLongerStart), .01f);
        Invoke(nameof(DisableObj), 5f);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        loopCurve();
        if(useTheCurve) rb.velocity = transform.forward * bulletSpeed * radialBullets.currentPattern.curve.Evaluate(time);
        else rb.velocity = transform.forward * bulletSpeed;
        bulletSpeed += radialBullets.currentPattern.acceleration;
    }
    void loopCurve(){
        if(time < 1) time += Time.deltaTime;
        if(time > 1) time = 0;
    }
    private void OnCollisionEnter(Collision other) {
        gameObject.SetActive(false);
    }
    private void DisableObj(){
        if(gameObject.activeInHierarchy){
            gameObject.SetActive(false);
        }
    }
    private void NoLongerStart(){
        bulletSpeed = radialBullets.currentPattern.projectileSpeed;
        useTheCurve = radialBullets.currentPattern.useCurve; 
    }
}
