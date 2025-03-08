using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System.Text;

[System.Serializable]
public class User
{
    public string username;
    public string password;
    public int user_id;

    public User(string username, string password, int id)
    {
        this.username = username;
        this.password = password;
        this.user_id = id;
    }
}

public class RegisterManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject loginModalPanel, entrarModalPanel;
    public GameObject entrarModal;
    public TMP_InputField entrar_usernameInput, login_usernameInput;
    public TMP_InputField entrar_passwordInput, login_passwordInput;
    public TMP_Text entrar_messageText, login_messageText;
    public GameObject loadingScreen;
    public TMP_InputField farmNameInput;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;

    private string registerUrl = "https://revvopublicidade.com.br/api/register.php";

    public async void RegisterUser()
    {
        StartCoroutine(RegisterCoroutine());
    }

    public void openModalEntrar()
    {
        mainMenuPanel.SetActive(false);
        entrarModal.SetActive(true);
    }

    public void LoginUser()
{
    string username = entrar_usernameInput.text;
    string password = entrar_passwordInput.text;

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        entrar_messageText.text = "Por favor, preencha todos os campos.";
        return;
    }

    StartCoroutine(LoginCoroutine(username, password));
}

IEnumerator LoginCoroutine(string username, string password)
{
    string loginUrl = "https://revvopublicidade.com.br/api/login.php";
    User user = new User(username, password, -1);
    string jsonData = JsonUtility.ToJson(user);

    using (UnityWebRequest www = UnityWebRequest.PostWwwForm(loginUrl, "POST"))
    {
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            Debug.Log("Response: " + jsonResponse);
            ResponseData response = JsonUtility.FromJson<ResponseData>(jsonResponse);

            if (response.status == "success")
            {
                PlayerPrefs.SetInt("user_id", response.user_id);
                PlayerPrefs.SetString("username", username);
                PlayerPrefs.SetString("password", password);
                PlayerPrefs.Save();
                StartCoroutine(LoadSceneAsync("MainScene"));
            }
            else
            {
                entrar_messageText.text = "Login falhou: " + response.message;
            }
        }
        else
        {
            Debug.LogError("Error: " + www.error);
            entrar_messageText.text = "Erro de conexão.";
        }
    }
}




    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("username") && PlayerPrefs.HasKey("password"))
        {
            string username = PlayerPrefs.GetString("username");
            string password = PlayerPrefs.GetString("password");
            StartCoroutine(AutoLogin(username, password));
        }
        else
        {
            messageText.text = "Nenhuma sessão salva. Faça login primeiro.";
        }
    }

    IEnumerator AutoLogin(string username, string password)
    {
        string loginUrl = "https://revvopublicidade.com.br/api/login.php";
        User user = new User(username, password, -1);
        string jsonData = JsonUtility.ToJson(user);
        Debug.Log("Sending JSON: " + jsonData);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(loginUrl, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);
                ResponseData response = JsonUtility.FromJson<ResponseData>(jsonResponse);

                if (response.status == "success")
                {
                    Debug.Log("Login successful: " + response.message);
                    PlayerPrefs.SetInt("user_id", response.user_id);
                    StartCoroutine(LoadSceneAsync("MainScene"));
                }
                else
                {
                    Debug.LogError("Login failed: " + response.message);
                }
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    public void OpenLoginModal()
    {
        mainMenuPanel.SetActive(false);
        loginModalPanel.SetActive(true);
    }

    public void CloseLoginModal()
    {
        loginModalPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        entrarModal.SetActive(false);
    }

    IEnumerator RegisterCoroutine()
    {
        string jsonData = "{\"farm_name\":\"" + farmNameInput.text + "\",\"username\":\"" + usernameInput.text + "\",\"password\":\"" + passwordInput.text + "\"}";
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(registerUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log(jsonResponse);
                ResponseData response = JsonUtility.FromJson<ResponseData>(jsonResponse);
                messageText.text = response.message;

                if (response.status == "success")
                {
                    PlayerPrefs.SetInt("user_id", response.user_id);
                    PlayerPrefs.SetString("username", usernameInput.text);
                    PlayerPrefs.SetString("password", passwordInput.text);
                    PlayerPrefs.Save();
                    StartCoroutine(LoadSceneAsync("MainScene"));
                }
            }
            else
            {
                messageText.text = "Error: " + www.error;
            }
        }
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetActive(true);
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}

 


[System.Serializable]
public class ResponseData
{
    public string status;
    public string message;
    public int user_id;
}
