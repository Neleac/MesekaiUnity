using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class WebView: MonoBehaviour
{
    private const string GlbExtension = ".glb";
    private const string DataUrlFieldName = "url";
    private const string AvatarExportEventName = "v1.avatar.exported";

    private WebViewWindowBase webViewObject = null;

    [SerializeField] private MessagePanel messagePanel = null;
    
    [Header("Padding")]
    [SerializeField] private int left;
    [SerializeField] private int top;
    [SerializeField] private int right;
    [SerializeField] private int bottom;
      
    public bool Loaded { get; private set; }
    
    // Event to call when avatar is created, receives GLB url.
    public Action<string> OnAvatarCreated;

    /// <summary>
    ///     Create WebView object attached to a MonoBehaviour object
    /// </summary>
    public void CreateWebView()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            messagePanel.SetMessage(MessagePanel.MessageType.NetworkError);
            messagePanel.SetVisible(true);
        }
        else
        {
            #if UNITY_EDITOR || !(UNITY_ANDROID || UNITY_IOS)
                messagePanel.SetMessage(MessagePanel.MessageType.NotSupported);
                messagePanel.SetVisible(true);
            #else
                if (webViewObject == null)
                {
                    messagePanel.SetMessage(MessagePanel.MessageType.Loading);
                    messagePanel.SetVisible(true);

                    #if UNITY_ANDROID
                        webViewObject = gameObject.AddComponent<AndroidWebViewWindow>();
                    #elif UNITY_IOS
                        webViewObject = gameObject.AddComponent<IOSWebViewWindow>();
                    #endif
                }

                webViewObject.OnLoaded = OnLoaded;
                webViewObject.OnJS = OnWebMessageReceived;

                WebViewOptions options = new WebViewOptions();
                webViewObject.Init(options);

                PartnerSO partner = Resources.Load<PartnerSO>("Partner");
                string url = partner.GetUrl();
                webViewObject.LoadURL(url);
            #endif
        }
        
        SetScreenPadding(left, top, right, bottom);
    }

    /// <summary>
    ///     Set WebView screen padding in pixels.
    /// </summary>
    public void SetScreenPadding(int left, int top, int right, int bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;

        if (webViewObject)
        {
            webViewObject.SetMargins(left, top, right, bottom);
        }

        messagePanel.SetMargins(left, top, right, bottom);
    }

    /// <summary>
    ///     Set WebView window visible, shows or hides both message window and WebView
    /// </summary>
    /// <param name="visible">Visibility of the <see cref="webViewObject"/> or <see cref="messagePanel"/></param>
    public void SetVisible(bool visible)
    {
        if (webViewObject)
        {
            webViewObject.IsVisible = visible;
            messagePanel.SetVisible(visible);
        }
    }

    private void OnWebMessageReceived(string message)
    {
        Debug.Log($"--- WebView Message: { message }");

        try
        {
            WebMessage webMessage = JsonConvert.DeserializeObject<WebMessage>(message);

            if(webMessage.eventName == AvatarExportEventName)
            {
                if (webMessage.data.TryGetValue(DataUrlFieldName, out string avatarUrl))
                {
                    webViewObject.IsVisible = false;
                    OnAvatarCreated?.Invoke(avatarUrl);
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log($"--- Message is not JSON: { message }\nError Message: { e.Message }");
        }
    }

    private void OnLoaded(string message)
    {
        if (Loaded) return;
        
        Debug.Log("--- WebView Loaded.");

        webViewObject.EvaluateJS(@"
            document.cookie = 'webview=true';

            if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                window.Unity = {
                    call: function(msg) { 
                        window.webkit.messageHandlers.unityControl.postMessage(msg); 
                    }
                }
            }
            else {
                window.Unity = {
                    call: function(msg) {
                        window.location = 'unity:' + msg;
                    }
                }
            }

            function subscribe(event) {
                const json = parse(event);
                const source = json.source;
                    
                if (source !== 'readyplayerme') {
                    return;
                }

			    Unity.call(event.data);
		    }

            function parse(event) {
                try {
                    return JSON.parse(event.data);
                } catch (error) {
                    return null;
                }
            }

            window.postMessage(
                JSON.stringify({
                    target: 'readyplayerme',
                    type: 'subscribe',
                    eventName: 'v1.**'
                }),
                '*'
            );

            window.removeEventListener('message', subscribe);
            window.addEventListener('message', subscribe)
        ");

        Loaded = true;

        // Tasks break WebView, used invoke instead.
        Invoke(nameof(DisplayWebView), 1f);
    }

    private void DisplayWebView() => webViewObject.IsVisible = true;

    private void OnDrawGizmos()
    {
        RectTransform rectTransform = transform as RectTransform;
        Gizmos.matrix = rectTransform.localToWorldMatrix;

        Gizmos.color = Color.green;
        Vector3 center = new Vector3((left - right) / 2, (bottom - top) / 2);
        Vector3 size = new Vector3(rectTransform.rect.width - (left + right), rectTransform.rect.height - (bottom + top));

        Gizmos.DrawWireCube(center, size);
    }
}

public struct WebMessage
{
    public string type;
    public string source;
    public string eventName;
    public Dictionary<string, string> data;
}
