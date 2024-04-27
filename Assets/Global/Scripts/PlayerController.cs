using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public Vector2 vecMovement;
    public float speedWalking = 5f;

    [Header("Camera")]
    public Vector2 vecCamera;
    public float speedCamera = 10f;

    [Header("Running")]
    public float speedRunning = 10f;

    [Header("Jumping")]
    public float speedJumping = 4f;

    [Header("Ray Target")]
    public GameObject groundTarget;

    [Header("Animation")]
    public float VelocityX = 0.0f;
    public float VelocityY = 0.0f;

    private float isRunning;
    private float isJumping;
    private float Movement;
    private GameObject playerCamera;
    public Animator animator;

    [HideInInspector]
    public float rotationClamp;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerCamera = GameObject.Find("PlayerCamera");
        animator = animator.GetComponent<Animator>();
    }

    void Update()
    {
        PlayerRayGround();
        PlayerMovement();
        PlayerCamera();
        // PlayerRunning();
        PlayerJumping();
    }

    #region [----Fonction Update----]

    void PlayerRayGround()
    {
        Debug.DrawRay(transform.position, Vector3.down * 0.75f, Color.red);

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.75f) == true)
        {
            groundTarget = hit.transform.gameObject;
        }
        else
        {
            groundTarget = null;
        }
    }

    void PlayerMovement()
    {
        transform.Translate(new Vector3(vecMovement.x, 0f, vecMovement.y) * speedWalking * Time.deltaTime);

        // acceleration Animation

        if (vecMovement.x == 1 &&VelocityX < 0.5f)
        {
            VelocityX += Time.deltaTime * 2.0f;
        }

        if (vecMovement.x == -1 && VelocityX > -0.5f)
        {
            VelocityX -= Time.deltaTime * 2.0f;
        }

        if (vecMovement.y == 1 && VelocityY < 0.5f)
        {
            VelocityY += Time.deltaTime * 2.0f;
        }

        if (vecMovement.y == -1 && VelocityY > -0.5f)
        {
            VelocityY -= Time.deltaTime * 2.0f;
        }

        // deceleration Animation

        if (!(vecMovement.y == 1) && VelocityY > 0.0f)
        {
            VelocityY -= Time.deltaTime * 2.0f;
        }

        if (!(vecMovement.y == -1) && VelocityY < 0.0f)
        {
            VelocityY += Time.deltaTime * 2.0f;
        }

        if (!(vecMovement.y == 1) && !(vecMovement.y == -1) && VelocityY != 0.0f && (VelocityY > -0.05f && VelocityY < 0.05f))
        {
            VelocityY = 0.0f;
        }

        if (!(vecMovement.x == 1) && VelocityX > 0.0f)
        {
            VelocityX -= Time.deltaTime * 2.0f;
        }

        if (!(vecMovement.x == -1) && VelocityX < 0.0f)
        {
            VelocityX += Time.deltaTime * 2.0f;
        }

        if (!(vecMovement.x == 1) && !(vecMovement.x == -1) && VelocityX != 0.0f && (VelocityX > -0.05f && VelocityX < 0.05f))
        {
            VelocityX = 0.0f;
        }

        // diagonale acceleration

/*        if (vecMovement.x == 1 && VelocityX < 0.5f)
        {
            VelocityX += Time.deltaTime * 2.0f;
        }

        if (vecMovement.x == -1 && VelocityX > -0.5f)
        {
            VelocityX -= Time.deltaTime * 2.0f;
        }*/

        if (vecMovement.y == 0.707107f && vecMovement.x == 0.707107f && VelocityY < 0.40406114285f && VelocityX < 0.40406114285f)
        {
            if (VelocityY < 0.5f)
            {
                VelocityX += Time.deltaTime * 2.0f;
            }

            VelocityY += Time.deltaTime * 2.0f;
        }

        if (vecMovement.y == 0.707107f && vecMovement.x == -0.707107f && VelocityY < 0.40406114285f && VelocityX > -0.40406114285f)
        {
            VelocityY += Time.deltaTime * 2.0f;
            VelocityX -= Time.deltaTime * 2.0f;
        }

        if (vecMovement.y == -0.707107f && vecMovement.x == 0.707107f && VelocityY > -0.40406114285f && VelocityX < 0.40406114285f)
        {
            VelocityY += Time.deltaTime * 2.0f;
            VelocityX -= Time.deltaTime * 2.0f;
        }

        if (vecMovement.y == -0.707107f && vecMovement.x == -0.707107f && VelocityY > -0.40406114285f && VelocityX > -0.40406114285f)
        {
            VelocityY -= Time.deltaTime * 2.0f;
            VelocityX -= Time.deltaTime * 2.0f;
        }

        //diagonale deceleration

        if (!(vecMovement.y == 0.707107f) && !(vecMovement.x == 0.707107f) && VelocityY > 0.0f && VelocityX > 0.0f)
        {
            VelocityY -= Time.deltaTime * 2.0f;
            VelocityX -= Time.deltaTime * 2.0f;
        }

        if (!(vecMovement.y == 0.707107f) && !(vecMovement.x == -0.707107f) && VelocityY > 0.0f && VelocityX < 0.0f)
        {
            VelocityY -= Time.deltaTime * 2.0f;
            VelocityX += Time.deltaTime * 2.0f;
        }

        if (!(vecMovement.y == -0.707107f) && !(vecMovement.x == 0.707107f) && VelocityY < 0.0f && VelocityX > 0.0f)
        {
            VelocityY += Time.deltaTime * 2.0f;
            VelocityX -= Time.deltaTime * 2.0f;
        }

        if (!(vecMovement.y == -0.707107f) && !(vecMovement.x == -0.707107f) && VelocityY < 0.0f && VelocityX < 0.0f)
        {
            VelocityY += Time.deltaTime * 2.0f;
            VelocityX += Time.deltaTime * 2.0f;
        }

        if (!(vecMovement.y == 0.707107f) && !(vecMovement.x == 0.707107f) | !(vecMovement.y == -0.707107f) && !(vecMovement.x == 0.707107f) && VelocityY != 0.0f && VelocityX != 0.0f && (VelocityY > -0.05f && VelocityY < 0.05f) && (VelocityX > -0.05f && VelocityX < 0.05f))
        {
            VelocityY = 0.0f;
            VelocityX = 0.0f;
        }

/*        if (!(vecMovement.y == 0.707107f) && !(vecMovement.x == 0.707107f) | !(vecMovement.y == -0.707107f) && !(vecMovement.x == 0.707107f) && VelocityY != 0.0f && (VelocityY > -0.05f && VelocityY < 0.05f))
        {
            VelocityY = 0.0f;
            VelocityX = 0.0f;
        }
*/
        animator.SetFloat("X_Velocity", VelocityX);
        animator.SetFloat("Y_Velocity", VelocityY);
    }

    void PlayerCamera()
    {
        transform.Rotate(new Vector3(0f, vecCamera.x, 0f) * speedCamera * Time.deltaTime);

        playerCamera.transform.Rotate(new Vector3(-vecCamera.y, 0f, 0f) * speedCamera * Time.deltaTime);

        rotationClamp = playerCamera.transform.eulerAngles.x;

        if (rotationClamp > 180)
        {
            rotationClamp -= 360;
        }

        playerCamera.transform.localRotation = Quaternion.Euler(Mathf.Clamp(rotationClamp, -80, 80), 0, 0);
    }
    void PlayerRunning()
    {
/*        if (isRunning == 0)
        {
            Movement = speedWalking;
        }
        else if (isRunning == 1)
        {
            Movement = speedRunning;
        }*/
    }

    void PlayerJumping()
    {
        if (isJumping == 1 && groundTarget != null)
        {
            GetComponent<Rigidbody>().velocity = Vector3.up * speedJumping;
        }
    }

    #endregion

    #region [----Fonction----]

    void OnCamera(InputValue value)
    {
        vecCamera = value.Get<Vector2>();
    }

    void OnMovement(InputValue value)
    {
        vecMovement = value.Get<Vector2>();
    }

    void OnRun(InputValue value)
    {
        isRunning = value.Get<float>();
    }

    void OnJump(InputValue value)
    {
        isJumping = value.Get<float>();
    }

    #endregion  
}