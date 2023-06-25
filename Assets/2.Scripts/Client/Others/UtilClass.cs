using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class UtilClass
{
    #region 이벤트 트리거
    public static void AddListener(this EventTrigger trigger, EventTriggerType triggerType, UnityAction<BaseEventData> callback)
    {
        if (ReferenceEquals(trigger, null))
        {
            Debug.LogError("EventTrigger is NULL.");
            return;
        }

        EventTrigger.Entry entry = new();
        entry.eventID = triggerType;
        entry.callback.AddListener(callback);

        trigger.triggers.Add(entry);
    }

    public static void RemoveListener(this EventTrigger trigger, EventTriggerType triggerType, UnityAction<BaseEventData> callback)
    {
        if (ReferenceEquals(trigger, null))
        {
            Debug.LogError("EventTrigger is NULL.");
            return;
        }

        EventTrigger.Entry entry = trigger.triggers.Find(e => e.eventID == triggerType);
        entry?.callback.RemoveListener(callback);
    }

    public static void RemoveAllListeners(this EventTrigger trigger, EventTriggerType triggerType)
    {
        if (ReferenceEquals(trigger, null))
        {
            Debug.LogError("EventTrigger is NULL.");
            return;
        }

        EventTrigger.Entry entry = trigger.triggers.Find(e => e.eventID == triggerType);
        entry?.callback.RemoveAllListeners();
    }
    #endregion

    #region 토큰 번호 만들기
    private static readonly System.Random random = new System.Random();
    private static readonly string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateToken(int length)
    {
        char[] token = new char[length];
        for (int i = 0; i < length; i++)
        {
            token[i] = characters[random.Next(characters.Length)];
        }
        return new string(token);
    }
    #endregion

    #region 셔플
    public static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }
    #endregion
}