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

    private List<string> items = new List<string>();

    [SerializeField]
    private string[] shopItems = new string[3];

    [SerializeField]
    private string[] rodItems;

    [SerializeField]
    private string[] gearItems;

    [SerializeField]
    private List<string> totalItems = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;

        totalItems.AddRange(rodItems);
        totalItems.AddRange(gearItems);
        totalItems.Remove("");

        foreach (string item in shopItems)
        {
            switch (i)
            {
                case 0:
                    shopItems[i] = rodItems[Random.Range(0, rodItems.Length)];
                    break;
                case 1:
                    shopItems[i] = gearItems[Random.Range(0, gearItems.Length)];
                    break;
                case 2:
                    totalItems.Remove(shopItems[0]);
                    totalItems.Remove(shopItems[1]);
                    shopItems[i] = totalItems.ToArray()[Random.Range(0, totalItems.ToArray().Length)];
                    break;

            }
            
            i++;
        }

        AlterDuplicates();

        item1.text = shopItems[0];
        item2.text = shopItems[1];
        item3.text = shopItems[2];
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
        int i = 0;


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
