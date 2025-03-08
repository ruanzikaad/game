using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using MyGame.Models;

public class TutorialManager : MonoBehaviour
{
    public Animator animBtnShop;
    public TMP_Text coinsText;
    public TMP_Text levelText;

    public GameObject panelTutorial;
    public GameObject tutorialStep1;
    public GameObject tutorialStep2;
    public GameObject tutorialStep3;

    public AudioSource uiAudioSource, uiAudioSourceNextStep;
    public AudioClip clickSound;
    public AudioClip clickSoundNextStep;

    public int currentStep = 0;
    public int currentLevel = 0;
    public int currentCoins = 0; // Now managed directly from the server
    public bool canDoTutorial = false;

    private void Start()
    {
        int userId = PlayerPrefs.GetInt("user_id", -1);
        if (userId != -1) {
            StartCoroutine(FetchUserData(userId));
        } else {
            Debug.LogError("User ID not found in PlayerPrefs");
        }
    }

    private IEnumerator FetchUserData(int userId)
    {
        string url = "https://revvopublicidade.com.br/api/get_user_data.php"; // Assumes endpoint returns both level and coins
        WWWForm form = new WWWForm();
        form.AddField("user_id", userId);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                UserResponse response = JsonUtility.FromJson<UserResponse>(www.downloadHandler.text);
                if (response.status == "success")
                {
                    currentLevel = int.Parse(response.level);
                    currentCoins = int.Parse(response.coins);
                    UpdateDisplay();
                    DetermineTutorialVisibility();
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

 private void Update()
    {
        if (canDoTutorial && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                HandleTutorialProgression();
                PlayAudioNext();
            }
        }
    }

    private void UpdateDisplay()
    {
        levelText.text = currentLevel.ToString();
        coinsText.text = currentCoins.ToString();
    }

    private void DetermineTutorialVisibility()
    {
        canDoTutorial = currentLevel == 0;
        panelTutorial.SetActive(canDoTutorial);
    }

    private void HandleTutorialProgression()
    {
        currentStep++;
        switch (currentStep)
        {
            case 1:
                tutorialStep1.SetActive(false);
                tutorialStep2.SetActive(true);
                break;
            case 2:
                tutorialStep2.SetActive(false);
                tutorialStep3.SetActive(true);
                PlayAudio();
                break;
            case 3:
                tutorialStep3.SetActive(false);
                panelTutorial.SetActive(false);
                animBtnShop.SetBool("scaleIn", true);
                CompleteTutorial();
                break;
        }
        PlayAudioNext();
    }

    private void CompleteTutorial()
    {
        canDoTutorial = false;
        currentStep = 0;
        UpdateUserCoins(500); // Increment coins by 500
 
    }

    private void UpdateUserCoins(int coinsToAdd)
    {
        int userId = PlayerPrefs.GetInt("user_id", -1);
        if (userId != -1) {
            StartCoroutine(UpdateUserCoinsCoroutine(userId, coinsToAdd));
        } else {
            Debug.LogError("User ID not found for updating coins");
        }
    }

    private IEnumerator UpdateUserCoinsCoroutine(int userId, int coinsToAdd)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", userId);
        form.AddField("coins_to_add", coinsToAdd);
        using (UnityWebRequest www = UnityWebRequest.Post("https://revvopublicidade.com.br/api/update_coins.php", form))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                UserResponse response = JsonUtility.FromJson<UserResponse>(www.downloadHandler.text);
                if (response.status == "success")
                {
                    currentCoins += coinsToAdd; // Update locally only after confirmation from the server
                    currentLevel += 1;  // Assume level increments with each coin update or based on specific conditions

                    UpdateDisplay();

                }
                else
                {
                    Debug.LogError("Failed to update coins: " + response.message);
                }
            }
            else
            {
                Debug.LogError("Failed to update coins: " + www.error);
            }
        }
    }

    private void PlayAudio()
    {
        if (uiAudioSource != null && clickSound != null)
        {
            uiAudioSource.PlayOneShot(clickSound);
        }
    }

    private void PlayAudioNext()
    {
        if (uiAudioSourceNextStep != null && clickSoundNextStep != null)
        {
            uiAudioSourceNextStep.PlayOneShot(clickSoundNextStep);
        }
    }
}
