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
    [SerializeField] private GameObject inventoryPanel;

    private void Start() {
        // Ocultar el cursor al iniciar el juego
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void closePanels()
    {
        inventoryPanel.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    #region EVENTS
    private void OnEnable() {
        PlayerEvents.HealtUpdate += replyPlayerHealthUpdate;
        PlayerEvents.Death += replyPlayerDeath;
        PlayerEvents.Revive += replyPlayerRevive;
        PlayerEvents.StatsUpdate += replyPlayerStatsUpdate;
        InventoryEvents.OpenCloseInventory += replyOpenCloseInventory;
    }

    private void OnDisable()
    {
        PlayerEvents.HealtUpdate -= replyPlayerHealthUpdate;
        PlayerEvents.Death -= replyPlayerDeath;
        PlayerEvents.Revive -= replyPlayerRevive;
        PlayerEvents.StatsUpdate -= replyPlayerStatsUpdate;
        InventoryEvents.OpenCloseInventory -= replyOpenCloseInventory;
    }

    private void OnDestroy() 
    {
        PlayerEvents.HealtUpdate -= replyPlayerHealthUpdate;
        PlayerEvents.Death -= replyPlayerDeath;
        PlayerEvents.Revive -= replyPlayerRevive;
        PlayerEvents.StatsUpdate -= replyPlayerStatsUpdate;   
        InventoryEvents.OpenCloseInventory -= replyOpenCloseInventory;
    }   
    
    private void replyPlayerHealthUpdate(float actual, float max)
    {
        playerHealthBar.fillAmount = actual / max;
    }

    private void replyPlayerDeath()
    {
        deathPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    private void replyPlayerRevive()
    {
        deathPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void replyPlayerStatsUpdate(float thirst, float hunger, float fatigue)
    {
        playerThirstBar.fillAmount = thirst / 100;
        playerHungerBar.fillAmount = hunger / 100;
        playerFatigueBar.fillAmount = fatigue / 100;
    }

    private void replyOpenCloseInventory(bool isOpen)
    {
        if (isOpen) {
            closePanels();
        }
        //abrir o cerrar el panel de inventario
        inventoryPanel.SetActive(isOpen);
        // Mostrar u ocultar el cursor
        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
    #endregion
}
