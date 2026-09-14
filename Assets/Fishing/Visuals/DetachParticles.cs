using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Sets parent to null and plays particles and audio.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class DetachParticles : MonoBehaviour
{
    public bool looping;
    public void Detach()
    {
        transform.parent = null;
        gameObject.SetActive(true);

        if (TryGetComponent(out AudioSource audioSource))
        {
            audioSource.Play();
        }

        ParticleSystem particleSystem = GetComponent<ParticleSystem>();

        if (looping)
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        else
            particleSystem.Play();

        StartCoroutine(WaitForDeath(particleSystem));
    }

    public void Duplicate()
    {
        DetachParticles dp = Instantiate(gameObject, transform.position, transform.rotation).GetComponent<DetachParticles>();
        dp.transform.localScale = transform.lossyScale;
        dp.Detach();
    }

    IEnumerator WaitForDeath(ParticleSystem ps)
    {
        // withChildren=true also checks sub-emitters
        while (ps.IsAlive(true))
            yield return null;
        Destroy(gameObject);
    }
}
