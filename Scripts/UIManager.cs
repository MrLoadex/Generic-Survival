using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image playerHealthBar;
    [SerializeField] private GameObject deathPanel;

    private void Start() {
        PlayerEvents.HealtUpdate += replyPlayerHealthUpdate;
        PlayerEvents.Death += replyPlayerDeath;
        PlayerEvents.Revive += replyPlayerRevive;
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
    #endregion
}
