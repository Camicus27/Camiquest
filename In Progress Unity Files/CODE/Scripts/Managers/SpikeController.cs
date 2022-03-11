using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeController : MonoBehaviour
{
    public Lever leverActivater;
    public bool isStationary;
    public Spike[] spikes;
    private PolygonCollider2D[] colliders;
    private SpriteRenderer[] renderers;
    private float markedTime;

    void Start()
    {
        // Fill the collider/renderer arrays
        colliders = new PolygonCollider2D[spikes.Length];
        renderers = new SpriteRenderer[spikes.Length];
        for (int i = 0; i < spikes.Length; i++)
        {
            colliders[i] = spikes[i].GetComponent<PolygonCollider2D>();
            renderers[i] = spikes[i].GetComponent<SpriteRenderer>();
        }

        StartCoroutine(OpenAndClose());
    }

    private IEnumerator OpenAndClose()
    {
        if (!isStationary)
        {
            while (!leverActivater.ON)
            {
                // Disable the spikes
                markedTime = Time.time;
                yield return new WaitUntil(isLeverOnOrAfterSomeTime);
                for (int i = 0; i < spikes.Length; i++)
                {
                    colliders[i].enabled = false;
                    renderers[i].enabled = false;
                }

                if (leverActivater.ON) yield break;

                // Enable the spikes
                markedTime = Time.time;
                yield return new WaitUntil(isLeverOnOrAfterSomeTime);
                if (!leverActivater.ON)
                {
                    for (int i = 0; i < spikes.Length; i++)
                    {
                        colliders[i].enabled = true;
                        renderers[i].enabled = true;
                    }
                }
            }

            // Disable the spikes
            for (int i = 0; i < spikes.Length; i++)
            {
                colliders[i].enabled = false;
                renderers[i].enabled = false;
            }
        }
        else
        {
            yield return new WaitUntil(isLeverOn);
            // Disable the spikes
            for (int i = 0; i < spikes.Length; i++)
            {
                colliders[i].enabled = false;
                renderers[i].enabled = false;
            }
        }
    }

    private bool isLeverOn()
    {
        if (leverActivater.ON)
            return true;
        else
            return false;
    }
    private bool isLeverOnOrAfterSomeTime()
    {
        if (leverActivater.ON || Time.time - markedTime > 1.5f)
            return true;
        else
            return false;
    }
}
