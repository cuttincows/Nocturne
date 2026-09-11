using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Video;

public class TrackTarget : MonoBehaviour
{
    public Transform LookAt;
    public float VerticalLookOffset = 0.3f;
    public float LerpSpeed = 0.8f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LookAt == null)
        {
            return;
        }       
        
        Vector3 lookPos = (LookAt.position - transform.position).normalized + Vector3.down * VerticalLookOffset;
        transform.forward = Vector3.Lerp(transform.forward, lookPos, LerpSpeed);
        // transform.forward = (LookAt.position - transform.position).normalized + Vector3.down * VerticalLookOffset;
    }
}
