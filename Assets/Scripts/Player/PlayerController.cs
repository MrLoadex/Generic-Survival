using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController
{
    public abstract float getHorizontalAxis();
    public abstract float getVerticalAxis();
    public abstract float getRotationXAxis();
    public abstract float getRotationYAxis();
    public abstract bool getRunInput();
    public abstract bool getJumpInput();
    public abstract bool getOpenCloseInventoryInput();
    public abstract bool getAttackInput();
    public abstract bool getInteractInput();
    public abstract float getScrollInput();
}
