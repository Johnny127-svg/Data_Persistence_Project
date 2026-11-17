using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUIHandler : MonoBehaviour
{
    public InputField NameInput;
    public Text BestScoreText;

    private void Start()
    {
        NameInput.text = GameData.PlayerName;
        NameInput.onValueChanged.AddListener(UpdateBestScoreText);
        UpdateBestScoreText(NameInput.text);
    }

    private void UpdateBestScoreText(string typedName)
    {
        GameData.PlayerName = typedName;

        string name = typedName.Trim();
        if (name.Length == 0)
        {
            BestScoreText.text = "Enter Name";
        }
        else if (GameData.HasHighScore(name))
        {
            BestScoreText.text = $"Best Score : {name} : {GameData.GetHighScore(name)}";
        }
        else
        {
            BestScoreText.text = $"Best Score : {name} : No high score yet";
        }
    }

    public void StartNew()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }
}
