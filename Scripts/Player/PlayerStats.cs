using UnityEngine;
using System.Collections;
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float thirst = 100; //sed
    [SerializeField] private float hunger = 100; //hambre
    [SerializeField] private float fatigue = 0; //fatiga

    public float Thirst => thirst;
    public float Hunger => hunger;
    public float Fatigue => fatigue;

    //valores originales de consumo de stats
    [Header("Stats Consume Rate")]
    [SerializeField] private const float originalThirstConsumeRate = 0.25f;
    [SerializeField] private const float originalHungerConsumeRate = 0.15f;
    [SerializeField] private const float originalFatigueConsumeRate = 0.2f;

    [Header("Damage")]
    [SerializeField] float damagePerSecond = 10;

    [Header("Heal")]
    [SerializeField] float healPerSecond = 2; // Curacion por segundo si hambre >= 80% de lo contrario es la mitad

    private float actualThirstConsumeRate = 0.25f;
    private float actualHungerConsumeRate = 0.15f;
    private float actualFatigueConsumeRate = 0.2f;

    bool exhausted = false;
    bool healing = false;

    private void Start()
    {
        // Subscripcion a eventos
        PlayerEvents.Revive += replyRevive;
        PlayerEvents.Run += replyPlayerRun;
        PlayerEvents.Drink += replyDrink;
        PlayerEvents.Eat += replyEat;
        PlayerEvents.Rest += replyRest;
        PlayerEvents.HealtUpdate += replyHealthUpdate;
        StartCoroutine(UpdateStats());
    }

    private IEnumerator UpdateStats()
    {
        yield return new WaitForSeconds(1);
        consumeStats();
        PlayerEvents.StatsUpdate(thirst, hunger, fatigue);
        StartCoroutine(UpdateStats());
    }

    private void consumeStats()
    {
        thirst = Mathf.Clamp(thirst - actualThirstConsumeRate, 0, 100);
        hunger = Mathf.Clamp(hunger - actualHungerConsumeRate, 0, 100);
        fatigue = Mathf.Clamp(fatigue + actualFatigueConsumeRate, 0, 100);
        checkAndTriggerEvents();
    }

    private void checkAndTriggerEvents()
    {
        if (thirst <= 0)
        {
            PlayerEvents.TakeDamage(damagePerSecond);
        }
        if (hunger <= 0)
        {
            PlayerEvents.TakeDamage(damagePerSecond);
        }
        if (fatigue >= 100 && !exhausted)
        {
            exhausted = true;
            PlayerEvents.Exhausted();
        }
        if (fatigue <= 99 && exhausted)
        {
            exhausted = false;
            PlayerEvents.Rested();
        }
    }

    //Metodos para modificar las estadisticas del jugador
    public void Drink(float amount) // beber
    {
        thirst = Mathf.Clamp(thirst + amount, 0, 100);
    }

    public void Eat(float amount) // comer
    {
        hunger = Mathf.Clamp(hunger + amount, 0, 100);
    }

    public void Rest(float amount) // descansar
    {
        fatigue = Mathf.Clamp(fatigue - amount, 0, 100);
    }

    private void consumeThirst(float amount)
    {
        thirst = Mathf.Clamp(thirst - amount, 0, 100);
        checkAndTriggerEvents();
    }

    private void consumeHunger(float amount)
    {
        hunger = Mathf.Clamp(hunger - amount, 0, 100);
        checkAndTriggerEvents();
    }

    private void increaseFatigue(float amount)
    {
        fatigue = Mathf.Clamp(fatigue + amount, 0, 100);
        checkAndTriggerEvents();
    }

    #region EVENTS
    private void replyRevive()
    {
        thirst = 100;
        hunger = 100;
        fatigue = 0;
    }

    private void replyPlayerRun()
    {
        // Aumentar el consumo de stats durante un segundo
        actualThirstConsumeRate = originalThirstConsumeRate * 2;
        actualHungerConsumeRate = originalHungerConsumeRate * 2;
        actualFatigueConsumeRate = originalFatigueConsumeRate * 2; 
        // Actualizar los eventos de stats
        PlayerEvents.StatsUpdate(thirst, hunger, fatigue);

        // Devolver el consumo de stats a la normalidad después de un segundo
        StartCoroutine(WaitAndResetStatsConsumption());
    }

    private IEnumerator WaitAndResetStatsConsumption()
    {
        yield return new WaitForSeconds(1);
        actualThirstConsumeRate = originalThirstConsumeRate;
        actualHungerConsumeRate = originalHungerConsumeRate;
        actualFatigueConsumeRate = originalFatigueConsumeRate;
    }

    private void replyDrink(float amount)
    {
        Drink(amount);
    }

    private void replyEat(float amount)
    {
        Eat(amount);
    }

    private void replyRest(float amount)
    {
        Rest(amount);
    }

    private void replyHealthUpdate(float health, float maxHealth)
    {
        if (health < maxHealth && hunger > 20) // si la vida es menor a la vida maxima y el hambre es mayor a 20
        {
            if (!healing)
            {
                healing = true;
                StartCoroutine(HealOverTime());
            }
        }
    }

    private IEnumerator HealOverTime()
    {
        yield return new WaitForSeconds(1);
        healing = false; // Se llama inmediatamente luego de esperar un segundo porque el evento Heal puede ejecutarse en otro hilo y cortaria la corrutina

        if (hunger >= 80) // si el hambre es mayor o igual a 80 || No deberia hardcodear valores pero no tengo ganas de hacer un sistema de configuracion con serializefields
        {
            PlayerEvents.Heal(healPerSecond);
        }
        else
        {
            PlayerEvents.Heal(healPerSecond / 2);
        }
        consumeHunger(1f);
    }

    private void OnDisable()
    {
        PlayerEvents.Revive -= replyRevive;
        PlayerEvents.Run -= replyPlayerRun;
        PlayerEvents.Drink -= replyDrink;
        PlayerEvents.Eat -= replyEat;
        PlayerEvents.Rest -= replyRest;
        PlayerEvents.HealtUpdate -= replyHealthUpdate;
    }
    #endregion
}
