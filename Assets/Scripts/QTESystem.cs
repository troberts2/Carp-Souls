using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QTESystem : MonoBehaviour
{
    public float mashDelay = .5f;
    public GameObject text;
    public int mashQuota = 8;
    public int mashCount = 0;

    [SerializeField]
    private TextMeshProUGUI QTEStatus;

    float mash = 5;
    bool pressed = false;
    bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            mashCount = 0;
            mash = 5;
            started = true;
        }

        if (started == true)
        {
            QTEStatus.text = "Reel in you catch by mashing the A button";
            text.SetActive(true);
            mash -= Time.deltaTime;

            if(Input.GetKeyDown(KeyCode.F) && pressed == false)
            {
                pressed = true;
                mashCount++;
            }
            else if (Input.GetKeyUp(KeyCode.F))
            {
                pressed = false;
            }
            if (mash <= 0 && mashCount < mashQuota)
            {
                QTEStatus.text = "Fish Fled";
            }
            else if (mash <= 0 && mashCount >= mashQuota)
            {
                QTEStatus.text = "Fish Caught";
            }
        }
    }
}
