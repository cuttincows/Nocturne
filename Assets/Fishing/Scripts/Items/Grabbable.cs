using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public Rigidbody rb;
    public Grabber grabber;

    public bool Grab(Grabber grabber)
    {
        if (this.grabber != null) return false;
        this.grabber = grabber;
        return true;
    }

    private void FixedUpdate()
    {
        if (grabber != null)
            rb.MovePosition(grabber.transform.position);
    }

    public void Detach()
    {
        if (grabber != null)
        {
            grabber.Detach(this);
            grabber = null;
        }
    }
}
