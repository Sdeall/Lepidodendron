using UnityEngine;

public enum ControlMode {Walk, Interact, Weapon, Inventory}

public class PlayerModes : MonoBehaviour
{
    public ControlMode CurrentMode;
}
