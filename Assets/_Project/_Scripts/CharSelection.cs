using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelection : MonoBehaviour
{
    public GameObject[] PlayerSkins;
    private int Index;


    private void Start()
    {
        //Só os filhos com modelo (SkinnedMeshRenderer) são personagens. Os demais (esqueleto com a arma,
        //cano da arma, canvas de vida) precisam continuar ativos e não podem entrar na seleção.
        var skins = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.GetComponentInChildren<SkinnedMeshRenderer>(true) != null)
                skins.Add(child.gameObject);
        }
        PlayerSkins = skins.ToArray();

        if (PlayerSkins.Length == 0) return;

        Index = Mathf.Clamp(PlayerPrefs.GetInt("PlayerSelected"), 0, PlayerSkins.Length - 1);

        //Desligando a renderização dos modelos.
        foreach (GameObject go in PlayerSkins)
            go.SetActive(false);

        //Ligando o Index escolhido
        PlayerSkins[Index].SetActive(true);
    }

    public void Voltar()
    {
        //Desativar o modelo atual
        PlayerSkins[Index].SetActive(false);

        Index--;
        if (Index < 0)
            Index = PlayerSkins.Length - 1;

        //Ativar o novo modelo
        PlayerSkins[Index].SetActive(true);
    }

    public void Proximo()
    {
        //Desativar o modelo atual
        PlayerSkins[Index].SetActive(false);

        Index++;
        if (Index == PlayerSkins.Length)
            Index = 0;

        //Ativar o novo modelo
        PlayerSkins[Index].SetActive(true);
    }

    public void Confirmar()
    {
        PlayerPrefs.SetInt("PlayerSelected", Index);
    }


}
