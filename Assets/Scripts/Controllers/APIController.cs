
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class APIController : MonoBehaviour
{
    [SerializeField]
    string MAIN_URL = "https://games.kintoncloud.com/numbers";

    public NumbersCollection NumbersCol { get; private set; }

    private void Awake()
    {
        StartCoroutine(GetRequest(MAIN_URL));
    }

    private void Start()
    {

    }

    private void Update()
    {

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
            }
            else
            {
                Console.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                NumbersCol = JsonUtility.FromJson<NumbersCollection>(webRequest.downloadHandler.text);
                Console.Log("Length numbers received: "+ NumbersCol.numbers.Length);
            }
        }
    }

    public APINumbers GetRandomNumber()
    {
        int l = NumbersCol.numbers.Length;
        return NumbersCol.numbers[Random.Range(0, l)];
    }
}