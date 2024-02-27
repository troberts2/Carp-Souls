using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;
    private DefaultInputActions playerInput;
    private InputAction move;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Awake() {
        playerInput = new DefaultInputActions();
        move = playerInput.Player.Move;
    }
    private void OnEnable() {
        move.Enable();
    }
    private void OnDisable() {
        move.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        //rotate player object
        float horizontalInput = move.ReadValue<Vector2>().x;
        float verticalInput = move.ReadValue<Vector2>().y;
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(player.GetComponent<EnemyLockOn>().enemyLocked && FindObjectOfType<PlayerMovement>().state != PlayerMovement.MovementState.dashing && FindObjectOfType<PlayerMovement>().state != PlayerMovement.MovementState.attacking){
            playerObj.forward = Vector3.Slerp(playerObj.forward, orientation.forward, Time.deltaTime * rotationSpeed);
            return;
        }

        if(inputDir != Vector3.zero && FindObjectOfType<PlayerMovement>().state != PlayerMovement.MovementState.dashing && FindObjectOfType<PlayerMovement>().state != PlayerMovement.MovementState.attacking){
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}
