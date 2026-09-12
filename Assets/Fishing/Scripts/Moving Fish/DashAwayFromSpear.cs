using UnityEngine;

public class DashAwayFromSpear : DirectionProvider
{
    public NearbySensorDirector director;
    SpearState spearState;
    public float multiplier;

    public override bool Can_Perform => director.Can_Perform && enabled && IsSpearing();

    public override Vector3 GetDirection()
    {
        return director.GetDirection() * multiplier;
    }

    private void Start()
    {
        Spear.OnAnySpearStateChanged += OnAnySpearStateChanged;
    }

    private void OnDestroy()
    {
        Spear.OnAnySpearStateChanged -= OnAnySpearStateChanged;
    }

    private void OnAnySpearStateChanged(Spear spear, SpearState state)
    {
        spearState = state;
    } 

    public bool IsSpearing()
    {
        return spearState == SpearState.Throwing;
    }
}
