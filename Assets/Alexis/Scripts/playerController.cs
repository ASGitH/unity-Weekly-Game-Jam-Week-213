using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    #region Private
    private Animator playerAnimator;

    private bool isActive = true, isGrounded = false, isIdle = false;

    private Camera attachedCamera;

    private float airDrag = 0.25f, groundDrag = 8f;
    private float horizontalMovement = 0f, verticalMovement = 0f;
    private float mouseX = 0f, mouseY = 0f;
    private float xRotation = 0f, yRotation = 0f;

    private Vector2[] positionInt = new Vector2[4];

    private Rigidbody rb;

    private Vector3 moveDirection;
    #endregion

    #region Public
    public BoxCollider resetTrigger;

    public float cameraSensitivityX = 0f, cameraSensitivityY = 0f;
    public float jumpForce = 0f;
    public float movementSpeed;

    public GridContainer referenceToGridContainer;
    #endregion

    void Start()
    {
        attachedCamera = gameObject.GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerAnimator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (isActive)
        {
            animatePlayer();

            controlDrag();

            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f + 0.1f);

            if (isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.Space)) { jump(); }

                referenceToGridContainer.destroyTile(Mathf.RoundToInt(positionInt[0].x), Mathf.RoundToInt(positionInt[0].y));
                referenceToGridContainer.destroyTile(Mathf.RoundToInt(positionInt[1].x), Mathf.RoundToInt(positionInt[1].y));
                referenceToGridContainer.destroyTile(Mathf.RoundToInt(positionInt[2].x), Mathf.RoundToInt(positionInt[2].y));
                referenceToGridContainer.destroyTile(Mathf.RoundToInt(positionInt[3].x), Mathf.RoundToInt(positionInt[3].y));
            }
        }

        getInput();

        updatePositionAndRotation();
    }

    private void FixedUpdate() 
    {
        if (isActive) { gravityPull(); }
        
        movement(); 
    }

    private void OnTriggerEnter(Collider other) { if(isActive && other == resetTrigger) { playerSpectate(); } }

    private void animatePlayer()
    {
        playerAnimator.SetBool("isGrounded", isGrounded);

        if (horizontalMovement != 0f || verticalMovement != 0f) { playerAnimator.SetBool("isIdle", isIdle = false); }
        else { playerAnimator.SetBool("isIdle", isIdle = true); }
    }

    private void controlDrag()
    {
        if (!isGrounded) { rb.drag = airDrag; }
        else { rb.drag = groundDrag; }
    }

    private void jump() { rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); }

    private void getInput() 
    {
        // Camera Rotational Input
        mouseX = Input.GetAxisRaw("Mouse X"); mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * cameraSensitivityX * 0.01f;
        xRotation -= mouseY * cameraSensitivityY * 0.01f;

        xRotation = Mathf.Clamp(xRotation, -75f, 75f);

        // Player Movement Input
        horizontalMovement = Input.GetAxisRaw("Horizontal"); verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = attachedCamera.transform.forward * verticalMovement + attachedCamera.transform.right * horizontalMovement;
    }

    private void gravityPull() { if (!isGrounded) { rb.AddForce(new Vector3(0f, -8f, 0f), ForceMode.Acceleration); } }

    private void movement()
    {
        if (!isActive) { rb.AddForce(new Vector3(moveDirection.normalized.x * movementSpeed * 8f, moveDirection.normalized.y * movementSpeed * 8f, moveDirection.normalized.z * movementSpeed * 8f), ForceMode.Acceleration); }
        else
        {
            if (!isGrounded) { rb.AddForce(new Vector3(moveDirection.normalized.x * movementSpeed * .8f, 0f, moveDirection.normalized.z * movementSpeed * 0.8f), ForceMode.Acceleration); }
            else { rb.AddForce(new Vector3(moveDirection.normalized.x * movementSpeed * 16f, 0f, moveDirection.normalized.z * movementSpeed * 16f), ForceMode.Acceleration); }
        }
    }

    private void playerSpectate()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;

        isActive = false;

        playerAnimator.enabled = false;

        rb.drag = 5f;
        rb.useGravity = false;

        transform.position = new Vector3(15f, 8f, 15f);
    }

    private void updatePositionAndRotation()
    {
        attachedCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        positionInt[0].x = transform.position.x + .32f; positionInt[0].y = transform.position.z + .32f;
        positionInt[1].x = transform.position.x + .32f; positionInt[1].y = transform.position.z - .32f;
        positionInt[2].x = transform.position.x - .32f; positionInt[2].y = transform.position.z + .32f;
        positionInt[3].x = transform.position.x - .32f; positionInt[3].y = transform.position.z - .32f;

        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
