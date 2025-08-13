using UnityEngine;
using UnityEngine.EventSystems;

public class UIControlZone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public PlayerMovement.MovementState zoneState;

    [SerializeField] Texture2D _cursor;

    private PlayerMovement playerMovement;
    

    void Awake()
    {
        SetComponents();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(_cursor, new Vector2(0,0), CursorMode.Auto);
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