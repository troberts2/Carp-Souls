using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;

    public float dashSpeed;
    public float dashSpeedChangeFactor;

    public float maxYSpeed;

    public float groundDrag;

    public float airMultiplier;




    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;



    public Transform orientation;
    public Transform playerObj;
    public Material playerMaterial;

    float horizontalInput;
    float verticalInput;
    internal bool lockRotation;

    Vector3 moveDirection;

    Rigidbody rb;
    FishingRodAttack attack;

    public MovementState state;
    public enum MovementState
    {
        walking,
        dashing,
        attacking,
        air
    }

    public bool dashing;
    public bool iFrames;
    public float maxHp = 3f;
    private float hp;
    public Image playerHpBar;
    //Input Actions
    public DefaultInputActions playerInput;
    private InputAction move;

    //hi it's Gabriel
    private bool damageBoost;

    private void Start()
    {
        attack = GetComponent<FishingRodAttack>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        hp = maxHp;

    }
    private void Awake() {
        playerInput = new DefaultInputActions();
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        SpeedControl();
        StateHandler();

        // handle drag
        if (state == MovementState.walking)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if (hp == 0)
        {
            SceneManager.LoadScene("Loss Scene");
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }


    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;
    private void StateHandler()
    {
        playerHpBar.fillAmount = hp/maxHp;
        // Mode - Dashing
        if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
            playerMaterial.color = Color.green;
            iFrames = true;
        }

        //Mode = Attack
        else if(attack.freezePlayer){
            state = MovementState.attacking;
            desiredMoveSpeed = 0;
        }


        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            playerMaterial.color = Color.yellow;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            desiredMoveSpeed = walkSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    private void MovePlayer()
    {
        if (state == MovementState.dashing || state == MovementState.attacking) return;

        verticalInput = playerInput.Player.Move.ReadValue<Vector2>().y;
        horizontalInput = playerInput.Player.Move.ReadValue<Vector2>().x;
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded && moveDirection != Vector3.zero)
            rb.MovePosition(transform.position + moveDirection.normalized * moveSpeed * Time.deltaTime);

        // in air
        else if (!grounded && moveDirection != Vector3.zero)
            rb.MovePosition(transform.position + moveDirection.normalized * moveSpeed * Time.deltaTime * airMultiplier);

    }


    private void SpeedControl()
    {

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        // limit y vel
        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
    }
    void OnTriggerEnter(Collider collider){
        if((collider.CompareTag("EnemyAttack") || collider.CompareTag("Enemy"))){
            damageBoost = true; //hi again
            if(!iFrames) StartCoroutine(TakeDamage());
        }
    }
    IEnumerator TakeDamage(){
        //change to boss damage later
        hp--;
        Debug.Log(hp);
        iFrames = true;
        //change later just to show its taking damage
        transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.magenta;
        yield return new WaitForSeconds(0.1f);
        transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.yellow;
        yield return new WaitForSeconds(1f);
        iFrames = false;

        yield return new WaitForSeconds(2.5f);

        damageBoost = false;
    }
    private void OnEnable() {
        
        move = playerInput.Player.Move;

        move.Enable();
        
    }
    private void OnDisable() {


        move.Disable();

    }


}