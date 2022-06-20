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
    private GameObject enemigoGrande;
    private GameObject enemigoPequeno;
    public Text ContadorEnemigosSmall;
    public Text ContadorEnemigosBig;
    // Start is called before the first frame update
    void Start()
    {
        enemigoGrande = Resources.Load<EnemySO>("EnemyBig").prefab;
        enemigoPequeno = Resources.Load<EnemySO>("EnemySmall").prefab;
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
                        float limiteX = 10f;
                        float limiteXNegativo = -14.5f;
                        float limiteZ = 39f;
                        float limiteZNegativo = -8f;
                        float posx = Random.Range(limiteXNegativo, limiteX);
                        float posz = Random.Range(limiteZNegativo, limiteZ);
                        while (Vector3.Distance(player.transform.position,new Vector3(posx, player.transform.position.y, posz)) < 5)
                        {
                            posx = Random.Range(limiteXNegativo, limiteX);
                            posz = Random.Range(limiteZNegativo, limiteZ);
                        }
                        int Index = Random.Range(0, 10);
                        GameObject obj;
                        if (Index <= 2)
                        {
                            obj = Instantiate(enemigoGrande, new Vector3(posx, 0.63f, posz), Quaternion.identity);
                        }
                        else
                        {
                            obj  = Instantiate(enemigoPequeno, new Vector3(posx, 0.63f, posz), Quaternion.identity);
                        }
                        obj.GetComponent<EnemyController>().followAt = player.transform;
                        //Debug.Log("instanciar un enemigo en coordenadas posx y pos z");
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
                    Invoke("InstanciarWave", 2f);
                    contar = false;
                    contadorWaves++;
                    textoWaves.GetComponent<Text>().text = "oleada " + contadorWaves;
                    textoWaves.SetActive(true);
                    textoWaves.GetComponent<Text>().color = new Color(255f, 255f, 255f, 1f);
                }
                if(GameObject.FindGameObjectsWithTag("EnemySmall").Length <= 0 && GameObject.FindGameObjectsWithTag("EnemyBig").Length <= 0)
                {
                    timerActual = -1;
                }
            }
            ContadorEnemigosSmall.text = GameObject.FindGameObjectsWithTag("EnemySmall").Length + "";
            ContadorEnemigosBig.text = GameObject.FindGameObjectsWithTag("EnemyBig").Length + "";
        }
    }
    public void iniciarWaves()
    {
        enemyAmount = int.Parse(gm.menuInicio.transform.GetChild(0).GetComponent<InputField>().text != "" ? gm.menuInicio.transform.GetChild(0).GetComponent<InputField>().text : "0");
        gm.quitarPausa();
        iniciar = true;
        contadorWaves++;
        textoWaves.GetComponent<Text>().text = "oleada " + contadorWaves;
        textoWaves.SetActive(true);
        textoWaves.GetComponent<Text>().color = new Color(255f, 255f, 255f, 1f);
        InstanciarWave();
    }
    public void InstanciarWave()
    {
        desvanecer = true;
    }
    public void cambiarNumEnemigos()
    {
        enemyAmount = int.Parse(gm.menuPausa.transform.GetChild(2).GetComponent<InputField>().text != "" ? gm.menuPausa.transform.GetChild(2).GetComponent<InputField>().text : "0");
    }
}
