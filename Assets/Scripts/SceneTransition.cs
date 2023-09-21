using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string LoadScene;
    //public int LevelIndex;

    void OnTriggerEnter()
    {
        SceneManager.LoadScene(LoadScene);
    }

}
