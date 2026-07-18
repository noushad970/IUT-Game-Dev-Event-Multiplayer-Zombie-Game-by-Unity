using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementMulti : MonoBehaviourPun
{
    [Header("Joysticks")]
    private FixedJoystick movementJoystick;
    private FixedJoystick shootJoystick;
    private Button jumpButton;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 15f;

    private Rigidbody rb;
    private Animator anim;

    private Vector3 movement;
    private Vector3 lookDirection;

    [Header("Jump")]
    public float jumpForce = 6f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;

    private bool isGrounded;
    private bool isJumping;
    private PlayerWeaponMulti playerWeapon;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerWeapon = GetComponent<PlayerWeaponMulti>();

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        if (!photonView.IsMine)
            return;

        FindMovementJoystick();
        FindShootJoystick();
        FindJumpButton();
    }

    //====================================================
    // FIND REFERENCES
    //====================================================

    private void FindMovementJoystick()
    {
        MovementJoystickRef movementRef =
            FindFirstObjectByType<MovementJoystickRef>();

        if (movementRef != null)
        {
            movementJoystick =
                movementRef.GetComponent<FixedJoystick>();

            Debug.Log("Movement Joystick Initialized.");
        }
        else
        {
            Debug.LogWarning("MovementJoystickRef not found.");
        }
    }

    private void FindShootJoystick()
    {
        ShotJoystickRef shootRef =
            FindFirstObjectByType<ShotJoystickRef>();

        if (shootRef != null)
        {
            shootJoystick =
                shootRef.GetComponent<FixedJoystick>();

            Debug.Log("Shoot Joystick Initialized.");
        }
        else
        {
            Debug.LogWarning("ShotJoystickRef not found.");
        }
    }

    private void FindJumpButton()
    {
        JumpRef jumpRef = FindFirstObjectByType<JumpRef>();

        if (jumpRef == null)
        {
            Debug.LogWarning("JumpRef not found.");
            return;
        }

        jumpButton = jumpRef.GetComponent<Button>();

        if (jumpButton != null)
            jumpButton.onClick.AddListener(Jump);
    }

    //====================================================
    // UPDATE
    //====================================================

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        if (movementJoystick == null)
            return;

        CheckGround();
        MovePlayer();
    }

    //====================================================
    // GROUND
    //====================================================

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer);

        if (isGrounded)
            isJumping = false;
    }

    //====================================================
    // JUMP
    //====================================================

    public void Jump()
    {
        if (!isGrounded || isJumping)
            return;

        isJumping = true;

        if (anim != null)
            anim.SetTrigger("Jump");

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0,
            rb.linearVelocity.z);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        Debug.Log("Player Jumped");
    }

    //====================================================
    // MOVEMENT
    //====================================================

    private void MovePlayer()
    {
        //-----------------------------------
        // Keyboard Input
        //-----------------------------------

        Vector2 keyboardInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            float h = 0;
            float v = 0;

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                h--;

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                h++;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                v++;

            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                v--;

            keyboardInput = new Vector2(h, v);
        }

        //-----------------------------------
        // Movement
        //-----------------------------------

        Vector3 joystickMove =
            new Vector3(
                movementJoystick.Horizontal,
                0,
                movementJoystick.Vertical);

        Vector3 keyboardMove =
            new Vector3(
                keyboardInput.x,
                0,
                keyboardInput.y);

        movement = joystickMove + keyboardMove;

        if (movement.magnitude > 1f)
            movement.Normalize();

        //-----------------------------------
        // Animation
        //-----------------------------------

        if (anim != null)
            anim.SetBool("Running", movement.sqrMagnitude > 0.01f);

        //-----------------------------------
        // Move Player
        //-----------------------------------

        Vector3 newPos =
            rb.position +
            movement *
            moveSpeed *
            Time.fixedDeltaTime;

        rb.MovePosition(newPos);

        //-----------------------------------
        // Aim Joystick
        //-----------------------------------

        Vector3 lookDirection = Vector3.zero;

        bool aiming = false;

        if (shootJoystick != null)
        {
            lookDirection = new Vector3(
                shootJoystick.Horizontal,
                0,
                shootJoystick.Vertical);

            aiming = lookDirection.magnitude > 0.2f;
        }

        //-----------------------------------
        // Rotation
        //-----------------------------------

        if (aiming)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(lookDirection.normalized);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime));
        }
        else if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(movement);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime));
        }

        //-----------------------------------
        // AUTO FIRE USING AIM JOYSTICK
        //-----------------------------------

        if (playerWeapon != null)
        {
            if (aiming)
            {
                playerWeapon.StartFire();
            }
            else
            {
                playerWeapon.StopFire();
            }
        }
    }

    //====================================================
    // PUBLIC
    //====================================================

    public Vector3 GetShootDirection()
    {
        if (shootJoystick == null)
            return transform.forward;

        Vector3 dir = new Vector3(
            shootJoystick.Horizontal,
            0,
            shootJoystick.Vertical);

        if (dir.sqrMagnitude < 0.05f)
            return transform.forward;

        return dir.normalized;
    }
}