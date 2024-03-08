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

    private float bootSpeedMultiplier = 1.5f;
    private int bootGain = 2;

    [SerializeField] private GameObject loadout;
 
    [SerializeField] private TextMeshProUGUI item1;
    [SerializeField] private TextMeshProUGUI item2;
    [SerializeField] private TextMeshProUGUI item3;

    //Update texts
    [SerializeField] private string[] itemTexts;
    [SerializeField] private int[] itemCosts;
    [SerializeField] private TextMeshProUGUI fishBucksText;
    [SerializeField] private TextMeshProUGUI beerCostText;

    [SerializeField] private TextMeshProUGUI item1CostText;
    [SerializeField] private TextMeshProUGUI item2CostText;
    [SerializeField] private TextMeshProUGUI item3CostText;

    [SerializeField] private TextMeshProUGUI item1Description;
    [SerializeField] private TextMeshProUGUI item2Description;
    [SerializeField] private TextMeshProUGUI item3Description;

    private string chosenText;
    private int chosenCost;

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
                    ChooseText(shopItems[i]);
                    ChooseCost(shopItems[i]);
                    item1CostText.text = chosenCost.ToString();
                    item1Description.text = chosenText;
                    break;
                case 1:
                    shopItems[i] = gearItems[Random.Range(0, gearItems.Length)];
                    ChooseText(shopItems[i]);
                    ChooseCost(shopItems[i]);
                    item2CostText.text = chosenCost.ToString();
                    item2Description.text = chosenText;
                    break;
                case 2:
                    totalItems.Remove(shopItems[0]);
                    totalItems.Remove(shopItems[1]);
                    shopItems[i] = totalItems.ToArray()[Random.Range(0, totalItems.ToArray().Length)];
                    ChooseText(shopItems[i]);
                    ChooseCost(shopItems[i]);
                    item3CostText.text = chosenCost.ToString();
                    item3Description.text = chosenText;
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
            pm.walkSpeed += bootSpeedMultiplier;
            bootSpeedMultiplier += bootGain;
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


    private void ChooseText(string item)
    {
        if (item == "Steel Rod")
        {
            chosenText = itemTexts[0];
        }
        if (item == "Ultralight Rod")
        {
            chosenText = itemTexts[1];
        }
        if (item == "Fling Rod")
        {
            chosenText = itemTexts[2];
        }
        if (item == "Rain Boots")
        {
            chosenText = itemTexts[3];
        }
        if (item == "Pine Tree")
        {
            chosenText = itemTexts[4];
        }
        if (item == "Catsup")
        {
            chosenText = itemTexts[5];
        }

        //Debug.Log(chosenText);
    }

    private void ChooseCost(string item)
    {
        if (item == "Steel Rod")
        {
            chosenCost = itemCosts[0];
        }
        if (item == "Ultralight Rod")
        {
            chosenCost = itemCosts[1];
        }
        if (item == "Fling Rod")
        {
            chosenCost = itemCosts[2];
        }
        if (item == "Rain Boots")
        {
            chosenCost = itemCosts[3];
        }
        if (item == "Pine Tree")
        {
            chosenCost = itemCosts[4];
        }
        if (item == "Catsup")
        {
            chosenCost = itemCosts[5];
        }
    }

    public void Beer()
    {
        if (beerCost <= fishBucks)
        {
            fishBucks -= beerCost;
            //pm.hp = pm.maxHp;
            pm.drinksLeft++;
            beerCost += 2;
        }
        else
        {
            Debug.Log("Too poor");
        }
    }

}
