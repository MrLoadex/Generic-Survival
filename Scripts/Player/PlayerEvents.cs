using System;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{

    public static Action<float, float> HealtUpdate;
    public static Action Death;
    public static Action Revive;
    public static Action<float, float, float> StatsUpdate; // thirst, hunger, fatigue
    public static Action Run;
    public static Action<float> TakeDamage;
    public static Action<float> Heal;
    public static Action Exhausted;
    public static Action Rested;
    public static Action<float> Drink;
    public static Action<float> Eat;
    public static Action<float> Rest;
}
