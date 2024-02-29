using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField] private float maxSize;
    [SerializeField] private float rate;
    [SerializeField] private float knockback;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x + 1, 1, transform.localScale.z + 1), rate);
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localScale.x + 1, 0, 0), rate);


        if (transform.localScale.x >= maxSize)
        {
            Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player"  || other.collider.CompareTag("Ground"))
        {
            Vector3 direction = other.collider.ClosestPointOnBounds(-transform.position);
            direction = -direction.normalized;

            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(direction * -knockback);

        }
    }
}
