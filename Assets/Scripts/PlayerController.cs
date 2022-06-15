
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float jumpForce = 3f;
    public float fireRange = 5f;
    public float rotationXSensitivity = 1f;
    public GameObject explosion;

    private PlayerInputAction mInputAction;
    private InputAction mMovementAction;
    private InputAction mViewAction;
    private Rigidbody mRigidbody;
    private Transform mFirePoint;
    private Transform mCameraTransform;
    private float mRotationX = 0f;
    private bool jumpPressed = false;
    private bool onGround = true;


    private void Awake()
    {
        mInputAction = new PlayerInputAction();
        mRigidbody = GetComponent<Rigidbody>();
        mFirePoint = transform.Find("FirePoint");
        mCameraTransform = transform.Find("Main Camera");

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        // Codigo que se ejecutara al habilitar un GO
        mInputAction.Player.Jump.performed += DoJump;
        mInputAction.Player.Jump.Enable();

        mInputAction.Player.Fire.performed += DoFire;
        mInputAction.Player.Fire.Enable();

        mViewAction = mInputAction.Player.View;
        mInputAction.Player.View.Enable();

        mMovementAction = mInputAction.Player.Movement;
        mMovementAction.Enable();

    }


    private void DoFire(InputAction.CallbackContext obj)
    {
        // Lanzar un raycast
        RaycastHit hit;

        if (Physics.Raycast(
            mFirePoint.position,
            mCameraTransform.forward,
            out hit,
            fireRange
        ))
        {
            // Hubo una colision
            Debug.Log(hit.collider.name);
            GameObject nuevaExplosion = 
                Instantiate(explosion, hit.point, transform.rotation);
            Destroy(nuevaExplosion, 1f);
        }

        Debug.DrawRay(mFirePoint.position,
            transform.forward * fireRange,
            Color.red,
            .25f
        );
    }

    private void OnDisable()
    {
        // Codigo que se ejecutara al deshabilitar un GO
        mInputAction.Player.Jump.Disable();
        mMovementAction.Disable();
        mInputAction.Disable();
        //mInputAction.Player.View.Disable();
    }

    private void Update()
    {
        #region Rotacion
        Vector2 deltaPos = mViewAction.ReadValue<Vector2>();
        transform.Rotate(
            Vector3.up * deltaPos.x * Time.deltaTime
        );

        mRotationX -= deltaPos.y * rotationXSensitivity;
        mCameraTransform.localRotation = Quaternion.Euler(
            Mathf.Clamp(mRotationX, -90f, 90f),
            0f,
            0f
        );
        #endregion

        #region Movimiento
        Vector2 movement = Vector2.ClampMagnitude(
            mMovementAction.ReadValue<Vector2>(),
            1f
        );

        mRigidbody.velocity = movement.x * transform.right * moveSpeed +
            movement.y * transform.forward * moveSpeed + 
            transform.up * mRigidbody.velocity.y;
        /*mRigidbody.velocity = new Vector3(
            movement.x * moveSpeed,
            mRigidbody.velocity.y,
            movement.y * moveSpeed
        );*/
        #endregion

        #region Salto
        if (jumpPressed && onGround)
        {
            mRigidbody.velocity += Vector3.up * jumpForce;
            jumpPressed = false;
            onGround = false;
        }
        #endregion
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        jumpPressed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        onGround = true;
        jumpPressed = false;
    }

}
