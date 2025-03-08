using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using MyGame.Models;
using UnityEngine.UI;
using System;
using System.Globalization;


public class ClickableObject : MonoBehaviour
{
    public delegate void ClickAction();
    public event ClickAction OnClick;

    private void OnMouseDown()
    {
        OnClick?.Invoke();
    }
}


[System.Serializable]
public class ResponseDataConstrucao
{
    public string status;
    public string message;
    public int construcaoId;  // <-- Adiciona o campo para receber o ID da construção
}


public class ShopManager : MonoBehaviour
{
     [Header("UI Components")]
    [Tooltip("Display for the user's current coins.")]
    public TMP_Text coinsText;

    public GameObject textMeshPrefab; // Referência ao prefab do TextMesh
    private GameObject textMeshInstance; // Instância do TextMesh para manipulação


    [Tooltip("Display for the user's current XP.")]
    public TMP_Text xpText;
 
    [Header("Audio Elements")]
    [Tooltip("Display for the user's current coins.")]
    public AudioSource audioSourceCashIn;
    public AudioSource  audioSourcePlaced;
    
    public AudioClip cashInSound, placedInSound;



    [Header("Painéis do menu")]

    [Tooltip("Prefab for the Menu")]
    public GameObject painelMenu, painelMenuFerramentas, painelMenuRecursos, espacoDisponivel;

    [Header("Prefabs dos itens do menu")]



    [Tooltip("Prefab for the Cacau Seedling item.")]
    public GameObject cacauSeedlingPrefab;
    public GameObject enxadaBasicaPrefab;

    public float checkHeight = 1.0f; // Altura para detectar se há algo sobre a terra
 
     [Header("Spawn dos itens do menu")]

 
    [Tooltip("Spawn offset for the Cacau Seedling prefab.")]
    public Vector3 cacauSeedlingSpawnOffset;
    public Vector3 enxadaBasicaSpawnOffset = Vector3.zero;

    private Vector3 snappedPosition;
    private Dictionary<Vector3, GameObject> occupiedPositions;
    public float checkRadius = 0.0f; // Raio de detecção para sobreposição


    [Header("Private Fields")]
    [SerializeField, Tooltip("List of items available in the shop.")]
    private List<ShopItem> itemsAvailable = new List<ShopItem>();

    [Header("Player Info")]


    [SerializeField, Tooltip("Current amount of coins the user has.")]
    private int currentCoins;

    [SerializeField, Tooltip("Current XP level of the user.")]
    private int currentXP;

    [Header("Materials")]
    public Material transparentGreenMaterial, transparentRedMaterial;
    private Material originalMaterial;

    private GameObject currentPreviewItem;
    private int currentConstrucaoId;
    public bool isPlaced = false;

    private int clickCount = 0;
    private float clickTime;
    private const float doubleClickThreshold = 0.3f;

    private void Start()
    {
        // Adicionar itens à loja
        itemsAvailable.Add(new CacauSeedling("Muda de Cacau", 100, 1, 50, 120, cacauSeedlingPrefab, cacauSeedlingSpawnOffset));
        itemsAvailable.Add(new EnxadaBasica("Enxada Básica", 100, 1, 100, enxadaBasicaPrefab, enxadaBasicaSpawnOffset));
        espacoDisponivel.SetActive(false);
        currentXP = 0;
        UpdateDisplay();
        occupiedPositions = new Dictionary<Vector3, GameObject>();



    }

    
 private void HandleClick()
    {
        clickCount++;
        if (clickCount == 1)
        {
            clickTime = Time.time;
        }
        else if (clickCount == 2 && (Time.time - clickTime) <= doubleClickThreshold)
        {
            ConfirmPlacement();
            clickCount = 0;
        }
        else
        {
            clickCount = 1;
            clickTime = Time.time;
        }
    }

private bool HasAvailableLand()
{
    GameObject[] terras = GameObject.FindGameObjectsWithTag("Terra");

    foreach (GameObject terra in terras)
    {
   

        // Define um ponto acima da terra para checar se há objetos lá
        Vector3 checkPosition = terra.transform.position + Vector3.up * checkHeight;

        // Usa uma esfera para detectar colisões acima da terra
        Collider[] colliders = Physics.OverlapSphere(checkPosition, checkRadius);

        bool isOccupied = false;
        foreach (Collider col in colliders)
        {
            if (col.gameObject != terra) // Se houver outro objeto além da terra, está ocupado
            {
                isOccupied = true;
                break;
            }
        }

        if (!isOccupied) // Se achamos uma terra livre, retorna verdadeiro
        {
            return true;
        }
    }

    return false; // Nenhuma terra disponível
}

   public void TryBuyItem(string itemName)
{
    isPlaced = false;
    ShopItem item = itemsAvailable.Find(i => i.Name == itemName);

    if (item != null)
    {
         if (item is CacauSeedling && !HasAvailableLand())
            {
                Debug.Log("Não há espaço disponível para plantar!");
                espacoDisponivel.SetActive(true);
                return;
            }
            BuyItem(item);
    }
    else
    {
        Debug.Log("Item não encontrado.");
    }
}

    public void BuyItem(ShopItem item)
{

     
    if (item is CacauSeedling seedling)  // Verifica se o item é uma instância de CacauSeedling
    {
        StartCoroutine(BuyItemCoroutine(seedling));
    }
    else
    {
        // Trata outros tipos de itens sem XpReward
        StartCoroutine(BuyItemCoroutine(item));
    }
}

private IEnumerator BuyItemCoroutine(ShopItem item)
{
    WWWForm form = new WWWForm();
    form.AddField("user_id", PlayerPrefs.GetInt("user_id"));
    form.AddField("item_name", item.Name);
    form.AddField("item_cost", item.Cost);
    form.AddField("xp_recompensa", item.XpReward);

    using (UnityWebRequest www = UnityWebRequest.Post("https://revvopublicidade.com.br/api/buy_item.php", form))
    {
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<ResponseDataConstrucao>(www.downloadHandler.text);
            Debug.Log("Server response: " + response.message);

            if (response.status == "success")
            {
                Debug.Log("Compra efetuada com sucesso!");
                PlayAudioCash();

                StartCoroutine(FetchUserData(PlayerPrefs.GetInt("user_id"))); 

                    painelMenu.SetActive(false);
                    painelMenuFerramentas.SetActive(false);
                    painelMenuRecursos.SetActive(false);

                // ✅ Obtém o ID da construção retornado pelo servidor
                int construcaoId = response.construcaoId;
                currentConstrucaoId = response.construcaoId;

                EnterConstructionMode(item);

               
            }
            else
            {
                Debug.LogError("Falha na compra: " + response.message);
            }
        }
        else
        {
            Debug.LogError("Erro na comunicação com o servidor: " + www.error);
        }
    }
}

    private void EnterConstructionMode(ShopItem item)
    {
        if (currentPreviewItem != null)
        {
            Destroy(currentPreviewItem);
        }
        
        if(!isPlaced){
            StartCoroutine(HandleConstructionMode(item));
        }else{
            Debug.Log("O objeto já foi posiciondo.");
        }
     }

    
     private IEnumerator HandleConstructionMode(ShopItem item)
{
    while (!isPlaced)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if ((item is CacauSeedling && hit.collider.CompareTag("Terra")) || 
                    (!(item is CacauSeedling) && hit.collider.CompareTag("Terrain")))
                {
                    if (currentPreviewItem == null)
                    {
                        // Primeiro instancia o objeto no ponto onde o raycast acertou
                        currentPreviewItem = Instantiate(item.Prefab, SnapToGrid(hit.point), Quaternion.identity);
                        // Depois que o objeto é instanciado, tenta buscar o componente MeshRenderer
                        MeshRenderer meshRenderer = currentPreviewItem.GetComponent<MeshRenderer>();
                        if (meshRenderer != null)  // Verifica se o componente MeshRenderer foi encontrado
                        {
                            originalMaterial = meshRenderer.material;  // Armazena o material original
                            meshRenderer.material = transparentGreenMaterial;  // Aplica o material transparente
                        }
                        else
                        {
                            Debug.LogError("Erro: O objeto instanciado não possui um componente MeshRenderer.");
                        }
                        currentPreviewItem.AddComponent<ClickableObject>().OnClick += ConfirmPlacement;
                    }
                     
                }
            }
        }
        yield return null;
    }
}

   private void ConfirmPlacement()
{
    Vector3 snappedPosition = SnapToGrid(currentPreviewItem.transform.position);
    
    // Check if the position is already occupied
    if (occupiedPositions.ContainsKey(snappedPosition))
    {
        Debug.Log("🚫 Position is already occupied.");
        return; // Prevent placement
    }

    // Otherwise, mark this position as occupied
    occupiedPositions[snappedPosition] = currentPreviewItem;

    // Proceed with setting the position and updating the database
    StartCoroutine(UpdateItemPositionInDatabase(currentConstrucaoId, snappedPosition, currentPreviewItem.transform.localScale));

    isPlaced = true; // Mark the item as placed
    MeshRenderer meshRenderer = currentPreviewItem.GetComponent<MeshRenderer>();
    if (meshRenderer != null)
    {
        meshRenderer.material = originalMaterial; // Restore the original material
    }
    else
    {
        Debug.LogError("Error: The instantiated object does not have a MeshRenderer component.");
    }

    // Initiate growth if applicable
    if (currentPreviewItem.tag == "CacauSeedling")
    {
        StartGrowth(currentPreviewItem);
    }

    currentPreviewItem = null; // Clear the reference
    PlayAudioPlaced();
}




    private void StartGrowth(GameObject seedling)
{
    // Cada planta deve ter seu próprio TextMesh, garantindo que não compartilhem a mesma instância
    GameObject textMeshInstance = Instantiate(textMeshPrefab, seedling.transform.position + Vector3.up * 2, Quaternion.identity);
    textMeshInstance.transform.localScale = Vector3.one * 0.1f; // Ajuste o tamanho conforme necessário
    StartCoroutine(Grow(seedling, textMeshInstance)); // Passa a instância específica para a corrotina
}

    private IEnumerator Grow(GameObject seedling, GameObject textMeshInstance)
{
    float growthDuration = 90.0f;
    float currentTime = 0;
    TextMeshPro textComponent = textMeshInstance.GetComponent<TextMeshPro>();

    while (currentTime <= growthDuration)
    {
        float remainingTime = growthDuration - currentTime;
        textComponent.text = $"{remainingTime:N1} s";
        seedling.transform.localScale = Vector3.Lerp(new Vector3(0.1f, 0.1f, 0.1f), new Vector3(1.1f, 1.1f, 1.1f), currentTime / growthDuration);
        currentTime += Time.deltaTime;
        yield return null;
    }

    seedling.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    Destroy(textMeshInstance); // Garante que o TextMesh seja destruído ao final do crescimento
}
    
    private IEnumerator FetchUserData(int userId)
{
    WWWForm form = new WWWForm();
    form.AddField("user_id", userId);

    using (UnityWebRequest www = UnityWebRequest.Post("https://revvopublicidade.com.br/api/get_user_data.php", form))
    {
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<UserResponse>(www.downloadHandler.text);
            if (response.status == "success")
            {
                currentCoins = int.Parse(response.coins);
                currentXP = int.Parse(response.xp);
                UpdateDisplay();
            }
            else
            {
                Debug.LogError("Failed to fetch user data: " + response.message);
            }
        }
        else
        {
            Debug.LogError("Error fetching user data: " + www.error);
        }
    }
}

private IEnumerator WaitForInstantiation(ShopItem item, int construcaoId)
{
    bool instantiated = false;
    GameObject newItem = null;

    while (!instantiated)
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Terra")) 
                {
                    TerraManager terra = hit.collider.GetComponent<TerraManager>();

                    if (terra != null && terra.EstaDisponivel()) 
                    {
                        Vector3 spawnPosition = hit.point;

                        // Instancia o objeto
                        newItem = Instantiate(item.Prefab, spawnPosition, Quaternion.identity);
                        instantiated = true;

                        // Marca o terreno como ocupado
                        terra.OcupaTerreno();

                        Debug.Log($"🌱 Instanciado {item.Name} na posição {spawnPosition}");

                        // Atualiza a posição no banco de dados
                        //StartCoroutine(UpdateItemPositionInDatabase(construcaoId, spawnPosition));
                    }
                    else
                    {
                        Debug.Log("❌ Este terreno já está ocupado! Escolha outro.");
                    }
                }
                else
                {
                    Debug.Log("❌ Clique inválido. O objeto não é um terreno!");
                }
            }
        }
        yield return null;
    }
}

private bool IsPositionOccupied(Vector3 position)
{
     Collider[] colliders = Physics.OverlapSphere(position, checkRadius);

    foreach (Collider col in colliders)
    {
        if (col.gameObject.CompareTag("Terra")) // Se já existe um bloco de terra na posição, retorna verdadeiro
        {
            return true;
        }
    }

    return false; // Posição está livre
}



private IEnumerator UpdateItemPositionInDatabase(int construcaoId, Vector3 position, Vector3 scale)
{
    WWWForm form = new WWWForm();
    int userId = PlayerPrefs.GetInt("user_id");
    form.AddField("user_id", userId);
    form.AddField("construcao_id", construcaoId);
    form.AddField("pos_x", position.x.ToString("F2", CultureInfo.InvariantCulture));
    form.AddField("pos_y", position.y.ToString("F2", CultureInfo.InvariantCulture));
    form.AddField("pos_z", position.z.ToString("F2", CultureInfo.InvariantCulture));
    form.AddField("scale_x", scale.x.ToString("F2", CultureInfo.InvariantCulture));
    form.AddField("scale_y", scale.y.ToString("F2", CultureInfo.InvariantCulture));
    form.AddField("scale_z", scale.z.ToString("F2", CultureInfo.InvariantCulture));

    form.AddField("timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")); // Enviar o timestamp UTC

    Debug.Log($"📡 Enviando para o banco: user_id={userId}, construcao_id={construcaoId}, X={position.x}, Y={position.y}, Z={position.z}, ScaleX={scale.x}, ScaleY={scale.y}, ScaleZ={scale.z}");

    using (UnityWebRequest www = UnityWebRequest.Post("https://revvopublicidade.com.br/api/update_item_position.php", form))
    {
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<ResponseDataConstrucao>(www.downloadHandler.text);
            Debug.Log($"✅ Posição enviada para o banco: {response.message}");
        }
        else
        {
            Debug.LogError($"❌ Erro ao enviar posição: {www.error}");
        }
    }
}

    private Vector3 SnapToGrid(Vector3 position)
    {
        float gridSize = 1.61f;
        
        float snappedX = Mathf.Round(position.x / gridSize) * gridSize;
        float snappedZ = Mathf.Round(position.z / gridSize) * gridSize;
        
        return new Vector3(snappedX, position.y, snappedZ);
    }



    private void InstantiateItem(ShopItem item, int construcaoId)
    {
        StartCoroutine(WaitForInstantiation(item, construcaoId)); // Agora passamos construcaoId corretamente
    }

    private void UpdateDisplay()
    {
        coinsText.text = $"{currentCoins}";
        xpText.text = $"{currentXP}";
    }

     private void PlayAudioCash()
    {
        if (audioSourceCashIn != null && cashInSound != null)
        {
            audioSourceCashIn.PlayOneShot(cashInSound);
        }
    }
     private void PlayAudioPlaced()
    {
        if (audioSourcePlaced != null && placedInSound != null)
        {
            audioSourcePlaced.PlayOneShot(placedInSound);
        }
    }

     public void CloseAviso(){
        espacoDisponivel.SetActive(false);
    }


}
