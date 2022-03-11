using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningCutScene : MonoBehaviour
{
    private Animator anim;
    public RectTransform vignetteTop;
    public RectTransform vignetteBottom;

    public void StartScene()
    {
        // Set the player to cutscene movement
        GameManager.instance.player.transform.position = new Vector3(0, -4.84f, 0);
        GameManager.instance.player.GetComponent<Player>().canMove = false;
        GameManager.instance.invCanBeShown = false;

        // Update animation state
        anim = GameManager.instance.playerObj.GetComponent<Animator>();
        anim.SetBool("Moving", true);
        anim.SetFloat("Vertical", 1);
        anim.SetFloat("LastMoveVert", 1);
        anim.SetFloat("Horizontal", 0);
        anim.SetFloat("LastMoveHor", 0);

        // Start playing the scene
        StartCoroutine(PlayScene());
    }

    private IEnumerator PlayScene()
    {
        StartCoroutine(GameManager.instance.FadeIn());

        GameManager.instance.playerObj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, .69f);
        // Move up
        while (GameManager.instance.playerObj.transform.position.y < -2.5f)
        {
            yield return null;
        }
        GameManager.instance.playerObj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        // Update moving state
        // Stop walking
        GameManager.instance.player.enabled = true;
        anim.SetBool("Moving", false);
        anim.SetFloat("Vertical", 0);
        yield return new WaitForSeconds(1f);

        StartCoroutine(FadeOutVignette());

        // Look left
        anim.SetFloat("LastMoveVert", 0);
        anim.SetFloat("LastMoveHor", -1);
        yield return new WaitForSeconds(.75f);
        // Look right
        anim.SetFloat("LastMoveHor", 1);
        yield return new WaitForSeconds(.75f);
        // Look up
        anim.SetFloat("LastMoveVert", 1);
        anim.SetFloat("LastMoveHor", 0);
        yield return new WaitForSeconds(.25f);

        // Re-enable the player movement
        GameManager.instance.player.GetComponent<Player>().canMove = true;
        GameManager.instance.playerObj.GetComponent<CircleCollider2D>().enabled = true;
        GameManager.instance.player.enabled = true;
    }

    private IEnumerator FadeOutVignette()
    {
        // Fade out the vignette
        float height = 1f;
        while (height > 0f)
        {
            height -= .004f;
            Vector3 hei = new Vector3(1f, height, 1f);
            // Move top bar
            vignetteTop.localScale = hei;
            // Move bottom bar
            vignetteBottom.localScale = hei;
            yield return null;
        }

        GameObject.Destroy(vignetteTop.parent.gameObject);
    }
}
