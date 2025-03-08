using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class ObjectSelection : MonoBehaviour
{
    public GameObject selectionPanel;
    public TextMeshProUGUI objectNameText;
    public Button moveButton;
    public Button removeButton;
    public Button closeButton;
    public Material highlightMaterial; 

    private GameObject selectedObject;
    private Material originalMaterial;
    private int selectedConstructionId;

    void Start()
    {
        selectionPanel.SetActive(false);

        moveButton.onClick.AddListener(MoveObject);
        removeButton.onClick.AddListener(RemoveObject);
        closeButton.onClick.AddListener(ClosePanel);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                SelectObject(hit.collider.gameObject);
            }
        }
    }

    void SelectObject(GameObject obj)
{
    if (obj.CompareTag("Terra") || obj.CompareTag("CacauSeedling"))
    {
        if (selectedObject != null && selectedObject.GetComponent<Renderer>() != null)
        {
            selectedObject.GetComponent<Renderer>().material = originalMaterial;
        }

        selectedObject = obj;

        // 🔹 Pegamos o ID da construção armazenado no ConstructionData
        ConstructionData constructionData = obj.GetComponent<ConstructionData>();
        if (constructionData != null)
        {
            selectedConstructionId = constructionData.id_construcao; // Obtém o ID correto
            Debug.Log($"🔎 Objeto selecionado com ID: {selectedConstructionId}");
        }
        else
        {
            Debug.LogWarning("⚠️ Objeto selecionado não possui um ID de construção.");
            selectedConstructionId = -1; // Define um ID inválido
        }

        string cleanName = obj.name.Replace(" Variant(Clone)", "").Replace("(Clone)", "");
        objectNameText.text = cleanName;

        Renderer objRenderer = selectedObject.GetComponent<Renderer>();
        if (objRenderer != null)
        {
            originalMaterial = objRenderer.material;
            objRenderer.material = highlightMaterial;
        }

        selectionPanel.SetActive(true);
    }
}


    void MoveObject()
    {
        if (selectedObject != null)
        {
            Debug.Log("Modo de movimentação ativado para " + selectedObject.name);
        }
    }

    void RemoveObject()
    {
        if (selectedObject != null)
        {
            StartCoroutine(DeleteConstructionFromDB(selectedConstructionId));

            Destroy(selectedObject);
            ClosePanel();
        }
    }

    IEnumerator DeleteConstructionFromDB(int constructionId)
    {
        int userId = PlayerPrefs.GetInt("user_id", -1);
        if (userId == -1)
        {
            Debug.LogError("Usuário não encontrado nos PlayerPrefs!");
            yield break;
        }

        string uri = $"https://revvopublicidade.com.br/api/delete_construction.php?id_construcao={constructionId}&id_usuario={userId}";

        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Construção deletada do banco de dados: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("❌ Erro ao deletar construção: " + www.error);
            }
        }
    }

    void ClosePanel()
    {
        if (selectedObject != null && selectedObject.GetComponent<Renderer>() != null)
        {
            selectedObject.GetComponent<Renderer>().material = originalMaterial;
        }

        selectionPanel.SetActive(false);
        selectedObject = null;
    }
}
