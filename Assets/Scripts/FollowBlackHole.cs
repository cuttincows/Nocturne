using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class FollowBlackHole : MonoBehaviour
{
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

    void Update()
    {
        Vfx.SetVector3("BlackHolePos", blackHoleTr.position);
    }
}
