using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Barras de estadisticas del jugador
    [Header("Player Stats Bars")]
    [SerializeField] private Image playerHealthBar;
    [SerializeField] private Image playerThirstBar;
    [SerializeField] private Image playerHungerBar;
    [SerializeField] private Image playerFatigueBar;

    // Paneles
    [Header("Panels")]
    [SerializeField] private GameObject deathPanel;

    private void Start() {
        PlayerEvents.HealtUpdate += replyPlayerHealthUpdate;
        PlayerEvents.Death += replyPlayerDeath;
        PlayerEvents.Revive += replyPlayerRevive;
        PlayerEvents.StatsUpdate += replyPlayerStatsUpdate;
    }

    public void RevivePlayer()
    {
        PlayerEvents.Revive?.Invoke();
    }

    #region EVENTS
    private void replyPlayerHealthUpdate(float actual, float max)
    {
        playerHealthBar.fillAmount = actual / max;
    }

    private void replyPlayerDeath()
    {
        deathPanel.SetActive(true);
    }

    private void replyPlayerRevive()
    {
        deathPanel.SetActive(false);
    }

    private void replyPlayerStatsUpdate(float thirst, float hunger, float fatigue)
    {
        playerThirstBar.fillAmount = thirst / 100;
        playerHungerBar.fillAmount = hunger / 100;
        playerFatigueBar.fillAmount = fatigue / 100;
    }

    private void OnDisable()
    {
        PlayerEvents.HealtUpdate -= replyPlayerHealthUpdate;
        PlayerEvents.Death -= replyPlayerDeath;
        PlayerEvents.Revive -= replyPlayerRevive;
        PlayerEvents.StatsUpdate -= replyPlayerStatsUpdate;
    }    
    #endregion
}
