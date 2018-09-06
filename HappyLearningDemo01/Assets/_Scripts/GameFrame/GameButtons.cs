using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class GameButtons : MonoBehaviour {

	public Button PauseButton;
	public Button ResumeButton;
	public Button HomeButton;
    public int HomeLevel = 0;
    public GameObject PausedScreen;

	UnityAction pauseAction = null;
	UnityAction resumeAction = null;
	UnityAction homeAction = null;

	void Start () {

		PausedScreen.SetActive(false);

		pauseAction = () => { OnGamePaused(); };
		PauseButton.onClick.AddListener(pauseAction);


		resumeAction = () => { OnGameResumed(); };
		ResumeButton.onClick.AddListener(resumeAction);


		homeAction = () => { GotoHome(); };
		HomeButton.onClick.AddListener(homeAction);
        		
	}
	

	private void OnGamePaused()
    {
		Time.timeScale = 0;
        PauseButton.image.enabled = false;
        PausedScreen.SetActive(true);
    }

	private void OnGameResumed()
    {
		Time.timeScale = 1;
        PauseButton.image.enabled = true;
        PausedScreen.SetActive(false);
    }



	private void GotoHome()
    {
		SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }


}








