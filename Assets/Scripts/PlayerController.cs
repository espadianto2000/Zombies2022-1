
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float jumpForce = 3f;
    public float fireRange = 15f;
    public float rotationXSensitivity;
    public float rotationYSensitivity;
    public GameManager gm;
    
    private float timeDelay;
    private float tiempoRecarga;
    // public GameObject impacto;

    //Manejo Municion
    public GameObject municion;

    public Slider recarga; 


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
    private InputAction mHoldFire;
    private Rigidbody mRigidbody;
    private Transform mFirePoint;
    private Transform mCameraTransform;
    
    private float mRotationX;
    private float mRotationY;
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

        //mInputAction.Player.Fire.performed += DoFire;
        //mInputAction.Player.Fire.Enable();


        mViewAction = mInputAction.Player.View;
        mInputAction.Player.View.Enable();

        mMovementAction = mInputAction.Player.Movement;
        mMovementAction.Enable();

        mHoldFire = mInputAction.Player.HoldFire;
        mHoldFire.Enable();

        mInputAction.Player.Recarga.performed += Recargar;
        mInputAction.Player.Recarga.Enable();

        mInputAction.Player.Pause.performed += gm.manejoPausa;
        mInputAction.Player.Pause.Enable();
    }
    private void Recargar(InputAction.CallbackContext obj)
    {
        if(municion.transform.GetChild(0).GetComponent<Text>().text!=30.ToString())
            {
            recarga.gameObject.SetActive(true);
            Debug.Log("recargar");
        }
        
    }


  
    /*private void DoFire(InputAction.CallbackContext obj)
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
    }*/

    
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

        // audio
        if (source != null)
        {

            if (source.transform.IsChildOf(transform))
            {
                source.Play();
            }
            else
            {
                AudioSource newAS = Instantiate(source);
                if ((newAS = Instantiate(source)) != null && newAS.outputAudioMixerGroup != null && newAS.outputAudioMixerGroup.audioMixer != null)
                {
                    newAS.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", UnityEngine.Random.Range(audioPitch.x, audioPitch.y));
                    newAS.pitch = UnityEngine.Random.Range(audioPitch.x, audioPitch.y);

                    newAS.PlayOneShot(GunShotClip);

                    Destroy(newAS.gameObject, 4);
                }
            }
        }
        //Raycast
        RaycastHit hit;

        if (Physics.Raycast(
            mFirePoint.position,
            mCameraTransform.forward,
            out hit,
            fireRange
        ))
        {
            // Hubo una colision
            Debug.Log(hit.collider.transform.tag);
            if(hit.collider.tag == "Enemy")
            {
                //Debug.Log("colision enemigo");
                hit.collider.GetComponentInParent<EnemyController>().vida-=10;
                
            }
         /*   GameObject nuevoImpacto =
                Instantiate(impacto, hit.point, transform.rotation);
            Destroy(nuevoImpacto, 1f);*/
        }

        Debug.DrawRay(mFirePoint.position,
            transform.forward * fireRange,
            Color.red,
            .25f
        );

        municion.transform.GetChild(0).GetComponent<Text>().text = (int.Parse(municion.transform.GetChild(0).GetComponent<Text>().text) - 1).ToString();

    }

    private void OnDisable()
    {
        // Codigo que se ejecutara al deshabilitar un GO
        mInputAction.Player.Jump.Disable();
        mMovementAction.Disable();
        mInputAction.Disable();
        mInputAction.Player.Recarga.Disable();
        //mHoldFire.Disable();
        //mInputAction.Player.View.Disable();
    }

    private void Update()
    {
        int munActual = int.Parse(municion.transform.GetChild(0).GetComponent<Text>().text);
        #region Rotacion
        Vector2 deltaPos = mViewAction.ReadValue<Vector2>();
        transform.Rotate(
            Vector3.up * deltaPos.x * Time.deltaTime * rotationYSensitivity
        );
        if (!gm.pausaEstado)
        {
            mRotationX -= deltaPos.y * rotationXSensitivity;
            mRotationX = mRotationX > 90 ? 90 : mRotationX;
            mRotationX = mRotationX < -90 ? -90 : mRotationX;
            mCameraTransform.localRotation = Quaternion.Euler(
                Mathf.Clamp(mRotationX, -90f, 90f),
                0f,
                0f
            );
        }
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

        rotationXSensitivity = (0.49f * gm.menuPausa.transform.GetChild(6).GetComponent<Slider>().value) + 0.01f;
        rotationYSensitivity = (19f * gm.menuPausa.transform.GetChild(5).GetComponent<Slider>().value) + 1f;

        //Disparo
        timeDelay += Time.deltaTime;
        if (timeDelay>=0.1f && mHoldFire.phase == InputActionPhase.Performed && munActual>0 && !recarga.IsActive())
        {
            timeDelay = 0;
            FireWeapon();
            
        }
        //Debug.Log(munActual);
        if(recarga.IsActive())
        {
            
            recarga.value += 0.02f;
        }
        if(recarga.value>=1)
        {
            recarga.gameObject.SetActive(false);
            recarga.value = 0f;
            municion.transform.GetChild(0).GetComponent<Text>().text = 30.ToString();
        }
    }
  

    private void DoJump(InputAction.CallbackContext obj)
    {
        jumpPressed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("piso"))
        {
            Debug.Log("collision");
            onGround = true;
            jumpPressed = false;
        }
        else if (collision.transform.CompareTag("Enemy"))
        {
            //recibir da�o
        }
    }

}
