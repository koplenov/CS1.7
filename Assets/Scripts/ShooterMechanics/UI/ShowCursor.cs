using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Network = Utils.Network;

public class ShowCursor : MonoBehaviour
{
    public GameObject pauseMenu;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        }
    }

    public void Resume()
    {
        Cursor.visible = false;
        pauseMenu.SetActive(false);
    }

    public void ChangeServer()
    {
        Client.Instance.CloseUdp();
        Server.newServer?.Stop();
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Client.Instance.CloseUdp();
        Server.newServer?.Stop();
        Application.Quit();
    }
}