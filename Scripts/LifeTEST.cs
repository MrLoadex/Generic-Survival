using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class statsTEST : MonoBehaviour
{
    [SerializeField] private ActionType actionType = ActionType.Damage;
    [SerializeField] private float actionInterval = 1f;
    [SerializeField] private float amount = 10f;

    
    private IEnumerator ApplyAction()
    {
        switch (actionType)
        {
            case ActionType.Damage: // Daño
                PlayerEvents.TakeDamage?.Invoke(amount);
                break;
            case ActionType.Heal: // Curar
                PlayerEvents.Heal?.Invoke(amount);
                break;
            case ActionType.Drink: // Beber
                PlayerEvents.Drink?.Invoke(amount);
                break;
            case ActionType.Eat: // Comer
                PlayerEvents.Eat?.Invoke(amount);
                break;
            case ActionType.Rest: // Descansar
                PlayerEvents.Rest?.Invoke(amount);
                break;
        }

        yield return new WaitForSeconds(actionInterval);
        
        StartCoroutine(ApplyAction());
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(ApplyAction());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           StopAllCoroutines();
        }
    }
}

public enum ActionType
{
    Damage,
    Heal,
    Drink,
    Eat,
    Rest
}


