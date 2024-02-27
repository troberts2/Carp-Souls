using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttacks : MonoBehaviour
{
    [SerializeField] private GameObject waveAttack;
    [SerializeField] private GameObject bubbleAttack;
    [SerializeField] private GameObject hydroAttack;
    private GameObject spinAttack;
    private GameObject player;

    private Vector3 startPos;
    private Vector3 rotateDirection;

    private Quaternion target;

    [SerializeField] private float jumpHeight;
    [SerializeField] private float spinSpeed;
    [SerializeField] private float spinTime;
    [SerializeField] private float attackTimer;
    [SerializeField] private float spinTriggerDistance;
    [SerializeField] private float lookSpeed;

    private bool charging;
    private bool jumping;
    private bool falling;
    private bool spinning;

    private bool chooseFlag;
    private int chooseInt;
    private int repeatCheck;
    private int repetitions;

    private Material bossMat;
    private Color bossCol;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        spinAttack = transform.GetChild(0).gameObject;

        jumpHeight += transform.position.y;

        startPos = transform.position;

        bossMat = GetComponent<MeshRenderer>().material;
        bossCol = bossMat.color;

        StartCoroutine(ChooseAttack());
        repetitions = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (jumping)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + jumpHeight), 0.01f);

            //Debug.Log("goin up");
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

        if (!spinning || !jumping)
        {
            //better rotation, but doesn't work right
            //target = Quaternion.LookRotation(player.transform.position - transform.position);
            //transform.localRotation = Quaternion.RotateTowards(transform.rotation, target, lookSpeed * Time.deltaTime);

            transform.LookAt(player.transform);
            transform.Rotate(0, 90, 0);
        }

    }

    IEnumerator StartWave()
    {   
        yield return new WaitForSeconds(1);

        //falling = true;
        transform.position = startPos;
        Instantiate(waveAttack);
    }

    IEnumerator Hydro()
    {
        charging = true;
        StartCoroutine(Charge());
        yield return new WaitForSeconds(2.2f);
        charging = false;
        gameObject.GetComponent<MeshRenderer>().material.color = bossCol;

        Instantiate(hydroAttack);
        hydroAttack.transform.position = transform.GetChild(1).position; //temporary use of Mouth child
        
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

        rotateDirection = new Vector3(0, 1, 0);

        while (t < spinTime)
        {
            t++;
            transform.Rotate(spinSpeed * rotateDirection * Time.deltaTime);
            yield return null;
        }

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

    IEnumerator ChooseAttack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < spinTriggerDistance)
        {
            //Debug.Log(Vector3.Distance(transform.position, player.transform.position));
            StartCoroutine(Spin());
        }
        else
        {
            if (repetitions < 2)
            {
                chooseInt = Random.Range(1, 3);

                if (repeatCheck == chooseInt)
                {
                    repetitions++;
                }
                else
                {
                    repetitions = 0;
                }

                repeatCheck = chooseInt;

                Debug.Log(repetitions);
            }
            else
            {
                Debug.Log("ending repeat");
                repetitions = 0;
                if (chooseInt == 1)
                {
                    chooseInt++;
                }
                else
                {
                    chooseInt--;
                }

            }

            switch (chooseInt)
            {
                case 1:
                    StartCoroutine(Hydro());
                    break;
                case 2:
                    jumping = true;
                    break;
            }
        }

        yield return new WaitForSeconds(attackTimer);

        StartCoroutine(ChooseAttack());
    }
}
