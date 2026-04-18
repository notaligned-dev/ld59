using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    private const string StartingSceneName = "Game Folder/Scenes/Testing/WholeGameSceneTest";

    [SerializeField] private Button _startNewGameButton;
    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _exitButton;

    private void OnEnable()
    {
        _startNewGameButton.onClick.AddListener(HandleStart);
        _exitButton.onClick.AddListener(HandleExit);
    }

    private void OnDisable()
    {
        _startNewGameButton.onClick.RemoveListener(HandleStart);
        _exitButton.onClick.RemoveListener(HandleExit);
    }

    private void OnValidate()
    {
        UtilitiesDD.RequireNotNull(
            (_startNewGameButton, nameof(_startNewGameButton)),
            (_continueGameButton, nameof(_continueGameButton)),
            (_exitButton, nameof(_exitButton))
        );
    }

    private void HandleStart()
    {
        SceneManager.LoadScene(StartingSceneName);
    }

    private void HandleExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
