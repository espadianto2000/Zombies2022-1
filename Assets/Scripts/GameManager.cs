using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject menuInicio;
    public GameObject menuPausa;
    public bool pausaEstado = false;

    // Start is called before the first frame update
    void Start()
    {
        pausaEstado = true;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void pausa()
    {
        menuPausa.SetActive(true);
    }
    public void quitarPausa()
    {
        menuInicio.SetActive(false);
        menuPausa.SetActive(false);
    }
}
