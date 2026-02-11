using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int levelNumber;
    public void OpenLevel(string LevelName)
    {
        SceneManager.LoadScene(LevelName); 
    }

    public void OpenNextLevel()
    {
        int nextLevelNumber = levelNumber + 1;
        string name = "Level" + nextLevelNumber;

        SceneManager.LoadScene(name);
    }
}
