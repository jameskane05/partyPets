using UnityEngine;
using UnityEngine.EventSystems;
using SpeechRecognition;

public class SpeechButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public WolfCtrl wolfCtrl;
    public GameObject loadingIndicator;

    public void OnPointerDown(PointerEventData eventData)
    {
        SpeechToText.instance.StartRecording();
        loadingIndicator.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SpeechToText.instance.StopRecording();
        loadingIndicator.SetActive(false);
    }
}
