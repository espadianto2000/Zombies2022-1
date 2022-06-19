using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject menuInicio;
    public GameObject menuPausa;
    public bool pausaEstado = false;
    private float minSensX = 0.01f;
    private float minSensY = 1;
    private float maxSensX = 0.5f;
    private float maxSensY = 20;

    // Start is called before the first frame update
    void Start()
    {
        pausaEstado = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }
    
    // Update is called once per frame
    void Update()
    {
        
        
    }
    public void manejoPausa(InputAction.CallbackContext obj)
    {
        if (pausaEstado && !menuInicio.active)
        {
            quitarPausa();
        }
        else pausa();
    }
    public void pausa()
    {
        menuPausa.SetActive(true);
        pausaEstado = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }
    public void quitarPausa()
    {
        Time.timeScale = 1;
        menuInicio.SetActive(false);
        menuPausa.SetActive(false);
        pausaEstado = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
