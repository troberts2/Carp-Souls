using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellBullet : MonoBehaviour
{
    private Rigidbody rb;
    private RadialBullets radialBullets;
    private float bulletSpeed;
    private bool useTheCurve;
    float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        radialBullets = FindObjectOfType<RadialBullets>();
        bulletSpeed = radialBullets.projectileSpeed;
        useTheCurve = radialBullets.useCurve;
    }

    // Update is called once per frame
    void Update()
    {
        loopCurve();
        if(useTheCurve) rb.velocity = transform.forward * bulletSpeed * radialBullets.curve.Evaluate(time);
        else rb.velocity = transform.forward * bulletSpeed;
        bulletSpeed += radialBullets.acceleration;
    }
    void loopCurve(){
        if(time < 1) time += Time.deltaTime;
        if(time > 1) time = 0;
    }
}
