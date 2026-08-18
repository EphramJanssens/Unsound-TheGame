using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    
    private Rigidbody rb;
    private Vector3 moveInput;
    private Camera mainCamera;

    private InputAction moveAction;
    private InputAction mouseAction;

    void Awake()
    {
        moveAction = new InputAction("Move");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        mouseAction = new InputAction("MousePosition", binding: "<Mouse>/position");
    }

    void OnEnable()
    {
        moveAction.Enable();
        mouseAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        mouseAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        Vector2 input2D = moveAction.ReadValue<Vector2>();
        
        moveInput.x = input2D.x;
        moveInput.z = input2D.y;
        moveInput.Normalize();
        
        LookAtMouse();
    }

    void FixedUpdate()
    {
        Vector3 moveVelocity = moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }

    private void LookAtMouse()
    {
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
        
        Vector2 mousePos = mouseAction.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 pointToLook = ray.GetPoint(rayDistance);
            Vector3 lookDirection = pointToLook - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                rb.MoveRotation(targetRotation);
            }
        }
    }
}