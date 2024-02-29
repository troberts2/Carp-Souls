using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    private GameObject loadout;

    [SerializeField]
    private TextMeshProUGUI item1;
    [SerializeField]
    private TextMeshProUGUI item2;
    [SerializeField]
    private TextMeshProUGUI item3;

    private List<int> items = new List<int>();

    [SerializeField]
    private int[] shopItems = new int[3];

    [SerializeField]
    private string[] possibleItems;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;

        foreach (int item in shopItems)
        {
            shopItems[i] = Random.Range(0, possibleItems.Length + 1);
            AlterDuplicates();
            i++;
        }

        item1.text = shopItems[0].ToString();
        item2.text = shopItems[1].ToString();
        item3.text = shopItems[2].ToString();
    }

    private void Update()
    {
        //temporary for debug purposes
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ItemSelected()
    {
        Debug.Log("button");

    }

    private void AlterDuplicates()
    {
        //for (int j = 0; j < shopItems.Length; j++)
        //{
        //    if (shopItems[i] == shopItems[j])
        //    {
        //        if (shopItems[i] <= shopItems.Length)
        //        {
        //            shopItems[i]++;
        //        }
        //        else
        //        {
        //            shopItems[i]--;
        //        }
        //    }
        //}
    }

}
