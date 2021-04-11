
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class APIController : MonoBehaviour
{
    [SerializeField]
    string MAIN_URL = "http://worldtimeapi.org/api/ip";

    APITime apiTime;
    Text clock;

    float timer;
    float waitTime = 1f;

    private void Awake()
    {
        clock = GetComponent<Text>();
        apiTime = new APITime();
    }

    private void Start()
    {
        StartCoroutine(GetRequest(MAIN_URL));
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            StartCoroutine(GetRequest(MAIN_URL));
            timer = 0;
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Console.Log(pages[page] + ": Error: " + webRequest.error);
                clock.text = webRequest.error;
            }
            else
            {
                //Console.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                JsonUtility.FromJsonOverwrite(webRequest.downloadHandler.text, apiTime);
                System.DateTime dateTime = System.DateTime.Parse(apiTime.datetime);
                clock.text = dateTime.ToString("yyyy-MM-dd hh:mm:ss");
            }
        }
    }
}