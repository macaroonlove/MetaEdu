using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheet : MonoBehaviour
{
    const string URL = "https://docs.google.com/spreadsheets/d/1OA24KyJiDrqxJgMKv_8LiywPjXuw2lWOrMUB8FdBhSw/export?format=tsv&range=B2:D";

    public Dictionary<int, List<string>> Quizs = new Dictionary<int, List<string>>();

    private void Start()
    {
        StartCoroutine("GetData");
    }
    IEnumerator GetData()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;

        for (int i = 0; i < data.Split("\n").Length; i++)
        {
            if (Quizs.ContainsKey(int.Parse(data.Split("\n")[i].Split("\t")[0])) == false)
            {
                Quizs.Add(int.Parse(data.Split("\n")[i].Split("\t")[0]), new List<string>());
                Quizs[int.Parse(data.Split("\n")[i].Split("\t")[0])].Add(data.Split("\n")[i].Split("\t")[1]);
                Quizs[int.Parse(data.Split("\n")[i].Split("\t")[0])].Add(data.Split("\n")[i].Split("\t")[2]);
            }
            else
            {
                Quizs[int.Parse(data.Split("\n")[i].Split("\t")[0])].Add(data.Split("\n")[i].Split("\t")[1]);
                Quizs[int.Parse(data.Split("\n")[i].Split("\t")[0])].Add(data.Split("\n")[i].Split("\t")[2]);
            }
        }
        yield break;
    }
}
