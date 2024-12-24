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
    
    public void RevivePlayer()
    {
        PlayerEvents.Revive?.Invoke();
    }
    
    #region EVENTS

    private void OnEnable() {
        PlayerEvents.Revive += replyRevive;
        PlayerEvents.TakeDamage += replyTakeDamage;
        PlayerEvents.Heal += replyHeal;
        PlayerEvents.Fall += replyFall;
    }

    private void OnDisable() {
        PlayerEvents.Revive -= replyRevive;
        PlayerEvents.TakeDamage -= replyTakeDamage;
        PlayerEvents.Heal -= replyHeal;
        PlayerEvents.Fall -= replyFall;
    }

    private void OnDestroy() {
        PlayerEvents.Revive -= replyRevive;
        PlayerEvents.TakeDamage -= replyTakeDamage;
        PlayerEvents.Heal -= replyHeal;
        PlayerEvents.Fall -= replyFall;
    }

    private void replyRevive()
    {
        defeated = false;
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

    private void replyFall(float fallHeight)
    {
        if (defeated) return;

        float damage = 0;
        float deathHeight = 11f;

        if (fallHeight > 4f)
        {
            if (fallHeight <= deathHeight)
            {
                damage = (maxLife / deathHeight) * (fallHeight - 4f);
            }
            else
            {
                damage = maxLife;
            }
        }

        takeDamage(damage);
    }
    #endregion
}
