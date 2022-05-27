using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class WebViewTest : MonoBehaviour
    {
        private GameObject avatar;

        [SerializeField] private WebView webView;
        [SerializeField] private GameObject loadingLabel = null;
        [SerializeField] private Button displayButton = null;
        [SerializeField] private Button closeButton = null;

        private void Start()
        {
            displayButton.onClick.AddListener(DisplayWebView);
            closeButton.onClick.AddListener(HideWebView);
        }

        // Display WebView or create it if not initialized yet 
        private void DisplayWebView()
        {
            if(webView == null)
            {
                webView = FindObjectOfType<WebView>();
            }
            
            if (webView.Loaded)
            {
                webView.SetVisible(true);
            }
            else
            {
                webView.CreateWebView();
                webView.OnAvatarCreated = OnAvatarCreated;
            }

            closeButton.gameObject.SetActive(true);
            displayButton.gameObject.SetActive(false);
        }

        private void HideWebView()  {
            webView.SetVisible(false);
            closeButton.gameObject.SetActive(false);
            displayButton.gameObject.SetActive(true);
        }

        // WebView callback for retrieving avatar url
        private void OnAvatarCreated(string url)
        {
            if (avatar) Destroy(avatar);

            webView.SetVisible(false);
            loadingLabel.SetActive(true);
            displayButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(false);

            AvatarLoader avatarLoader = new AvatarLoader();
            avatarLoader.LoadAvatar(url, null, OnAvatarImported);
        }

        // AvatarLoader callback for retrieving loaded avatar game object
        private void OnAvatarImported(GameObject avatar, AvatarMetaData metaData)
        {
            this.avatar = avatar;
            loadingLabel.SetActive(false);
            displayButton.gameObject.SetActive(true);

            Debug.Log("Loaded");
        }

        private void Destroy()
        {
            displayButton.onClick.RemoveListener(DisplayWebView);
            closeButton.onClick.RemoveListener(HideWebView);
        }
    }
}
