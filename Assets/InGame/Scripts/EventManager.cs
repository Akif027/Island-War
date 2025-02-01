using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    // Unified dictionary to manage all events with any parameter type
    private static Dictionary<string, Delegate> eventDictionary = new Dictionary<string, Delegate>();
    /// <summary>
    // Add events with parameters
    public static event Action<int, int> CoinAdd;
    public static event Action<int, int> CoinDeduct;

    public static event Action<int, int> GloryPointAdd;    // Add Glory Point
    public static event Action<int, int> GloryPointDeduct; // Deduct Glory Point
    /// <summary>
    /// Subscribe to an event with one parameter.
    /// </summary>
    public static void Subscribe<T>(string eventName, Action<T> listener)
    {
        if (!eventDictionary.TryGetValue(eventName, out var existingDelegate))
        {
            eventDictionary[eventName] = listener;
        }
        else
        {
            eventDictionary[eventName] = Delegate.Combine(existingDelegate, listener);
        }
    }

    /// <summary>
    /// Unsubscribe from an event with one parameter.
    /// </summary>
    public static void Unsubscribe<T>(string eventName, Action<T> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out var existingDelegate))
        {
            var currentDelegate = Delegate.Remove(existingDelegate, listener);
            if (currentDelegate == null)
            {
                eventDictionary.Remove(eventName);
            }
            else
            {
                eventDictionary[eventName] = currentDelegate;
            }
        }
    }

    /// <summary>
    /// Trigger an event with one parameter.
    /// </summary>
    public static void TriggerEvent<T>(string eventName, T parameter)
    {
        if (eventDictionary.TryGetValue(eventName, out var existingDelegate))
        {
            (existingDelegate as Action<T>)?.Invoke(parameter);
        }
    }

    /// <summary>
    /// Subscribe to an event with two parameters.
    /// </summary>
    public static void Subscribe<T1, T2>(string eventName, Action<T1, T2> listener)
    {
        if (!eventDictionary.TryGetValue(eventName, out var existingDelegate))
        {
            eventDictionary[eventName] = listener;
        }
        else
        {
            eventDictionary[eventName] = Delegate.Combine(existingDelegate, listener);
        }
    }

    /// <summary>
    /// Unsubscribe from an event with two parameters.
    /// </summary>
    public static void Unsubscribe<T1, T2>(string eventName, Action<T1, T2> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out var existingDelegate))
        {
            var currentDelegate = Delegate.Remove(existingDelegate, listener);
            if (currentDelegate == null)
            {
                eventDictionary.Remove(eventName);
            }
            else
            {
                eventDictionary[eventName] = currentDelegate;
            }
        }
    }

    /// <summary>
    /// Trigger an event with two parameters.
    /// </summary>
    public static void TriggerEvent<T1, T2>(string eventName, T1 param1, T2 param2)
    {
        if (eventDictionary.TryGetValue(eventName, out var existingDelegate))
        {
            (existingDelegate as Action<T1, T2>)?.Invoke(param1, param2);
        }
    }

    /// <summary>
    /// Subscribe to an event with no parameters.
    /// </summary>
    public static void Subscribe(string eventName, Action listener)
    {
        if (!eventDictionary.TryGetValue(eventName, out var existingDelegate))
        {
            eventDictionary[eventName] = listener;
        }
        else
        {
            eventDictionary[eventName] = Delegate.Combine(existingDelegate, listener);
        }
    }

    /// <summary>
    /// Unsubscribe from an event with no parameters.
    /// </summary>
    public static void Unsubscribe(string eventName, Action listener)
    {
        if (eventDictionary.TryGetValue(eventName, out var existingDelegate))
        {
            var currentDelegate = Delegate.Remove(existingDelegate, listener);
            if (currentDelegate == null)
            {
                eventDictionary.Remove(eventName);
            }
            else
            {
                eventDictionary[eventName] = currentDelegate;
            }
        }
    }

    /// <summary>
    /// Trigger an event with no parameters.
    /// </summary>
    public static void TriggerEvent(string eventName)
    {
        if (eventDictionary.TryGetValue(eventName, out var existingDelegate))
        {
            (existingDelegate as Action)?.Invoke();
        }
    }


    /// <summary>
    /// Trigger the CoinAdd event.
    /// </summary>
    public static void TriggerCoinAdd(int playerNumber, int coinAmount)
    {
        CoinAdd?.Invoke(playerNumber, coinAmount);
    }

    /// <summary>
    /// Trigger the CoinDeduct event.
    /// </summary>
    public static void TriggerCoinDeduct(int playerNumber, int coinAmount)
    {
        CoinDeduct?.Invoke(playerNumber, coinAmount);
    }

    public static void TriggerGloryPointAdd(int playerNumber, int gloryPointAmount)
    {
        GloryPointAdd?.Invoke(playerNumber, gloryPointAmount);
    }

    /// <summary>
    /// Trigger the GloryPointDeduct event.
    /// </summary>
    public static void TriggerGloryPointDeduct(int playerNumber, int gloryPointAmount)
    {
        GloryPointDeduct?.Invoke(playerNumber, gloryPointAmount);
    }
}
