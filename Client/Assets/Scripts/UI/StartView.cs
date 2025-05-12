using Entity.Response;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class StartView : MonoBehaviour
    {
        public Button button;
        public TMP_InputField usernameInput;
        public TMP_InputField addressInput;
        private NetworkManager _network;

        private void Start()
        {
            _network = NetworkManager.Instance;
            button.onClick.AddListener(OnButtonClick);
            usernameInput.text = PlayerPrefs.GetString("clientId", "");
            addressInput.text = PlayerPrefs.GetString("address", "");
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }

        private async void OnButtonClick()
        {
            var username = usernameInput.text;
            var address = addressInput.text;
            if (username.Length == 0 || address.Length == 0) return;
            if (!NetworkManager.IsValidAddress(address)) return;
            _network.OnStatusResponded += OnStatusResponded;
            _network.Initialize(address);
            var result = await _network.CheckStatus();
            if (result) return;
            _network.CleanUp();
            _network.OnStatusResponded -= OnStatusResponded;
        }

        private void OnStatusResponded(StatusResponse response)
        {
            _network.OnStatusResponded -= OnStatusResponded;
            if (response.Status != "ok")
            {
                _network.CleanUp();
                return;
            }
            LoadMain(usernameInput.text, addressInput.text);
        }

        private static void LoadMain(string username, string address) {
            PlayerPrefs.SetString("clientId", username);
            PlayerPrefs.SetString("address", address);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Main");
        }
    }
}