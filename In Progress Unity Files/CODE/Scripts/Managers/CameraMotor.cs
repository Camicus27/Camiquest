using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform focusObj;
    public float boundX = 0.03f;
    public float boundY = 0.2f;

    public bool fixedCamera = true;

    private void Start()
    {
        focusObj = GameManager.instance.playerObj.transform;
    }

    private void LateUpdate()
    {
        if (fixedCamera)
        {
            transform.position = new Vector3(focusObj.transform.position.x, focusObj.transform.position.y, -10);
        }
        else
        {
            Vector3 delta = Vector3.zero;

            // Check if we are inside the bounds on the X axis
            // Distance between focus area center and player
            float deltaX = focusObj.position.x - transform.position.x;
            if (deltaX > boundX || deltaX < -boundX)
            {
                // Player moving right
                if (transform.position.x < focusObj.position.x)
                {
                    delta.x = deltaX - boundX;
                }
                // Player moving left
                else
                {
                    delta.x = deltaX + boundX;
                }
            }

            // Check if we are inside the bounds on the Y axis
            // Distance between focus area center and player
            float deltaY = focusObj.position.y - transform.position.y;
            if (deltaY > boundY || deltaY < -boundY)
            {
                // Player moving up
                if (transform.position.y < focusObj.position.y)
                {
                    delta.y = deltaY - boundY;
                }
                // Player moving down
                else
                {
                    delta.y = deltaY + boundY;
                }
            }

            // Move the camera
            transform.position += new Vector3(delta.x, delta.y, 0);
        }
    }
}
