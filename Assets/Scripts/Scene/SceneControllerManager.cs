using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : SingletonMonobehavior<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    public SceneName startingSceneName;


    private IEnumerator Fade(float finalAlpha)
    {
        //set the fading flag to true so the FadeAndSwitchScenes coroutine won't be called again
        isFading = true;

        //make sure the canvasGroup blocks raycasts into thbe scene so no more input canbe accepted
        faderCanvasGroup.blocksRaycasts = true;

        //calculate how fast the CanvasGroup should fade based on it's alpha, it's final alpha and how long it has to change between the two 
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        //while the canvas group hasn't reached the alpha yet...
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            //...move the alpha towards it's target alpha
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            //wait for a frame then continue
            yield return null;
        }
        // set the flag to false since the fadd has finished 
        isFading = false;

        //stops the CanvasGroup from blocking raycasts so input is no longer ignored
        faderCanvasGroup.blocksRaycasts = false;
    }


    //this is a coroutine where the bulding blocks of the script are put together
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        //call before scene unload fade out event
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        //start fading to nlack and wait for it to finish before continuing
        yield return StartCoroutine(Fade(1f));

        //save scene date
        SaveLoadManager.Instance.StoreCurrentSceneDate();

        //set player position
        Player.Instance.gameObject.transform.position = spawnPosition;

        //call before scene unload event 
        EventHandler.CallBeforeSceneUnloadEvent();

        //unload the current active scene
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        //start loading the given scene and wait for nit to finish 
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        //call after scene load event
        EventHandler.CallAfterSceneLoadEvent();

        //load scene date
        SaveLoadManager.Instance.RrestoreCurrentSceneData();

        //start fading back in and wait for it to finish before exiting the function
        yield return StartCoroutine(Fade(0f));

        //call after scene load fade in event
        EventHandler.CallBeforeSceneUnloadEvent();
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        //allow the given scene to load over several frames and add it to the already loaded scenes (jsut the presistent scene at this point)
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        //find the scene that was most recently loaded (the one at the last index of the loaded scenes)
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        //set the newly loaded scene as the active scene (this marks it as the one to be unloaded next)
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private IEnumerator Start()
    {
        //set the initial alpha to start off with a black screen
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        //starts the first scene loading and waits for it to finsh 
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        //if the event has any subscribes, call it
        EventHandler.CallAfterSceneLoadEvent();

        SaveLoadManager.Instance.RrestoreCurrentSceneData();

        //once the scene is finished loading, starts fading in 
        StartCoroutine(Fade(0f));
    }

    //this is the main external point of contant and influence from the rest of the project
    //this will be called when player wants to switch scenes
    public void FadeAndloadScene(string sceneName, Vector3 spawnPosition)
    {
        //if a fade isnt happening then start fading and switching scenes
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }
}
