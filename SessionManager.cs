using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class SessionManager : MonoBehaviour
{
    private string loginUrl = "https://highticket.bet/api/login.php"; // API de autenticação
    public TMP_Text messageText; // Exibe erros, se houver

    void Start()
    {
        // Verifica se há uma sessão salva
        if (PlayerPrefs.HasKey("username") && PlayerPrefs.HasKey("password"))
        {
            string username = PlayerPrefs.GetString("username");
            string password = PlayerPrefs.GetString("password");

            // Tenta autenticar automaticamente
            StartCoroutine(AutoLogin(username, password));
        }
    }

    IEnumerator AutoLogin(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(loginUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                LoginResponseData response = JsonUtility.FromJson<LoginResponseData>(jsonResponse);

                if (response.status == "success")
                {
                    SceneManager.LoadScene("MainScene"); // ✅ Login bem-sucedido, carrega a fazenda
                }
                else
                {
                    messageText.text = "Erro: " + response.message; // Exibe erro
                }
            }
            else
            {
                messageText.text = "Erro: " + www.error; // Exibe erro de conexão
            }
        }
    }
}

// ✅ Classe auxiliar para ler JSON
[System.Serializable]
public class LoginResponseData
{
    public string status;
    public string message;
}
