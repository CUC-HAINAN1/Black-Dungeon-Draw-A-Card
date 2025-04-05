using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    private Dictionary<string, Action<object>> _eventDictionary;
    private static bool applicationIsQuitting = false;

    public static EventManager Instance {
    
        get {
            
            if (applicationIsQuitting) {

                return null;

            }

            if (_instance == null) {
            
                GameObject go = new GameObject("EventManager");
                _instance = go.AddComponent<EventManager>();
                DontDestroyOnLoad(go);
            
            }
            
            return _instance;
        }

    }

    //单例初始化
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _eventDictionary = new Dictionary<string, Action<object>>();
        DontDestroyOnLoad(gameObject);
    
    }

    //在应用完全退出前触发
    private void OnApplicationQuit() {
    
        applicationIsQuitting = true;
        _instance = null;
        Destroy(gameObject);
    
    }

    //订阅事件
    public void Subscribe(string eventName, Action<object> listener)
    {
        if (_eventDictionary.TryGetValue(eventName, out Action<object> existingEvent))
        {
            existingEvent += listener;
            _eventDictionary[eventName] = existingEvent;
        }
        
        else  
        {
            _eventDictionary.Add(eventName, listener);
        }
    
    }

    //取消订阅
    public void Unsubscribe(string eventName, Action<object> listener)
    {
        if (_eventDictionary.TryGetValue(eventName, out Action<object> existingEvent))
        {
            
            existingEvent -= listener;
            
            if (existingEvent == null)
            {
                _eventDictionary.Remove(eventName);
            }
            
            else
            {
                _eventDictionary[eventName] = existingEvent;
            }
        
        }
    
    }

    //事件触发器
    public void TriggerEvent(string eventName, object parameter = null)
    {
        
        if (_eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))    
        {
            thisEvent?.Invoke(parameter);
        }
    
    }

}