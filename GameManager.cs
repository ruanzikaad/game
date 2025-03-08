using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Globalization;
using TMPro; // Certifique-se de incluir esta diretiva para usar o TextMesh Pro
[System.Serializable]
public class Construction
{
    public int id;
    public string nome;
    public int nivel_minimo;
    public float preco;
    public string descricao;
    public int xp_recompensa;
    public float tempo_construcao; // tempo necessário para construir em segundos
public string data_inicio_str = "2023-03-01T12:00:00Z";  // Armazena a data como string

 [System.NonSerialized]
    private DateTime _dataInicio;
    public DateTime data_inicio
    {
        get
        {
            if (DateTime.TryParse(data_inicio_str, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out _dataInicio))
            {
                return _dataInicio;
            }
            Debug.LogError($"Falha ao converter a data: {data_inicio_str}");
            return DateTime.MinValue;
        }
    }


    public float pos_x;
    public float pos_y;
    public float pos_z;
    public int id_usuario;
    public int id_construcao;
    public float scale_x, scale_y, scale_z; // Adicionando escala
}

[System.Serializable]
public class ConstructionResponse
{
    public string status;
    public string message;
    public Construction[] data;  
}

public class GameManager : MonoBehaviour
{
    [Header("Lista de Prefabs das Construções")]
    public GameObject cacauSeedlingPrefab, EnxadaBasica;   // Prefab da Muda de Cacau
    public GameObject timerTextPrefab; // Prefab para o TextMesh Pro do timer

    private Dictionary<string, GameObject> constructionPrefabs; // Dicionário para armazenar os prefabs
    private GameObject timerText;
    void Start()
    {
        int userId = PlayerPrefs.GetInt("user_id", -1);  
        if (userId != -1)
        {
            StartCoroutine(GetUserConstructions(userId));
        }
        else
        {
            Debug.LogError("⚠️ User ID not found in PlayerPrefs");
        }

        // Inicializa o dicionário associando os nomes aos prefabs
        constructionPrefabs = new Dictionary<string, GameObject>
        {
            { "Muda de Cacau", cacauSeedlingPrefab },
           { "Enxada Básica", EnxadaBasica }

        };
    }

    IEnumerator GetUserConstructions(int userId)
{
    string uri = $"https://revvopublicidade.com.br/api/get_construction.php?id_usuario={userId}";
    using (UnityWebRequest www = UnityWebRequest.Get(uri))
    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Error fetching constructions: " + www.error);
        }
        else
        {
            Debug.Log("📡 Received data: " + www.downloadHandler.text);
            ConstructionResponse response = JsonUtility.FromJson<ConstructionResponse>(www.downloadHandler.text);

            if (response.status == "success" && response.data != null)
            {
                foreach (var constr in response.data)
                {
                    Debug.Log($"Received construction: {constr.nome} with start date string: {constr.data_inicio_str}");
                    CreateConstruction(constr);
                }
            }
            else
            {
                Debug.LogError("❌ Failed to load constructions: " + response.message);
                Debug.LogError("Data array is null: " + (response.data == null));
            }
        }
    }
}


void CreateConstruction(Construction constr)
{
    Debug.Log($"🏗️ Criando construção: {constr.nome} na posição ({constr.pos_x}, {constr.pos_y}, {constr.pos_z})");

    if (constructionPrefabs.TryGetValue(constr.nome, out GameObject prefab))
    {
        Vector3 position = new Vector3(constr.pos_x, constr.pos_y, constr.pos_z);
        GameObject constructionInstance = Instantiate(prefab, position, Quaternion.identity);

        // Adiciona e configura o componente ConstructionData no objeto instanciado
        ConstructionData constructionData = constructionInstance.AddComponent<ConstructionData>();
        constructionData.id_construcao = constr.id_construcao;

        if (constr.nome.Equals("Muda de Cacau"))
        {
            TimeSpan timeElapsed = DateTime.Now - constr.data_inicio;
            if (timeElapsed.TotalSeconds < constr.tempo_construcao)
            {
                StartGrowth(constr, constructionInstance);
            }
            else
            {
                constructionInstance.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            }
        }
        else
        {
            constructionInstance.transform.localScale = new Vector3(constr.scale_x, constr.scale_y, constr.scale_z);
        }

        Debug.Log($"✅ {constr.nome} instanciada com sucesso com ID {constructionData.id_construcao}!");
    }
    else
    {
        Debug.LogError($"❌ Prefab não encontrado para: {constr.nome}");
    }
}


    void StartGrowth(Construction constr, GameObject constructionInstance)
    {
        DateTime endTime = constr.data_inicio.AddSeconds(90);

        if (DateTime.Now <= endTime)
        {
            // Instancia o objeto do texto do timer no mundo, mas define a posição baseada na muda de cacau
            GameObject timerText = Instantiate(timerTextPrefab, constructionInstance.transform.position + Vector3.up * 2, Quaternion.identity);
            TMPro.TextMeshPro textMeshPro = timerText.GetComponent<TMPro.TextMeshPro>();

            // Mantém o texto independente da construção
            timerText.transform.SetParent(null);

            StartCoroutine(GrowConstruction(constr, constructionInstance, textMeshPro, timerText));
        }
        else
        {
            // Se o tempo já expirou, a construção já nasce na escala final
            constructionInstance.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
    }

    IEnumerator GrowConstruction(Construction constr, GameObject constructionInstance, TMPro.TextMeshPro textMeshPro, GameObject timerText)
    {
        DateTime endTime = constr.data_inicio.AddSeconds(90);

        while (DateTime.Now < endTime)
        {
            float remainingTime = (float)(endTime - DateTime.Now).TotalSeconds;
            float progress = 1.0f - remainingTime / 90.0f;
            constructionInstance.transform.localScale = Vector3.one * Mathf.Lerp(0.1f, 1.1f, progress);

            if (textMeshPro != null)
            {
                textMeshPro.text = $"{remainingTime:F0}s";
            }

            // Mantém o texto posicionado acima da construção
            if (timerText != null)
            {
                timerText.transform.position = constructionInstance.transform.position + Vector3.up * 2;
            }

            yield return null;
        }

        // Garante que a construção tenha a escala final
        constructionInstance.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

        // Destrói o texto ao final do crescimento
        if (timerText != null)
        {
            Destroy(timerText);
        }
    }

}