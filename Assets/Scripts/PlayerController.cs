
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float jumpForce = 3f;
    public float fireRange = 5f;
    public float rotationXSensitivity;
    public float rotationYSensitivity;
    public GameObject explosion;


    // --- Muzzle ---
    public GameObject muzzlePrefab;
    public GameObject muzzlePosition;

    public GameObject projectilePrefab;
    public GameObject projectileToDisableOnFire;
    // --- Audio ---
    public AudioClip GunShotClip;
    public AudioSource source;
    public Vector2 audioPitch = new Vector2(.9f, 1.1f);

    private PlayerInputAction mInputAction;
    private InputAction mMovementAction;
    private InputAction mViewAction;
    private Rigidbody mRigidbody;
    private Transform mFirePoint;
    private Transform mCameraTransform;
    private float mRotationX;
    private bool jumpPressed = false;
    private bool onGround = true;
    [SerializeField] private float timeLastFired;

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
        FireWeapon();
    }
    
    public void FireWeapon()
    {

        var flash = Instantiate(muzzlePrefab, muzzlePosition.transform);

        // --- Shoot Projectile Object ---
        if (projectilePrefab != null)
        {
            GameObject newProjectile = Instantiate(projectilePrefab, muzzlePosition.transform.position, muzzlePosition.transform.rotation, transform);
        }

        // --- Disable any gameobjects, if needed ---
        if (projectileToDisableOnFire != null)
        {
            projectileToDisableOnFire.SetActive(false);
            Invoke("ReEnableDisabledProjectile", 3);
        }

        // --- Handle Audio ---
        if (source != null)
        {
            // --- Sometimes the source is not attached to the weapon for easy instantiation on quick firing weapons like machineguns, 
            // so that each shot gets its own audio source, but sometimes it's fine to use just 1 source. We don't want to instantiate 
            // the parent gameobject or the program will get stuck in a loop, so we check to see if the source is a child object ---
            if (source.transform.IsChildOf(transform))
            {
                source.Play();
            }
            else
            {
                // --- Instantiate prefab for audio, delete after a few seconds ---
                AudioSource newAS = Instantiate(source);
                if ((newAS = Instantiate(source)) != null && newAS.outputAudioMixerGroup != null && newAS.outputAudioMixerGroup.audioMixer != null)
                {
                    // --- Change pitch to give variation to repeated shots ---
                    newAS.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", UnityEngine.Random.Range(audioPitch.x, audioPitch.y));
                    newAS.pitch = UnityEngine.Random.Range(audioPitch.x, audioPitch.y);

                    // --- Play the gunshot sound ---
                    newAS.PlayOneShot(GunShotClip);

                    // --- Remove after a few seconds. Test script only. When using in project I recommend using an object pool ---
                    Destroy(newAS.gameObject, 4);
                }
            }
        }

        // --- Insert custom code here to shoot projectile or hitscan from weapon ---

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
            Vector3.up * deltaPos.x * Time.deltaTime * rotationYSensitivity
        );
        mRotationX -= deltaPos.y * rotationXSensitivity;
        mRotationX = mRotationX > 90 ? 90 : mRotationX;
        mRotationX = mRotationX < -90 ? -90 : mRotationX;
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
