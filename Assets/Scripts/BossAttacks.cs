using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttacks : MonoBehaviour
{
    [SerializeField] private GameObject waveAttack;
    [SerializeField] private GameObject bubbleAttack;
    [SerializeField] private GameObject hydroAttack;
    private GameObject spinAttack;
    public Transform player;

    private Vector3 startPos;
    private Vector3 rotateDirection;

    [SerializeField] private float jumpHeight;
    [SerializeField] private float spinSpeed;
    [SerializeField] private float spinTime;
    [SerializeField] private float attackTimer;
    [SerializeField] private float spinTriggerDistance;

    private bool charging;
    private bool jumping;
    private int chooseInt;
    private int repeatCheck;
    private int repetitions;

    public bool doubleWave;
    public bool tripleWave;

    private Material bossMat;
    private Color bossCol;

    private BossBehavior bb;

    void Start()
    {
        bb = FindObjectOfType<BossBehavior>();
        player = FindObjectOfType<PlayerMovement>().transform;

        spinAttack = transform.GetChild(0).gameObject;

        jumpHeight += transform.position.y;

        startPos = transform.position;

        bossMat = GetComponent<MeshRenderer>().material;
        bossCol = bossMat.color;
        repetitions = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (jumping)
        {
            bb.state = BossBehavior.BossState.attacking;

            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + jumpHeight), 0.01f);

        }

        if (transform.position.y >= jumpHeight && jumping)
        {
            jumping = false;
            StartCoroutine(StartWave());
        }
    }

    IEnumerator StartWave()
    {   
        yield return new WaitForSeconds(1);
        
        transform.position = startPos;
        Instantiate(waveAttack);

        if (tripleWave)
        {
            yield return new WaitForSeconds(3);
            Instantiate(waveAttack);
            yield return new WaitForSeconds(3);
            Instantiate(waveAttack);
        }
        else if (doubleWave)
        {
            yield return new WaitForSeconds(2);
            Instantiate(waveAttack);
        }


        bb.state = BossBehavior.BossState.following;
    }

    IEnumerator Hydro()
    {
        charging = true;
        StartCoroutine(Charge());
        yield return new WaitForSeconds(2.2f);
        charging = false;
        gameObject.GetComponent<MeshRenderer>().material.color = bossCol;

        float i = 0;

        while (i < 3)
        {
            i++;

            Instantiate(hydroAttack);
            hydroAttack.transform.position = transform.position; //temporary use of Mouth child
 

            yield return new WaitForSeconds(1);
        }
        bb.state = BossBehavior.BossState.following;
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
        bb.state = BossBehavior.BossState.attacking;

        charging = true;
        StartCoroutine(Charge());
        yield return new WaitForSeconds(2.2f);
        charging = false;
        gameObject.GetComponent<MeshRenderer>().material.color = bossCol;

        spinAttack.SetActive(true);

        float t = 0;

        rotateDirection = new Vector3(0, 1, 0);

        while (t < spinTime)
        {
            t += Time.deltaTime;
            transform.Rotate(spinSpeed * rotateDirection * Time.deltaTime);
            yield return null;
        }

        spinAttack.SetActive(false);

        bb.state = BossBehavior.BossState.following;

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

    public void ChooseAttack()
    {
        bb.state = BossBehavior.BossState.attacking;
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

            }
            else
            {
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


    }
}
