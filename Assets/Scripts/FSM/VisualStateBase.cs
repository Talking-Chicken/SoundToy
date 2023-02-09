using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisualStateBase
{
    public abstract void EnterState(VisualManager manager);
    public abstract void Update(VisualManager manager);
    public abstract void LeaveState(VisualManager manager);
}
