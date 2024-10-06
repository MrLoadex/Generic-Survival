using UnityEngine;

public class LifeBase : MonoBehaviour
{
    [SerializeField] protected float initialLife;
    [SerializeField] protected float maxLife;

    public float currentLife {get; protected set;}
    // Start is called before the first frame update
    protected void Start()
    {
        currentLife = initialLife;
    }

    public void takeDamage(float count)
    {
        if (count < 0) { return;}
        if (currentLife <= 0) { return;}

        currentLife -= count;
        updateHealthBar(currentLife, maxLife);
        if (currentLife <= 0)
        {
            currentLife = 0;
            updateHealthBar(currentLife, maxLife);
            characterDefeated();
        }
    }

    protected virtual void updateHealthBar(float currentLife, float maxLife){}

    protected virtual void characterDefeated(){}
}
