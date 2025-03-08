using UnityEngine;
using UnityEngine.UI;

public class MusicToggle : MonoBehaviour
{
    // Referências aos componentes
    public AudioSource soundtrack; // O componente AudioSource do soundtrack
    public Image musicIcon; // O Image que exibe o ícone de música
    public Sprite musicOnSprite; // O ícone de música ligada
    public Sprite musicOffSprite; // O ícone de música desligada
    
    private bool isMusicOn = true; // Variável para verificar o estado da música
    private RectTransform iconRectTransform; // RectTransform do ícone para detectar toques/cliques

    void Start()
    {
        // Pega o RectTransform do ícone de música
        iconRectTransform = musicIcon.GetComponent<RectTransform>();
        
        // Configura o estado inicial
        if (soundtrack.volume == 0) // Se o volume inicial for 0, significa que a música está desligada
        {
            isMusicOn = false;
            musicIcon.sprite = musicOffSprite; // Exibe o ícone de música desligada
        }
        else
        {
            isMusicOn = true;
            musicIcon.sprite = musicOnSprite; // Exibe o ícone de música ligada
        }
    }

    void Update()
    {
        // Verifica se o jogo está sendo executado em um dispositivo mobile ou PC
        if (Application.isMobilePlatform)
        {
            // Detecta toques na tela
            if (Input.touchCount > 0)
            {
                // Obtém o primeiro toque
                Touch touch = Input.GetTouch(0);

                // Verifica se o toque está dentro da área do ícone de música
                if (IsTouchingIcon(touch.position))
                {
                    ToggleMusic();
                }
            }
        }
        else
        {
            // Detecta clique no PC
            if (Input.GetMouseButtonDown(0))
            {
                // Obtém a posição do clique
                Vector2 mousePosition = Input.mousePosition;

                // Verifica se o clique está dentro da área do ícone de música
                if (IsTouchingIcon(mousePosition))
                {
                    ToggleMusic();
                }
            }
        }
    }

    // Verifica se o toque ou clique está dentro da área do ícone
    private bool IsTouchingIcon(Vector2 position)
    {
        // Verifica se a posição do toque ou clique está dentro da área do ícone de música
        return RectTransformUtility.RectangleContainsScreenPoint(iconRectTransform, position, Camera.main);
    }

    // Método que será chamado para alternar a música e o sprite
    private void ToggleMusic()
    {
        if (isMusicOn)
        {
            // Se a música estiver ligada, abaixa o volume para 0 e muda o ícone
            soundtrack.volume = 0f;
            musicIcon.sprite = musicOffSprite;
            isMusicOn = false;
        }
        else
        {
            // Se a música estiver desligada, aumenta o volume para 0.6 e muda o ícone
            soundtrack.volume = 0.1f;
            musicIcon.sprite = musicOnSprite;
            isMusicOn = true;
        }
    }
}
