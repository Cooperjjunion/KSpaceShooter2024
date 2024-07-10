using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour, IDamageable
{
    public enum State { IDLE, TRACE, ATTACK, DIE };

    [SerializeField] private State state;
    // 공격 사정거리
    [SerializeField] private float attackDist = 2.0f;
    // 추적 사정거리
    [SerializeField] private float traceDist = 10.0f;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anim;

    private WaitForSeconds ws;

    public bool isDie = false;

    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashDie = Animator.StringToHash("Die");

    // Monster Health
    private int hp = 100;

   
    void OnEnable()
    {
        // 이벤트 연결, 수신
        PlayerController.OnPlayerDie += this.YouWin;
    }

    void OnDisable()
    {
        // 이벤트 수신 해지
        PlayerController.OnPlayerDie -= this.YouWin;
    }


    void Start()
    {
        ws = new WaitForSeconds(0.3f);

        monsterTr = GetComponent<Transform>(); // monsterTr = transform;
        playerTr = GameObject.FindGameObjectWithTag("PLAYER")?.GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (playerTr == null)
        {
            Debug.LogError("플레이어가 없음");
        }

        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;

                case State.TRACE:
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    anim.SetBool(hashAttack, false);
                    anim.SetBool(hashTrace, true);
                    break;

                case State.ATTACK:
                    anim.SetBool(hashAttack, true);
                    break;

                case State.DIE:
                    anim.SetTrigger(hashDie);
                    agent.isStopped = true;
                    GetComponent<CapsuleCollider>().enabled = false;
                    GameManager.Instance.Score = 50;
                    isDie = true;
                    break;
            }

            yield return ws;
        }
    }

    IEnumerator CheckMonsterState()
    {
        while (isDie == false)
        {
            if (state == State.DIE) yield break;

            //float dist = Vector3.Distance(monsterTr.position, playerTr.position);
            float dist = (monsterTr.position - playerTr.position).sqrMagnitude;

            state = State.IDLE;

            // 추적 사정거리 이내일 경우
            if (dist <= traceDist * traceDist)
            {
                state = State.TRACE;
            }

            // 공격 사정거리 이내일 경우
            if (dist <= attackDist * attackDist)
            {
                state = State.ATTACK;
            }

            yield return ws;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            Destroy(coll.gameObject);
            
        }
    }

    public void YouWin()
    {
        anim.SetFloat("DanceSpeed", UnityEngine.Random.Range(0.8f, 1.5f));
        anim.SetTrigger("PlayerDie");
        StopAllCoroutines();
        agent.isStopped = true;
    }

    public void Damaged()
    {
        anim.SetTrigger(hashHit);

        hp -= 20; // hp = hp - 10;
                  // 몬스터 사망
        if (hp <= 0)
        {
            state = State.DIE;
        }
    }
}
