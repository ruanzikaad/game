using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 0.5f; // Velocidade de movimento ajustada para ser mais fluida
    public float zoomSpeed = 5f;  // Velocidade do zoom
    public float minZoom = 5f;   // Zoom mínimo permitido
    public float maxZoom = 30f;  // Zoom máximo permitido
    public float returnSpeed = 2f; // Velocidade de retorno da câmera
    public Vector2 minBounds; // Limites mínimos do mapa (X, Y)
    public Vector2 maxBounds; // Limites máximos do mapa (X, Y)

    private Vector3 lastPanPosition; // Última posição do toque/mouse
    private bool isPanning; // Verifica se a câmera está se movendo


    public Vector3 initialPosition; // Posição inicial definida no Inspector
    private bool isReturning = false; // Indica se a câmera está voltando
    private Vector3 targetPosition; // Guarda a posição para onde a câmera deve ir

    void Update()
    {
        HandlePan();
        HandleZoom();
    }

    void HandlePan()
    {
        // PC: Movimento com o botão esquerdo do mouse pressionado
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isPanning = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector3 currentPanPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = lastPanPosition - currentPanPosition;
            transform.position += new Vector3(delta.x, 0, delta.z) * panSpeed;
            lastPanPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        // Mobile: Movimento com um dedo na tela
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastPanPosition = Camera.main.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 currentPanPosition = Camera.main.ScreenToWorldPoint(touch.position);
                Vector3 delta = lastPanPosition - currentPanPosition;
                transform.position += new Vector3(delta.x, 0, delta.z) * panSpeed;
                lastPanPosition = Camera.main.ScreenToWorldPoint(touch.position);
            }
        }

        // Limita a movimentação da câmera dentro dos limites do mapa
        float clampedX = Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x);
        float clampedZ = Mathf.Clamp(transform.position.z, minBounds.y, maxBounds.y);
        transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
    }

    void HandleZoom()
    {
        // Zoom no PC (Scroll do mouse)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float newSize = Camera.main.orthographicSize - scroll * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }

        // Zoom no Mobile (Pinch com dois dedos)
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            float prevDistance = (touch1.position - touch1.deltaPosition - (touch2.position - touch2.deltaPosition)).magnitude;
            float currentDistance = (touch1.position - touch2.position).magnitude;
            float difference = currentDistance - prevDistance;

            float newSize = Camera.main.orthographicSize - difference * zoomSpeed * 0.01f;
            Camera.main.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }

     public void ReturnToInitialPosition()
    {
        isReturning = true;
        targetPosition = initialPosition;
    }

    void SmoothReturnToInitialPosition()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, returnSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isReturning = false;
        }
    }

}
