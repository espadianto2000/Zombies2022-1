using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public GameManager gm;
    public int enemyAmount;
    public float timerMaxOleadas = 60f;
    public GameObject player;
    public bool iniciar=false;
    public int contadorWaves=0;
    public GameObject textoWaves;
    public bool desvanecer = false;
    public bool contar = false;
    public float timerActual;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (iniciar)
        {
            if (desvanecer)
            {
                textoWaves.GetComponent<Text>().color = new Color(255f, 255f, 255f, textoWaves.GetComponent<Text>().color.a - (Time.deltaTime/2));
                if (textoWaves.GetComponent<Text>().color.a <= 0)
                {
                    for(int i = 0; i < enemyAmount; i++)
                    {
                        float limiteX = 10;
                        float limiteXNegativo = -limiteX;
                        float limiteZ = 10;
                        float limiteZNegativo = -limiteZ;
                        float posx = Random.Range(limiteXNegativo, limiteX);
                        float posz = Random.Range(limiteZNegativo, limiteZ);
                        while (Vector3.Distance(player.transform.position,new Vector3(posx, player.transform.position.y, posz)) < 5)
                        {
                            posx = Random.Range(limiteXNegativo, limiteX);
                            posz = Random.Range(limiteZNegativo, limiteZ);
                        }
                        Debug.Log("instanciar un enemigo en coordenadas posx y pos z");
                    }
                    desvanecer = false;
                    contar = true;
                    timerActual = timerMaxOleadas;
                }
            }
            if (contar)
            {
                timerActual -= Time.deltaTime;
                if (timerActual <= 0)
                {
                    timerMaxOleadas -= 5;
                    timerMaxOleadas = timerMaxOleadas < 10 ? 10 : timerMaxOleadas;
                    Invoke("InstanciarWave", 3f);
                    contar = false;
                }
            }
        }
    }
    public void iniciarWaves()
    {
        enemyAmount = int.Parse(gm.menuInicio.transform.GetChild(0).GetComponent<InputField>().text != "" ? gm.menuInicio.transform.GetChild(0).GetComponent<InputField>().text : "0");
        gm.quitarPausa();
        iniciar = true;
    }
    public void InstanciarWave()
    {
        contadorWaves++;
        textoWaves.GetComponent<Text>().text = "oleada " + contadorWaves;
        textoWaves.GetComponent<Text>().color = new Color(255f,255f,255f,1f);
        desvanecer = true;
    }
}
