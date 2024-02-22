using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttacks : MonoBehaviour
{
    [SerializeField] private GameObject waveAttack;
    [SerializeField] private GameObject bubbleAttack;
    private GameObject spinAttack;

    private Vector3 startPos;
    private Quaternion startRot;

    [SerializeField] private float jumpHeight;
    [SerializeField] private float spinTime;

    private bool charging;
    private bool jumping;
    private bool falling;
    private bool spinning;

    private Material bossMat;
    private Color bossCol;

    void Start()
    {
        spinTime = 2f;
        spinAttack = transform.GetChild(0).gameObject;

        jumpHeight += transform.position.y;

        startPos = transform.position;
        startRot = transform.rotation;

        bossMat = GetComponent<MeshRenderer>().material;
        bossCol = bossMat.color;
    }

    // Update is called once per frame
    void Update()
    {
        //1 is shockwave attack, 2 is bubble attack, 3 is spin attack
        if (Input.GetKeyDown(KeyCode.Alpha1) && !jumping)
        {
            jumping = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(Beam());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(Spin());
        }


        if (jumping)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + jumpHeight), 0.01f);

            Debug.Log("goin up");
        }
        //if (falling)
        //{
        //    transform.position = Vector3.Lerp(transform.position, startPos, 0.1f);
        //}

        if (transform.position.y >= jumpHeight && jumping)
        {
            jumping = false;
            StartCoroutine(StartWave());
        }
        //if (transform.position.y <= startPos.y)
        //{
        //    falling = false;
        //    transform.position = startPos;
        //}

    }

    IEnumerator StartWave()
    {   
        yield return new WaitForSeconds(1);

        //falling = true;
        transform.position = startPos;
        Instantiate(waveAttack);
    }

    IEnumerator Beam()
    {
        charging = true;
        StartCoroutine(Charge());
        yield return new WaitForSeconds(2.2f);
        charging = false;
        gameObject.GetComponent<MeshRenderer>().material.color = bossCol;

        Instantiate(bubbleAttack);

    }

    IEnumerator Spin()
    {
        charging = true;
        StartCoroutine(Charge());
        yield return new WaitForSeconds(2.2f);
        charging = false;
        gameObject.GetComponent<MeshRenderer>().material.color = bossCol;

        spinning = true;
        spinAttack.SetActive(true);

        float t = 0;
        Quaternion targetRotA = transform.rotation * Quaternion.Euler(0, 170, 0);
        Quaternion targetRotB = transform.rotation * Quaternion.Euler(0, 190, 0);



        while (t < spinTime)
        {
            //Debug.Log("yup");
            transform.rotation = Quaternion.Slerp(startRot, targetRotA, (t / spinTime) * 2);
            t += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotA;

        t = 0;
        while (t < spinTime)
        {
            //Debug.Log("YEAH");
            transform.rotation = Quaternion.Slerp(targetRotB, startRot, (t / spinTime) * 2);
            t += Time.deltaTime;
            yield return null;
        }
        transform.rotation = startRot;
        spinning = false;
        spinAttack.SetActive(false);
    }

    IEnumerator Charge()
    {
        while (charging)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
            yield return new WaitForSeconds(0.2f);
            gameObject.GetComponent<MeshRenderer>().material.color = bossCol;
            yield return new WaitForSeconds(0.2f);

        }
    }

}
