using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    static private int fishBucks;
    private int beerCost;

    private float bootSpeed;
    private int bootGain;

    [SerializeField] private GameObject loadout;
 
    [SerializeField] private TextMeshProUGUI item1;
    [SerializeField] private TextMeshProUGUI item2;
    [SerializeField] private TextMeshProUGUI item3;

    //Update texts
    [SerializeField] private TextMeshProUGUI[] itemTexts;
    [SerializeField] private TextMeshProUGUI fishBucksText;
    [SerializeField] private TextMeshProUGUI beerCostText;
    [SerializeField] private TextMeshProUGUI item1CostText;
    [SerializeField] private TextMeshProUGUI item2CostText;
    [SerializeField] private TextMeshProUGUI item3CostText;

    private string item;

    [SerializeField] private string[] shopItems = new string[3];
    [SerializeField] private string[] rodItems;
    [SerializeField] private string[] gearItems;
    [SerializeField] private List<string> totalItems = new List<string>();

    [SerializeField] private PlayerMovement pm;


    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        beerCost = 5;

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            Beer();
        }
        
        if (gameObject.activeInHierarchy)
        {
            TextUpdates();
        }
        if (fishBucksText != null)
        {
            fishBucksText.text = "Fish Bucks: " + fishBucks;
        }
        if (beerCostText != null)
        {
            beerCostText.text = "Cost: " + beerCost;
        }

    }

    public void ItemSelected()
    {
        item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        ItemEffect();
    }

    private void ItemEffect()
    {
        if (item == "Steel Rod")
        {
            //dmg increase
        }
        if (item == "Ultralight Rod")
        {
            //atk speed increase
        }
        if (item == "Fling Rod")
        {
            //range increase
        }
        if (item == "Rain Boots")
        {
            bootSpeed += bootGain;
            //move spd increase in water
            //move spd increase + dash
        }
        if (item == "Pine Tree")
        {
            //chance to skip boss atk? exponential descrease
        }
        if (item == "Catsup")
        {
            //cattails heal
            //after 1, cattail heal amount increase
            //can't be picked up at full hp
        }
    }

    private void TextUpdates()
    {
        foreach (TextMeshProUGUI text in itemTexts)
        {
            if (text != null)
            {

            }
        }
    }

    public void Beer()
    {
        if (beerCost <= fishBucks)
        {
            fishBucks -= beerCost;
            pm.hp = pm.maxHp;
            beerCost += 2;
        }
        else
        {
            Debug.Log("Too poor");
        }
    }

}
