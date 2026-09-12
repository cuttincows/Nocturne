using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SpearState
{
    Held,
    Throwing,
    Thrown,
    Retracting
}

public class Spear : MonoBehaviour
{
    public SpearTarget target;
    public Rigidbody spear;

    public float throwSpeed = 10f;
    public float retractSpeed = 10f;

    public SpearState CurrentState { get; private set; } = SpearState.Held;

    public Action<SpearState> OnStateChanged;
    public static Action<Spear, SpearState> OnAnySpearStateChanged;

    private void FixedUpdate()
    {
        if (CurrentState == SpearState.Throwing || CurrentState == SpearState.Retracting)
        {
            MoveSpear();
        }
    }

    private void Update()
    {
        if (CurrentState == SpearState.Held)
        {
            transform.LookAt(target.transform);
        }
        // TODO: change this to be controller friendly
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (CurrentState == SpearState.Held)
            {
                ThrowSpear();
            }
            else if (CurrentState == SpearState.Thrown)
            {
                RetractSpear();
            }
        }
    }

    private void MoveSpear()
    {
        if (CurrentState == SpearState.Throwing)
        {
            if (MoveTo(target.transform.position, throwSpeed))
            {
                Physics.SyncTransforms();
                ChangeState(SpearState.Thrown);
            }
        }
        else if (CurrentState == SpearState.Retracting)
        {
            if (MoveTo(transform.position, retractSpeed))
            {
                Physics.SyncTransforms();
                ChangeState(SpearState.Held); 
                target.enabled = true;
            }
        }
    }

    // Returns true if we are already within tolerance, otherwise it moves to the target.
    private float tolerance = 0.001f;
    private bool MoveTo(Vector3 targetPosition, float speed)
    {
        if (Vector3.Distance(spear.position, targetPosition) < tolerance) return true;
        spear.MovePosition(Vector3.MoveTowards(spear.position, targetPosition, Time.deltaTime * speed));
        return false;
    }
    
    private void ChangeState(SpearState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
        OnAnySpearStateChanged?.Invoke(this, newState);
    }

    private void ThrowSpear()
    {
        ChangeState(SpearState.Throwing);
        target.enabled = false;
    }

    private void RetractSpear()
    {
        ChangeState(SpearState.Retracting);
    }
}
