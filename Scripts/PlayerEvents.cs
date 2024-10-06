using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{

    public static Action<float, float> HealtUpdate;
    public static Action Death;
    public static Action Revive;
    // public static Action<Bed> OnPlayerResurrection; //Esto en un futuro

}
