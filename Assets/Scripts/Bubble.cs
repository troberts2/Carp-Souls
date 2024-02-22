using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject player;

    [SerializeField] private float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player"); //temporary dw

        //starting force for clarity, and projectile-ness
        //rb.AddForce(-1000, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        rb.position = Vector3.MoveTowards(rb.position, player.transform.position, speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            Destroy(transform.parent.gameObject);
            Destroy(gameObject);
            //also will hurt the player
            //pop sfx
        }
    }
}
