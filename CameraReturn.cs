using UnityEngine;

public class CameraReturn : MonoBehaviour
{
    public Transform cameraTransform; // Referência para a transformação da câmera
    public Vector3 originalPosition = new Vector3(-32, 80.2f, -36.3f); // Ajuste para usar a posição correta
    public float smoothSpeed = 1.5f; // Velocidade de interpolação
    private bool moveToOriginal = false;

    void Start()
    {
        // Garanta que a posição original seja a posição atual da câmera no início
        originalPosition = cameraTransform.position;
    }

    void Update()
    {
        if (moveToOriginal)
        {
            // Move a câmera suavemente para a posição original
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, originalPosition, smoothSpeed * Time.deltaTime);
        }

        // Verifique se a câmera está suficientemente perto da posição original para parar a interpolação
        if (Vector3.Distance(cameraTransform.position, originalPosition) < 5.1f)
        {
            moveToOriginal = false;
            cameraTransform.position = originalPosition; // Garante que a posição seja exatamente a original
        }
    }

    // Método chamado pelo botão House
    public void OnHouseButtonClick()
    {
        moveToOriginal = true;
    }
}
