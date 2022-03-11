using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isInteractable = true;
    protected bool currentlyInteracting = false;
    protected bool isInRange = false;
    public float interactRange = 0.35f;

    protected virtual void Update()
    {
        // If object is interactable and not currently interacting and within range
        if (isInteractable && !currentlyInteracting && Vector3.Distance(GameManager.instance.player.transform.position, transform.position) < interactRange)
        {
            isInRange = true;
            GameManager.instance.keyPrompt.SetActive(true);

            // If pressing E
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentlyInteracting = true;
                GameManager.instance.keyPrompt.SetActive(false);
                OnInteraction();
            }
        }
        // Check if previously in range and now have left the area
        else if (isInRange && Vector3.Distance(GameManager.instance.player.transform.position, transform.position) >= interactRange)
        {
            isInRange = false;
            GameManager.instance.keyPrompt.SetActive(false);
        }
    }

    protected virtual void OnInteraction()
    {
        // to override
    }
}
