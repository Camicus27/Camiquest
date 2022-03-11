using UnityEngine;

public class FarBackgroundScroller : MonoBehaviour
{
    //private Player playerInFocus;

    //public float horizontalScrollDistance = 0.3f;
    //public float verticalScrollDistance = 0.2f;

    //private void Start()
    //{
    //    GameObject playerObj = GameObject.Find("Player");
    //    playerInFocus = playerObj.GetComponent<Player>();
    //}

    //private void LateUpdate()
    //{
    //    Vector3 delta = Vector3.zero;

    //    // Distance between focus area center and player
    //    if (playerInFocus.isMoving)
    //    {
    //        // Player moving right
    //        if (playerInFocus.moveDelta.x > 0)
    //            delta.x = horizontalScrollDistance * Time.deltaTime;
    //        // Player moving left
    //        else if (playerInFocus.moveDelta.x < 0)
    //            delta.x = -horizontalScrollDistance * Time.deltaTime;

    //        // Player moving up
    //        if (playerInFocus.moveDelta.y > 0)
    //            delta.y = verticalScrollDistance * Time.deltaTime;
    //        // Player moving down
    //        else if (playerInFocus.moveDelta.y < 0)
    //            delta.y = -verticalScrollDistance * Time.deltaTime;
    //    }

    //    // Move the background accordingly
    //    transform.position += delta;
    //}
}
