using UnityEngine;

public class PlayerFootController : PlayerController
{
    public override float getHorizontalAxis()
    {
        return Input.GetAxis("Horizontal");
    }
    public override float getVerticalAxis()
    {
        return Input.GetAxis("Vertical");
    }
    public override float getRotationXAxis()
    {
        return Input.GetAxis("Mouse X");
    }
    public override float getRotationYAxis()
    {
        return Input.GetAxis("Mouse Y");
    }
    public override bool getRunInput()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }
    public override bool getJumpInput()
    {
        return Input.GetButtonDown("Jump"); 
    }
    public override bool getOpenCloseInventoryInput()
    {
        return Input.GetKeyDown(KeyCode.I);
    }
    public override bool getAttackInput()
    {
        return Input.GetMouseButton(0);
    }
    public override bool getInteractInput()
    {
        return Input.GetKeyDown(KeyCode.F);
    }
    public override float getScrollInput()
    {
        return Input.GetAxis("Mouse ScrollWheel");
    }
}

