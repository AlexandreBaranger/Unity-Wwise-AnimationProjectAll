using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WwisePlayEventInspector : MonoBehaviour
{
    [SerializeField] // Permet à la variable d'être visible dans l'inspecteur Unity
    private bool playEventAutomatically = false; // Ajoutez une variable booléenne pour contrôler si l'événement doit être déclenché automatiquement

    public AK.Wwise.Event EventPlay;

    private void Start()
    {
        if (playEventAutomatically)
        {
            EventLaunch(); // Déclencher l'événement automatiquement au démarrage si playEventAutomatically est true
        }
    }

    private void EventLaunch()
    {
        EventPlay.Post(gameObject); // Utiliser Post() pour déclencher l'événement
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (playEventAutomatically)
        {
            EventLaunch(); // Déclencher l'événement lorsqu'il y a des changements dans l'inspecteur
        }
    }
#endif
}
