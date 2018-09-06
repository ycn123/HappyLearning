using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class MenuButtons : MonoBehaviour {

	public Button PlayButton;
    public Button TestButton;
    public Button ExitButton;

    public int PlayLevel = 1;
    public int TestLevel = 2;

	


	UnityAction playAction = null;
	UnityAction TestingAction = null;
    UnityAction ExitAction = null;
 
    void Start ()
    {
	
        playAction = () => { SceneManager.LoadScene(1); };
        PlayButton.onClick.AddListener(playAction);

  
                TestingAction = () => { SceneManager.LoadScene(2); };
        TestButton.onClick.AddListener(TestingAction);

        ExitAction = () => { Application.Quit();};
        ExitButton.onClick.AddListener(ExitAction);

    }


}









