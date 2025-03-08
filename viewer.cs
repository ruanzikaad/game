using UnityEngine;

public class viewer : MonoBehaviour
{
    void Start()
    {
        // Exemplo de como logar valores específicos
        Debug.Log("PlayerPrefs - Username: " + PlayerPrefs.GetString("username", "Não definido"));
        Debug.Log("PlayerPrefs - Password: " + PlayerPrefs.GetString("password", "Não definido"));

        // Se você quiser verificar se certas chaves existem e logá-las
        LogPlayerPrefsValue("username");
        LogPlayerPrefsValue("password");
    }

    // Método para logar o valor de uma chave específica
    void LogPlayerPrefsValue(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string value = PlayerPrefs.GetString(key);
            Debug.Log("PlayerPrefs - " + key + ": " + value);
        }
        else
        {
            Debug.Log("PlayerPrefs - " + key + " não está definido.");
        }
    }
}
