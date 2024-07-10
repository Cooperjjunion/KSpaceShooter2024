using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Button startButton;
    private void Start()
    {
       // startButton.onClick.AddListener(() => GameStart());
        startButton.onClick.AddListener(delegate ()
        {
            GameStart();
        });
    }
    public void GameStart()
    {
        SceneManager.LoadScene("Level01");
        SceneManager.LoadScene("Logic",LoadSceneMode.Additive);
    }
}
