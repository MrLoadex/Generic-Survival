using UnityEngine;

[RequireComponent(typeof(Collider))]

public class PlayerLife : LifeBase
{

    public bool defeated {private set; get;} = false;
    public bool canBeHealed => currentLife < maxLife && !defeated;
    

    private Collider _collider;

    private void Awake() {
        _collider = GetComponent<Collider>();
    }

    protected void Start() {
        base.Start();
        updateHealthBar(currentLife, maxLife);

        PlayerEvents.Revive += replyRevive;
        PlayerEvents.TakeDamage += replyTakeDamage;
        PlayerEvents.Heal += replyHeal;
    }

    protected override void updateHealthBar(float currentLife,float maxLife)
    {
        PlayerEvents.HealtUpdate?.Invoke(currentLife, maxLife);
    }

    protected override void characterDefeated()
    {
        defeated = true;
        PlayerEvents.Death?.Invoke();
    }

    public void Heal(float amount)
    {
        if (!canBeHealed) return;
        currentLife += amount;
        if (currentLife > maxLife)
        {
            currentLife = maxLife;
        }
        updateHealthBar(currentLife, maxLife);
        
    }
    #region EVENTS

    private void replyRevive()
    {
        currentLife = maxLife;
        updateHealthBar(currentLife, maxLife);
    }

    private void replyTakeDamage(float amount)
    {
        takeDamage(amount);
    }

    private void replyHeal(float amount)
    {
        Heal(amount);
    }

    #endregion
}
