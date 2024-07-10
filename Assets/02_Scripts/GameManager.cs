using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance = null;
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private List<Transform> points = new List<Transform>();
    [SerializeField] private TMP_Text scoreText;

    private int score;
    public int Score
    {
        set
        {
            score += value;
            scoreText.text = $"Score: <color=#00ff00>{score:00000}</color>";
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject.Find("SpawnPointGroup")?.GetComponentsInChildren<Transform>(points);

        InvokeRepeating(nameof(CreateMonsters), 2.0f, 3.0f);
        StartCoroutine(CreateMonstersCoroutine());
    }

    void CreateMonsters()
    {
        StartCoroutine(CreateMonstersCoroutine());
    }
    IEnumerator CreateMonstersCoroutine()
    {
        yield return new WaitForSeconds(2f);
        int idx = UnityEngine.Random.Range(1, points.Count);
        Instantiate(monsterPrefab, points[idx].position, Quaternion.identity);
    }


    private bool isGameOver = false;
    public bool IsGameOver  
    {
        get { return isGameOver; }
        set {
                isGameOver = value;
            if (isGameOver)
            {
                StopCoroutine(CreateMonstersCoroutine());   
                CancelInvoke(nameof(CreateMonsters));
            }
            }
        

           
       
    }

    private void Awake()
    {
       
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this.gameObject);
           
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerDie += () => isGameOver = true;
    }
}
