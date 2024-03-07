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
    public float moveSpeed;
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
    public ParticleSystem bloodSpray;

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
        air,
        menu,
        drinking,
        stunned
    }

    public bool dashing;
    public bool drinking;
    [SerializeField] private int drinksLeft = 3;
    internal float drinkingCd;
    [SerializeField] private float timeBetweenDrinks = 3f;
    public bool iFrames = false;
    public float maxHp = 3f;
    public float hp;
    public Image playerHpBar;
    //Input Actions
    public DefaultInputActions playerInput;
    private InputAction move;
    private InputAction drink;

    //Player animations
    private Animator animator;
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Run");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Dash = Animator.StringToHash("Roll");
    private static readonly int Drink = Animator.StringToHash("Drinking");
    private static readonly int LeftStrafe = Animator.StringToHash("LeftStrafe");
    private static readonly int RightStrafe = Animator.StringToHash("RightStrafe");
    private static readonly int Hit = Animator.StringToHash("Hit");
    [SerializeField] private float _dashAnimTime = 0.5f;
    [SerializeField] private float _attackAnimTime = 1f;
    [SerializeField] private float _drinkingAnimTime = 2f;
    [SerializeField] private float _hitStunTime = 1f;
    private float _lockedTill;

    [SerializeField] private GameObject shop;

    private void Start()
    {
        attack = GetComponent<FishingRodAttack>();
        rb = GetComponent<Rigidbody>();
        animator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
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

        if(drinkingCd > 0) drinkingCd -= Time.deltaTime;

        // handle drag
        if (state == MovementState.walking)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if (hp == 0)
        {
            SceneManager.LoadScene("Loss Scene");
        }
        //animator stuff
        var animState = GetState();


        if (animState == _currentState) return;
        animator.CrossFade(animState, .25f, 0);
        _currentState = animState;
        
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
        //Mode - Hit
        if(iFrames && !dashing){
            state = MovementState.stunned;
            desiredMoveSpeed = 0;
        }
        // Mode - Dashing
        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
            playerMaterial.color = Color.green;
        }

        //Mode = Attack
        else if(attack.freezePlayer){
            state = MovementState.attacking;
            desiredMoveSpeed = 0;
        }
        //Mode = drink/heal
        else if(drinking){
            state = MovementState.drinking;
            desiredMoveSpeed = walkSpeed * .2f;
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

        //Mode - Menu/Shop
        if (shop.activeInHierarchy)
        {
            state = MovementState.menu;
            desiredMoveSpeed = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            //prevent player model turning
            //freeze cinemachine camera
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (!iFrames){
            Physics.IgnoreLayerCollision (3, 9, false);
        }
        else{
            Physics.IgnoreLayerCollision (3, 9, true);
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
    private int _currentState;
    private int GetState() {
        if (Time.time < _lockedTill) return _currentState;

        // Priorities
        if(iFrames && !dashing) return LockState(Hit, _hitStunTime);
        if (state == MovementState.dashing) return LockState(Dash, _dashAnimTime);
        if (attack.freezePlayer) return LockState(Attack, _attackAnimTime);
        if(drinking) return LockState(Drink, _drinkingAnimTime);
        if(move.ReadValue<Vector2>() == Vector2.zero){
            return Idle;
        }else{
            if(lockRotation){
                if(move.ReadValue<Vector2>().x < 0) return LeftStrafe;
                else if(move.ReadValue<Vector2>().x > 0) return RightStrafe;
                else return Walk;
            }else{
                return Walk;
            }
        }

        int LockState(int s, float t) {
            _lockedTill = Time.time + t;
            return s;
        }
    }
    private void HaveADrink(InputAction.CallbackContext callbackContext){
        if(!drinking && drinksLeft > 0 && drinkingCd <= 0 && state != MovementState.dashing && state != MovementState.attacking){
            drinksLeft--;
            drinking = true;
            drinkingCd = timeBetweenDrinks;
            Invoke(nameof(ResetDrink), _drinkingAnimTime);
        }
    }
    private void ResetDrink(){
        drinking = false;
        hp++;
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
    void OnCollisionEnter(Collision other){
        if((other.collider.CompareTag("EnemyAttack") || other.collider.CompareTag("Enemy"))){
            if(!iFrames){
                hp--;
                iFrames = true;
                Invoke(nameof(ResetIframes), _hitStunTime - .01f);
            } 
        }
    }
    void ResetIframes(){
        iFrames = false;
    }
    public void SprayBloodOnHit(Vector3 bobberPos, Vector3 bossPos){
        bloodSpray.transform.position = bobberPos;
        Vector3 dirVec = (bobberPos - bossPos).normalized;
        bloodSpray.transform.LookAt(dirVec, Vector3.forward);
        bloodSpray.Play();
    }
    private void OnEnable() {
        
        move = playerInput.Player.Move;

        move.Enable();

        drink = playerInput.Player.Drink;
        drink.Enable();
        drink.performed += HaveADrink;
        
    }
    private void OnDisable() {


        move.Disable();
        drink.Disable();

    }


}