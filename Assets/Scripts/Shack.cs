using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shack : MonoBehaviour
{
    private GameObject player;
    private GameObject shopManager;

    private int shopRange;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;

        shopManager = FindObjectOfType<ShopManager>().gameObject;
        shopManager.SetActive(false);

        shopRange = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if (shopManager.activeInHierarchy == true && Input.GetKeyDown(KeyCode.Q))
        {
            shopManager.SetActive(false);
        }
        else if (Vector3.Distance(transform.position, player.transform.position) < shopRange && Input.GetKeyDown(KeyCode.Q))
        {
            shopManager.SetActive(true);
        }
        
    }
}
