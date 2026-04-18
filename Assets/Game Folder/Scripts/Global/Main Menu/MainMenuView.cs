using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class MainMenuView : MonoBehaviour
{
    private const string StartingSceneName = "Game Folder/Scenes/Testing/WholeGameSceneTest";

    [SerializeField] private Button _startNewGameButton;
    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private ConfirmationDisplay _confirmationDisplay;

    private StoryService _storyService;

    [Inject]
    private void Construct(StoryService storyService)
    {
        _storyService = storyService;

        ReloadMenu();
    }

    private void OnEnable()
    {
        _startNewGameButton.onClick.AddListener(HandleStart);
        _continueGameButton.onClick.AddListener(HandleContinue);
        _exitButton.onClick.AddListener(HandleExit);
    }

    private void OnDisable()
    {
        _startNewGameButton.onClick.RemoveListener(HandleStart);
        _continueGameButton.onClick.RemoveListener(HandleContinue);
        _exitButton.onClick.RemoveListener(HandleExit);
    }

    private void OnValidate()
    {
        UtilitiesDD.RequireNotNull(
            (_startNewGameButton, nameof(_startNewGameButton)),
            (_continueGameButton, nameof(_continueGameButton)),
            (_exitButton, nameof(_exitButton)),
            (_confirmationDisplay, nameof(_confirmationDisplay))
        );
    }

    private void ReloadMenu()
    {
        if (_storyService.CanContinueStory())
            UnlockContinueButton();
        else
            LockContinueButton();
    }

    private void HandleStart()
    {
        if (_storyService.CanContinueStory())
            _confirmationDisplay.Show(StartNewGame);
        else
            StartNewGame();
    }

    private void HandleContinue()
    {
        ContinueGame();
    }

    private void UnlockContinueButton()
    {
        _continueGameButton.gameObject.SetActive(true);
    }

    private void LockContinueButton()
    {
        _continueGameButton.gameObject.SetActive(false);
    }

    private void StartNewGame()
    {
        SceneManager.LoadScene(StartingSceneName);
    }


    private void ContinueGame()
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
