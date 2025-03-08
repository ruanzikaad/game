using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelController : MonoBehaviour
{
    // Referências para os painéis
    public GameObject panelCacauRaro;
    public GameObject panelCacauComum;
    public GameObject panelCacauHibrido;

    // Função para mostrar painel
    public void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    // Função para esconder painel
    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
    }
}
