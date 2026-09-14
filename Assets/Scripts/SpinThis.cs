using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class SpinThis : MonoBehaviour
{
    public float spinSpeed = 1f;
    public Transform blackHoleTr;

    private VisualEffect _vfx;
    private VisualEffect Vfx
    {
        get 
        { 
            if ( _vfx == null )
            {
                _vfx = GetComponent<VisualEffect>();
            }
            return _vfx; 
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
        Vfx.SetVector3("BlackHolePos", blackHoleTr.position);
    }
}
