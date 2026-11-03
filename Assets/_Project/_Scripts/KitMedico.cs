using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitMedico : MonoBehaviour
{
    private int tempoNoMapa = 5;
    private int quantidadeDeCura = 15;

    private void Start()
    {
        Destroy(gameObject, tempoNoMapa);
    }
    public void OnTriggerEnter(Collider objetoDeColisao)
    {
        if (objetoDeColisao.CompareTag("Player"))
        {
            Destroy(gameObject);
            objetoDeColisao.GetComponent<PlayerController>().Heal(quantidadeDeCura);
        }
    }
}
