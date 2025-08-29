using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public string brokenLayerName = "Broken";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(brokenLayerName))
        {
            BlinkAndDestroy blink = collision.gameObject.GetComponent<BlinkAndDestroy>();
            if (blink != null)
            {
                blink.StartBlinking();
            }
        }
    }
}
