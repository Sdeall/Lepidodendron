using UnityEngine;
using UnityEngine.UI;

public interface Interactable
{
    void Interact();
}

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] RawImage renderTarget;
    [SerializeField] float interactDistance = 2f;

    PlayerModes playerModes;

    void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        playerModes = GetComponent<PlayerModes>();
    }

    void Update()
    {
        if (playerModes.CurrentMode == ControlMode.Interact)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryInteract();
            }
        }
    }

    void TryInteract()
    {
        RectTransform rt = renderTarget.rectTransform;
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out localPos);

        Vector2 uv;
        uv.x = (localPos.x + rt.rect.width * 0.5f) / rt.rect.width;
        uv.y = (localPos.y + rt.rect.height * 0.5f) / rt.rect.height;

        Vector3 renderTexPos = new Vector3(
            uv.x * playerCamera.pixelWidth,
            uv.y * playerCamera.pixelHeight,
            0f
        );

        Ray ray = playerCamera.ScreenPointToRay(renderTexPos);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.blue, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green, 1f);

            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(playerCamera.transform.position, hit.point);
                Debug.Log($"[Interact] Hit: {hit.collider.name}, Distance: {distance}");

                if (distance <= interactDistance)
                {
                    interactable.Interact();
                }
            }
        }
    }
}