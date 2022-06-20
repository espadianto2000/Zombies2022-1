
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public Transform followAt;
    public float distanceToFollow;
    public EnemySO data;
    public float vida;
    public float vidaMax;
    public Slider barraVida;

    private NavMeshAgent mNavMeshAgent;
    private Animator mAnimator;

    private void Awake()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mAnimator = transform.Find("Mutant").GetComponent<Animator>();
    }

    private void Start()
    {
        mNavMeshAgent.speed = data.speed;
        vida = data.health;
        vidaMax = data.health;
    }

    private void Update()
    {

        barraVida.value = vida / vidaMax;
        //mNavMeshAgent.destination = followAt.position;
        float distance = Vector3.Distance(transform.position, followAt.position);
        if (distance <= distanceToFollow)
        {
            mNavMeshAgent.isStopped = false;
            mNavMeshAgent.destination = followAt.position;
            mAnimator.SetTrigger("Walk");
        }
        else
        {
            mAnimator.SetTrigger("Stop");
            mNavMeshAgent.isStopped = true;
        }

        if(vida<=0)
        {
            Destroy(transform.gameObject);
        }
    }
}
