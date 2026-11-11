using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;
    
    public bool hasItem;
    public GameObject itemIcon;
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.CompareTag("Item"))
        {
            hasItem = true;
            itemIcon.SetActive(true);
            Destroy(hit.collider.gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("Totem"))
            {
                if (hit.collider.TryGetComponent(out Totem totem))
                {
                    totem.Deposit(this);
                }
            }
        }
    }
    
}
