using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;   // ← Added this

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Joystick")]
    private FixedJoystick joystick;
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        FindJoystick();
        FindJumpButton();
    }

    private void FindJumpButton()
    {
        jumpButton = FindFirstObjectByType<JumpRef>().GetComponent<Button>();
        if (jumpButton == null)
            Debug.LogWarning("Jump Button not found in the scene.");
        else
            jumpButton.onClick.AddListener(Jump);
    }

    private void FindJoystick()
    {
        joystick = FindFirstObjectByType<FixedJoystick>();

        if (joystick == null)
            Debug.LogWarning("FixedJoystick not found in the scene.");
    }

    private void Update()
    {
        // Keyboard Jump (Space)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (joystick == null)
            return;

        CheckGround();
        MovePlayer();
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded)
        {
            isJumping = false;
        }
    }

    public void Jump()
    {
        if (!isGrounded || isJumping)
            return;

        isJumping = true;

        // Play animation
        anim.Play("Jump");

        // Clear downward velocity before jumping
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        // Apply jump force
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        Debug.Log("Player Jumped");
    }

    private void MovePlayer()
    {
        // === Keyboard Input (WASD + Arrow Keys) using New Input System ===
        Vector2 keyboardInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;

            keyboardInput = new Vector2(horizontal, vertical);
        }

        // Joystick input
        Vector3 joystickMovement = new Vector3(
            joystick.Horizontal,
            0f,
            joystick.Vertical
        );

        // Combine both inputs
        Vector3 keyboardMovement = new Vector3(keyboardInput.x, 0f, keyboardInput.y);
        movement = joystickMovement + keyboardMovement;

        if (movement.magnitude > 0f)
        {
            anim.SetBool("Running", true);
        }
        else
        {
            anim.SetBool("Running", false);
        }

        if (movement.sqrMagnitude < 0.01f)
            return;

        movement.Normalize();

        Vector3 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);

        Quaternion targetRotation = Quaternion.LookRotation(movement);

        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            )
        );
    }
}