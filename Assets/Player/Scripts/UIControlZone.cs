using UnityEngine;
using UnityEngine.EventSystems;

public class UIControlZone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerMovement.MovementState zoneState;

    private PlayerMovement playerMovement;

    void Awake()
    {
        SetComponents();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        playerMovement.SetMovementState(zoneState);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        playerMovement.SetMovementState(PlayerMovement.MovementState.Static);
    }

    void SetComponents()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        if (playerMovement == null)
            Debug.LogError("PlayerMovement not found!");
    }
}