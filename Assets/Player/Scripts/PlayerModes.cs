  using UnityEngine;

public enum ControlMode {Walk, Interact, Weapon, Inventory}

public class PlayerModes : MonoBehaviour
{
    [SerializeField] Texture2D[] _cursors;
    [SerializeField] GameObject _controllerUI;

    public ControlMode CurrentMode;

    public void SetMode(ControlMode mode)
    {
        CurrentMode = mode;
        UpdateMode(CurrentMode);
    }

    void UpdateMode(ControlMode mode)
    {
        _controllerUI.SetActive(false);
        Cursor.visible = true;

        switch (mode)
        {
            case ControlMode.Walk:
                WalkMode();
                break;

            case ControlMode.Interact:
                InteractMode();
                break;

            case ControlMode.Weapon:
                WeaponMode();
                break;

            case ControlMode.Inventory:
                InventoryMode();
                break;

            default:
                break;
        }
    }

    void WalkMode()
    {
        _controllerUI.SetActive(true);
    }
    void InteractMode()
    {
        Cursor.SetCursor(_cursors[0], new Vector2(8, 8), CursorMode.Auto);
    }
    void InventoryMode()
    {
        Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto);
        Cursor.visible = false;
    }
    void WeaponMode()
    {
        Cursor.SetCursor(_cursors[1], new Vector2(8, 8), CursorMode.Auto);
    }
}
