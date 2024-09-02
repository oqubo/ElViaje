using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum TipoDeEfecto { Distorsion, Palpitar }

public class Efectos : MonoBehaviour
{
    public GameObject objetivo;
    public TipoDeEfecto efecto = TipoDeEfecto.Distorsion;

    void Start()
    {

        if (PlayerPrefs.GetInt("amigo")==0)
        {
            switch (efecto)
            {
                case TipoDeEfecto.Distorsion:
                    InvokeRepeating("distorsionAleatoria", 2f, 2f);
                    break;
                case TipoDeEfecto.Palpitar:
                    InvokeRepeating("palpitar", 3f, 3f);
                    break;
            }
        }

    }

    public void distorsionAleatoria() {
        objetivo.transform.DOShakeScale(0.4f,0.5f,5);
    }

    public void palpitar() {
        objetivo.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 1f);
    }
}
