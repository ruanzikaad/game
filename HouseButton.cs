using UnityEngine;
using UnityEngine.EventSystems; // Necessário para o uso de Pointer Events

public class HouseButton : MonoBehaviour, IPointerDownHandler
{
    public CameraController cameraController;

    // Este método é chamado quando o botão é pressionado (tocado ou clicado)
    public void OnPointerDown(PointerEventData eventData)
    {
        if (cameraController != null)
        {
            cameraController.ReturnToInitialPosition();
        }
    }
}
