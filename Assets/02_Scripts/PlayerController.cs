using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    /*
        외부 입력장치 값 받아오기 

        1. InputManager (Legacy)
        2. InputSystem (new)
    */
    [SerializeField]
    [Range(5, 15)]
    private float moveSpeed = 6.0f;

    [SerializeField]
    private float turnSpeed = 200.0f;

    private float v;
    private float h;
    private float r;

    private Animator animator;

    // Animator의 Parameter Hash 값을 추출
    private int hashForward = Animator.StringToHash("forward");
    private int hashStrafe = Animator.StringToHash("strafe");

    private bool isStarted = false;

    public int initHp = 100;
    public int currHp = 100;
    [SerializeField] private Image hpBar;
    // 델리게이트 (Delegate : 대리자)
    public delegate void PlayerDieHandler();

    // 이벤트 
    public static event PlayerDieHandler OnPlayerDie;

    IEnumerator Start()
    {
        animator = this.gameObject.GetComponent<Animator>();

        yield return new WaitForSeconds(0.2f);
        isStarted = true;
    }

    void Update()
    {
        v = Input.GetAxis("Vertical"); // -1.0f ~ 0.0f ~ +1.0f
        h = Input.GetAxis("Horizontal"); // -1.0f ~ 0.0f ~ +1.0f
        r = (isStarted == true) ? Input.GetAxis("Mouse X") : 0.0f;

        MoveAndRotate();
        PlayerAnim();
    }

    private void PlayerAnim()
    {
        animator.SetFloat(hashForward, v);
        animator.SetFloat(hashStrafe, h);
    }

    private void MoveAndRotate()
    {
        // 이동방향을 계산 : 벡터의 덧셈 연산
        // 벡터의 크기를 1로 변환
        // 벡터의 정규화 , Vector Normalized
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        transform.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * Time.deltaTime * turnSpeed * r);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (currHp > 0 && coll.CompareTag("PUNCH"))
        {
            currHp -= 10;
            hpBar.fillAmount = (float)currHp/(float)initHp;
            if (currHp <= 0)
            {
                GameManager.Instance.IsGameOver = true;
                // 이벤트를 발생(호출) Raise
                OnPlayerDie();

                //PlayerDie();
            }
        }
    }

    // private void PlayerDie()
    // {
    //     // 스테이지에 있는 모든 몬스터를 추출
    //     GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
    //     foreach (var monster in monsters)
    //     {
    //         //monster.SendMessage("YouWin", SendMessageOptions.DontRequireReceiver);
    //         monster.GetComponent<MonsterController>().YouWin();
    //     }
    // }
}


/*
    Vector3.forward = new Vector3(0, 0, 1)
    Vector3.up      = new Vector3(0, 1, 0)
    Vector3.right   = new Vector3(1, 0, 0)

    Vector3.one     = new Vector3(1, 1, 1)
    Vector3.zero    = new Vector3(0, 0, 0)
*/