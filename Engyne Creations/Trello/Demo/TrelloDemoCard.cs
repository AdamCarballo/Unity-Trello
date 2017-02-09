/*
 * TrelloDemoCard.cs
 * Demostration of a system information card.
 *  
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamEC/Unity-Trello
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Trello;

public class TrelloDemoCard : MonoBehaviour {

    public TrelloSend trelloSend;

    [Header("UI References")]
    public InputField title;
    public InputField desc;
    public Toggle screenshot;

    [Header("Transform Reference")]
    public Transform trackedObject;

    private byte[] file = null;


    /// <summary>
    /// Send a Trello Card using the info provided on the UI.
    /// </summary>
    public void DemoSendCard() {

        StartCoroutine(DemoSendCard_Internal());
    }

    private IEnumerator DemoSendCard_Internal() {

        TrelloCard card = new TrelloCard();

        card.name = title.text;
        card.due = DateTime.Today.ToString();
        card.desc = "#" + "Demo " + Application.version + "\n" +
            "___\n" +
            "###System Information\n" +
            "- " + SystemInfo.operatingSystem + "\n" +
            "- " + SystemInfo.processorType + "\n" +
            "- " + SystemInfo.systemMemorySize + " MB\n" +
            "- " + SystemInfo.graphicsDeviceName + " (" + SystemInfo.graphicsDeviceType + ")\n" +
            "\n" +
            "___\n" +
            "###User Description\n" +
            "```\n" +
            desc.text + "\n" +
            "```\n" +
            "___\n" +
            "###Other Information\n" +
            "Playtime: " + String.Format("{0:0}:{1:00}", Mathf.Floor(Time.time / 60), Time.time % 60) + "h" + "\n" +
            "Tracked object position: " + trackedObject.position;

        if (screenshot.isOn) {
            StartCoroutine(UploadJPG());
            while (file == null) {
                yield return null;
            }
            card.fileSource = file;
            card.fileName = DateTime.UtcNow.ToString() + ".jpg";
        }

        trelloSend.SendNewCard(card);
    }

    /// <summary>
    /// Captures the screen with UI and returns a byte array.
    /// </summary>
    /// <returns>Byte array of a jpg image</returns>
    private  IEnumerator UploadJPG() {

        // Only read the screen after all rendering is complete
        yield return new WaitForEndOfFrame();
        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        // Encode texture into JPG
        file = tex.EncodeToJPG();
        Destroy(tex);
    }
}