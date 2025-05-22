using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using System.Collections.Generic;

public class ChatManager : NetworkBehaviour
{
    [Header("UI References")]
    public RectTransform chatContent;
    public GameObject messagePrefab;
    public TMP_InputField inputField;
    public Button sendButton;

    private void Start()
    {
        sendButton.onClick.AddListener(OnSendClicked);
    }

    private void OnSendClicked()
    {
        string text = inputField.text.Trim();
        if (string.IsNullOrEmpty(text)) return;

        // отправляем на сервер
        SubmitMessageServerRpc(text);
        inputField.text = "";
    }

    [ServerRpc(RequireOwnership = false)]
    private void SubmitMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        // формируем имя отправителя
        ulong clientId = rpcParams.Receive.SenderClientId;
        string sender = "Player " + clientId;

        // рассылаем всем
        BroadcastMessageClientRpc(sender, message);
    }

    [ClientRpc]
    private void BroadcastMessageClientRpc(string sender, string message)
    {
        // создаём UI-элемент
        GameObject go = Instantiate(messagePrefab, chatContent);
        var textComp = go.GetComponent<TMP_Text>();
        textComp.text = $"[{sender}]: {message}";

        // прокрутка вниз
        Canvas.ForceUpdateCanvases();
        var scroll = chatContent.GetComponentInParent<ScrollRect>();
        scroll.verticalNormalizedPosition = 0f;
    }
}
