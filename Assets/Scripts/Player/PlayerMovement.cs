using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Joysticks")]
    private FixedJoystick movementJoystick;
    private FixedJoystick shotJoystick;
    private Button jumpButton;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 15f;

    private Rigidbody rb;
    private Vector3 movement;
    private Animator anim;

    [Header("Jump")]
    public float jumpForce = 6f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;

    private bool isGrounded;
    private bool isJumping;
    private PlayerWeapon playerWeapon;

    private bool wasAiming = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        playerWeapon = GetComponent<PlayerWeapon>();
        FindMovementJoystick();
        FindShotJoystick();
        FindJumpButton();
    }

    //---------------------------------------------------
    // Find References
    //---------------------------------------------------

    private void FindMovementJoystick()
    {
        MovementJoystickRef obj =
            FindFirstObjectByType<MovementJoystickRef>();

        if (obj != null)
            movementJoystick = obj.GetComponent<FixedJoystick>();
        else
            Debug.LogWarning("MovementJoystickRef not found.");
    }

    private void FindShotJoystick()
    {
        ShotJoystickRef obj =
            FindFirstObjectByType<ShotJoystickRef>();

        if (obj != null)
            shotJoystick = obj.GetComponent<FixedJoystick>();
        else
            Debug.LogWarning("ShotJoystickRef not found.");
    }

    private void FindJumpButton()
    {
        JumpRef jump =
            FindFirstObjectByType<JumpRef>();

        if (jump == null)
        {
            Debug.LogWarning("JumpRef not found.");
            return;
        }

        jumpButton = jump.GetComponent<Button>();

        if (jumpButton != null)
            jumpButton.onClick.AddListener(Jump);
    }

    //---------------------------------------------------
    // Update
    //---------------------------------------------------

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (movementJoystick == null)
            return;

        CheckGround();
        MovePlayer();
    }

    //---------------------------------------------------
    // Ground
    //---------------------------------------------------

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer);

        if (isGrounded)
            isJumping = false;
    }

    //---------------------------------------------------
    // Jump
    //---------------------------------------------------

    public void Jump()
    {
        if (!isGrounded || isJumping)
            return;

        isJumping = true;

        anim.Play("Jump");

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0,
            rb.linearVelocity.z);

        rb.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse);
    }

    //---------------------------------------------------
    // Movement
    //---------------------------------------------------

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
        // Shot Joystick
        //-----------------------------------

        Vector3 lookDirection = Vector3.zero;

        bool aiming = false;

        if (shotJoystick != null)
        {
            lookDirection = new Vector3(
                shotJoystick.Horizontal,
                0,
                shotJoystick.Vertical);

            aiming = lookDirection.magnitude > 0.2f;
        }

        //-----------------------------------
        // Rotate Player
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
        // Auto Fire Using Aim Joystick
        //-----------------------------------

        if (playerWeapon != null && shotJoystick != null)
        {
            aiming = shotJoystick.Direction.magnitude > 0.2f;

            if (aiming)
            {
                if (!wasAiming)
                {
                    wasAiming = true;
                    playerWeapon.StartFire();
                }
            }
            else
            {
                if (wasAiming)
                {
                    wasAiming = false;
                    playerWeapon.StopFire();
                }
            }
        }
    }

    //---------------------------------------------------
    // Public
    //---------------------------------------------------

    public Vector3 GetShootDirection()
    {
        if (shotJoystick != null)
        {
            Vector3 dir =
                new Vector3(
                    shotJoystick.Horizontal,
                    0,
                    shotJoystick.Vertical);

            if (dir.sqrMagnitude > 0.01f)
                return dir.normalized;
        }

        if (transform.forward.sqrMagnitude > 0.01f)
            return transform.forward;

        return Vector3.forward;
    }
}