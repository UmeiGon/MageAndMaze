using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSceneManager : MonoBehaviour {

    [SerializeField] SceneTransitionManager sceneTransitionMn;
    [SerializeField] EventSceneDialogManager eventSceneDialogMn;
    [SerializeField] string eventFileName;
    [SerializeField] string nextSceneName;

	// Use this for initialization
	void Start () {

        StartCoroutine(sceneTransitionMn.FadeIn());
        eventSceneDialogMn.EventStart(eventFileName);
	}
	
    public IEnumerator ToNextScene()
    {
        yield return StartCoroutine(sceneTransitionMn.FadeOut());
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene("CanvasScene",UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
}
