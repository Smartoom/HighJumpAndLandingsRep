using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float airMovementAccelerationMultiplier;//should be less than 1. for player to have less control in air
    [SerializeField] private float maxSpeed;

    [SerializeField] private float groundKineticFrictionAcceleration;//may work on this later. used to stop the player when not pressing movement.
    [SerializeField] private float airKineticFrictionAcceleration;//may work on this later. used to stop the player when not pressing movement.

    [Header("Jumping")]
    [SerializeField] private float intervalBetweenJumps;
    [SerializeField] private float jumpVerticalVelocity;

    [Header("Input")]
    private float horInput, verInput;
    private float timeSinceJumpPressed;

    [Header("Additional Aids")]
    [SerializeField] private float jumpPressBuffer;
    private float timeSinceLastJumped;

    [Header("Detection")]
    private bool groundDetected;
    [SerializeField] private float groundDetectionRadius;
    [SerializeField] private Transform groundDetectionPosition;
    [SerializeField] private LayerMask groundMask;
    //[SerializeField] private float slopeDetectionDistance;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform Orientation;

    private void Update()
    {
        timeSinceJumpPressed += Time.deltaTime;
        timeSinceLastJumped += Time.deltaTime;

        horInput = Input.GetAxisRaw("Horizontal");
        verInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
            timeSinceJumpPressed = 0;

        if (timeSinceJumpPressed <= jumpPressBuffer && timeSinceLastJumped >= intervalBetweenJumps && groundDetected)//dont forget to stop player from spamming jump
        {
            Jump();
        }
    }
    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVerticalVelocity, rb.linearVelocity.z);
        timeSinceLastJumped = 0;
    }

    private void FixedUpdate()
    {
        groundDetected = Physics.CheckSphere(groundDetectionPosition.position, groundDetectionRadius, groundMask);

        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0;

        Vector3 movementDirection = Orientation.forward * verInput + Orientation.right * horInput;

        if (horizontalVelocity.sqrMagnitude < maxSpeed * maxSpeed)//IF under the max speed, accelerate.
        {
            if (Grounded())
                rb.linearVelocity += Time.fixedDeltaTime * movementAcceleration * movementDirection.normalized;
            else
                rb.linearVelocity += Time.fixedDeltaTime * movementAcceleration * airMovementAccelerationMultiplier * movementDirection.normalized;
        }

        if (horizontalVelocity.sqrMagnitude > 0)//keeping it simple. but idk how to do it in another way
        {
            Vector3 originalVelocity = rb.linearVelocity;
            if (Grounded())
                rb.linearVelocity += Time.fixedDeltaTime * groundKineticFrictionAcceleration * -horizontalVelocity.normalized;
            else
                rb.linearVelocity += Time.fixedDeltaTime * airKineticFrictionAcceleration * -horizontalVelocity.normalized;

            if (Mathf.Sign(rb.linearVelocity.x) != Mathf.Sign(originalVelocity.x))
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, rb.linearVelocity.z);
            }
            if (Mathf.Sign(rb.linearVelocity.z) != Mathf.Sign(originalVelocity.z))
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, 0);
            }
        }
    }
    private bool Grounded() => groundDetected && rb.linearVelocity.y < 0.1f;
    private void OnDrawGizmos()
    {
        if (groundDetected)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundDetectionPosition.position, groundDetectionRadius);


        /*        Vector3 horizontalVelocity = rb.linearVelocity;
                horizontalVelocity.y = 0;

                if (horizontalVelocity.sqrMagnitude < maxSpeed * maxSpeed)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.red;

                Gizmos.DrawWireSphere(Vector3.up * 2, horizontalVelocity.magnitude);*/
    }
}
