using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class QTESystem : MonoBehaviour
{
    private DefaultInputActions playerInput;
    private InputAction fishing;

    public float mashDelay = .5f;
    public GameObject text;
    public GameObject FishingPromt;
    public int mashQuota = 8;
    public int mashCount = 0;

    [SerializeField]
    private TextMeshProUGUI QTEStatus;

    float mash = 5;
    bool pressed = false;
    bool started = false;

    private void Awake()
    {
        playerInput = new DefaultInputActions();
        fishing = playerInput.Fishing.Fish;
    }

    private void OnEnable()
    {
        fishing.Enable();
    }

    private void OnDisable()
    {
        fishing.Disable();    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fishing.IsPressed() && started == false)
        {
            mashCount = 0;
            mash = 5;
            started = true;
            FishingPromt.SetActive(false);
        }

        if (started == true)
        {
            QTEStatus.text = "Reel in your catch by mashing the A button.";
            text.SetActive(true);
            mash -= Time.deltaTime;

            if(fishing.WasPerformedThisFrame())
            {
                mashCount++;
            }
            ///else if (fishing.)
            ///{
            ///    pressed = false;
            ///}
            if (mash <= 0 && mashCount < mashQuota)
            {
                QTEStatus.text = "Fish Fled. Press A to Try Again";
                started = false;
            }
            else if (mash <= 0 && mashCount >= mashQuota)
            {
                QTEStatus.text = "Fish Caught?";
                SceneManager.LoadScene(3);
            }
        }
    }
}
