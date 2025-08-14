using UnityEngine;

public class DestroyableByInteraction : MonoBehaviour, Interactable
{
    public void Interact()
    {
        Destroy(gameObject);
    }
}
