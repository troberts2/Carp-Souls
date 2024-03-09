using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydroBullet : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject player;

    [SerializeField] private float speed;
    private float timeAlive = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player"); //temporary dw
        transform.LookAt(player.transform, Vector3.down);
        transform.Rotate(0, 90, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= (transform.right * Time.deltaTime * speed);
        timeAlive += Time.deltaTime;
        if (timeAlive > 10)
        {
            Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player || collision.collider.CompareTag("Ground"))
        {
            Destroy(transform.parent.gameObject);
            Destroy(gameObject);
            //also will hurt the player
            //pop sfx
        }
    }
}
