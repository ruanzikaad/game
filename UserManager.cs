using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // Não esqueça de adicionar esta linha para acessar a classe TMP_Text
using MyGame.Models;

public class UserManager : MonoBehaviour
{
    public int levelAtual;
    public int xpAtual;
    public int coinsAtual;
    public int cashAtual;

    // Adicione referências para os elementos TMP_Text
    public TMP_Text levelText;
    public TMP_Text xpText;
    public TMP_Text coinsText;
    public TMP_Text cashText;

    //visibilidade do painel das contruções por level
    public GameObject blocked_item1;
    public GameObject blocked_item2;
    public GameObject blocked_item3;
    public GameObject blocked_item4;
    public GameObject blocked_item5;

    public GameObject blocked_item_ferramentas1,blocked_item_ferramentas2,blocked_item_ferramentas3,blocked_item_ferramentas4,blocked_item_ferramentas5;
    public GameObject blocked_item_recursos2,blocked_item_recursos3,blocked_item_recursos4,blocked_item_recursos5;


    public GameObject panelTutorial;
    
 
    void Awake()
    {
        int userId = PlayerPrefs.GetInt("user_id", -1);
        if (userId != -1) {
            StartCoroutine(GetUserData(userId));
        } else {
            Debug.LogError("User ID not found in PlayerPrefs");
        }
    }

    IEnumerator GetUserData(int userId)
    {
        string uri = $"https://revvopublicidade.com.br/api/get_player_info.php?id={userId}";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch user data: " + www.error);
            }
            else
            {
                UserResponse response = JsonUtility.FromJson<UserResponse>(www.downloadHandler.text);
                if (response.status == "success")
                {
                    // Armazene os dados diretamente nas variáveis
                    levelAtual = response.data.level;
                    xpAtual = response.data.xp;
                    coinsAtual = (int)response.data.coins; // Cast para int se necessário
                    cashAtual = (int)response.data.cash; // Cast para int se necessário

                    // Atualize os textos na UI
                    UpdateUI();
                    UpdateItemVisibility(levelAtual);

                    Debug.Log("User data loaded and saved successfully.");
                }
                else
                {
                    Debug.LogError("Failed to load user data: " + response.message);
                }
            }
        }
    }

     public void UpdateItemVisibility(int currentLevel)
    {
         if (currentLevel == 0)
        {
            panelTutorial.SetActive(true);  // Desativa o item 1 se o nível for 2 ou maior
        }

        if (currentLevel >= 2)
        {
            blocked_item1.SetActive(false);  // Desativa o item 1 se o nível for 2 ou maior
            blocked_item_ferramentas1.SetActive(false);
        }
        if (currentLevel >= 3)
        {
            blocked_item2.SetActive(false);  // Desativa o item 2 se o nível for 3 ou maior
            blocked_item_ferramentas2.SetActive(false);

        }
        if (currentLevel >= 5)
        {
            blocked_item3.SetActive(false);  // Desativa o item 3 se o nível for 5 ou maior
            blocked_item_ferramentas3.SetActive(false);
            blocked_item_recursos2.SetActive(false);
        }
        if (currentLevel >= 7)
        {
            blocked_item4.SetActive(false);  // Desativa o item 4 se o nível for 7 ou maior
            blocked_item_ferramentas4.SetActive(false);
            blocked_item_recursos3.SetActive(false);
            blocked_item_recursos4.SetActive(false);


        }
        if (currentLevel >= 10)
        {
            blocked_item5.SetActive(false);  // Desativa o item 5 se o nível for 10 ou maior
           blocked_item_ferramentas5.SetActive(false);
           blocked_item_recursos5.SetActive(false);

        }
    }


    private void UpdateUI()
    {
        levelText.text = levelAtual.ToString();
        xpText.text = xpAtual.ToString();
        coinsText.text = coinsAtual.ToString();
        cashText.text = cashAtual.ToString();
    }

     
}
