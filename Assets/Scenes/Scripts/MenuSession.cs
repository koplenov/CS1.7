using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSession : MonoBehaviour
{
    [SerializeField] private TMP_InputField nick;
    [SerializeField] private TMP_InputField ip;
    [SerializeField] private Toggle serverToggle;
    
    private void Start()
    {
        serverToggle.onValueChanged.AddListener(delegate {
            ServerToggle(serverToggle);
        });
        
        ((TextMeshProUGUI) nick.placeholder).text = PlayerPrefs.GetString("nick", Environment.UserName);

#if UNITY_EDITOR
        ((TextMeshProUGUI) ip.placeholder).text = PlayerPrefs.GetString("ip", "127.0.0.1");
#else
        ((TextMeshProUGUI) ip.placeholder).text = PlayerPrefs.GetString("ip", "26.126.242.66");
#endif
    }

    public void Play()
    {
        if (nick.text.Trim() != String.Empty)
        {
            PlayerPrefs.SetString("nick", nick.text.Trim());
        }
        if (ip.text.Trim() != String.Empty)
        {
            PlayerPrefs.SetString("ip", ip.text.Trim());
        }
        
        PlayerPrefs.SetInt("isServer", serverToggle.isOn ? 1 : 0) ;

        SceneManager.LoadScene("Dust", LoadSceneMode.Single);
    }


    [SerializeField] private GameObject serverUiGroup;
    [SerializeField] private GameObject nonServerUiGroup;
    public void ServerToggle(Toggle change)
    {
        serverUiGroup.SetActive(change.isOn);
        nonServerUiGroup.SetActive(!change.isOn);
    }
}