using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _promptText;

    internal void UpdateText(string _promptMessage) => _promptText.text = _promptMessage;
}