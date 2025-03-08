using UnityEngine;
using UnityEngine.UI;

public class ShopMenuManager : MonoBehaviour
{
    public Animator btnShopAnimator;
    [Header("Painéis do Menu")]
    public GameObject shopPanel;
    public GameObject constructionsPanel;
    public GameObject toolsPanel;
    public GameObject seedlingsPanel;

    public GameObject objectivesPanel;  // Painel de objetivos

    [Header("Botões do Menu")]
    public Button shopButton;
    public Button constructionsButton;

    public Button objetivosButton;
    public Button toolsButton;
    public Button seedlingsButton;
    public Button closeShopButton, closeConstructionButton, closeToolsButton, closeSeedlingsButton, closeObjectivesButton;

    public Button backFromConstruction, backFromTools, backFromResources;

    [Header("Som de UI")]
    public AudioSource uiAudioSource;
    public AudioClip clickSound;

   private void CloseCategory(GameObject categoryPanel)
    {
        PlayClickSound();

        // Fecha apenas o painel ativo sem reabrir a loja principal
        categoryPanel.SetActive(false);
    }

    private void Start()
    {
        // Garante que apenas o painel da loja está fechado ao iniciar
        shopPanel.SetActive(false);
        constructionsPanel.SetActive(false);
        toolsPanel.SetActive(false);
        seedlingsPanel.SetActive(false);

        // Adiciona eventos aos botões da loja
        shopButton.onClick.AddListener(ToggleShopPanel);
        constructionsButton.onClick.AddListener(() => OpenCategory(constructionsPanel));
        toolsButton.onClick.AddListener(() => OpenCategory(toolsPanel));
        seedlingsButton.onClick.AddListener(() => OpenCategory(seedlingsPanel));
        closeShopButton.onClick.AddListener(CloseShopPanel);

        // Adiciona eventos aos botões de voltar
        backFromConstruction.onClick.AddListener(() => CloseCategoryAndReturnToShop(constructionsPanel));
        backFromTools.onClick.AddListener(() => CloseCategoryAndReturnToShop(toolsPanel));
        backFromResources.onClick.AddListener(() => CloseCategoryAndReturnToShop(seedlingsPanel));

        // Adiciona eventos aos botões de fechar categorias
        closeConstructionButton.onClick.AddListener(() => CloseCategory(constructionsPanel));
        closeToolsButton.onClick.AddListener(() => CloseCategory(toolsPanel));
        closeSeedlingsButton.onClick.AddListener(() => CloseCategory(seedlingsPanel));

        closeObjectivesButton.onClick.AddListener(CloseObjectivesPanel);  // Vincula o botão de fechar objetivos



    }

    public void ToggleObjectivesPanel()
    {
        PlayClickSound();
        objectivesPanel.SetActive(!objectivesPanel.activeSelf); // Alterna a visibilidade do painel de objetivos
    }


      public void CloseObjectivesPanel()
    {
        PlayClickSound();
        objectivesPanel.SetActive(false); // Fecha o painel de objetivos
    }


    private void ToggleShopPanel()
    {
        PlayClickSound();
        shopPanel.SetActive(!shopPanel.activeSelf); // Alterna a visibilidade
        btnShopAnimator.SetBool("scaleIn", false);
        
    }

    private void OpenCategory(GameObject categoryPanel)
    {
        PlayClickSound();
        CloseShopPanel();
        // Fecha todas as categorias antes de abrir a selecionada
        constructionsPanel.SetActive(false);
        toolsPanel.SetActive(false);
        seedlingsPanel.SetActive(false);

        // Abre a categoria selecionada
        categoryPanel.SetActive(true);
        shopPanel.SetActive(false); // Esconde a loja principal
    }

    private void CloseCategoryAndReturnToShop(GameObject categoryPanel)
    {
        PlayClickSound();

        // Fecha o painel ativo e retorna para a loja principal
        categoryPanel.SetActive(false);
        shopPanel.SetActive(true);
    }

    private void CloseShopPanel()
    {
        PlayClickSound();
        shopPanel.SetActive(false);
    }

    private void PlayClickSound()
    {
        if (uiAudioSource != null && clickSound != null)
        {
            uiAudioSource.PlayOneShot(clickSound);
        }
    }
}
