using System.Collections;
using UnityEngine;

public class Portal : Collidable
{
    public string sceneName;

    protected override void OnCollide()
    {
        GameManager.instance.StopMusic(2f);
        GetComponent<BoxCollider2D>().enabled = false;
        GameManager.instance.StopPlayerMovement();
        GameManager.instance.canQuit = false;
        GameManager.instance.PlaySound("SceneTransition");
        StartCoroutine(FadeAndTeleport());
    }

    private IEnumerator FadeAndTeleport()
    {
        // Fade to black
        yield return GameManager.instance.FadeOut();
        GameManager.instance.ResumePlayerMovement();
        // Teleport the player to the new scene
        GameManager.instance.sceneIsLoaded = false;
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }
}
