using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventListener : MonoBehaviour {

    public Dictionary<string, List<UnityEvent>> listeners = new Dictionary<string, List<UnityEvent>>();

    /// <summary>
    /// Adds a listener to a specific animation event with key (string)
    /// </summary>
    /// <param name="e">E.</param>
    /// <param name="s">S.</param>
    public void AddListener(UnityEvent e, string s) {
        if (!listeners.ContainsKey(s)) {
            listeners[s] = new List<UnityEvent>();
        }
        listeners[s].Add(e);
    }

    /// <summary>
    /// Removes the listener. Just don't use this.
    /// </summary>
    /// <param name="e">E.</param>
    public void RemoveListener(UnityEvent e) {
        bool complete = false;
        foreach (string s in listeners.Keys) {
            List<UnityEvent> l = listeners[s];
            for (int i = 0; i < l.Count; i++) {
                if (e == l[i]) {
                    l.RemoveAt(i);
                    complete = true;
                    break;
                }
            }
            if (complete)
                break;
        }
    }

    /// <summary>
    /// Removes the listener.
    /// </summary>
    /// <param name="s">S.</param>
    public void RemoveListener(string s) {
        listeners.Remove(s);
    }

    public void AnimationEvent(string s) {
        foreach (UnityEvent e in listeners[s]) {
            e.Invoke();
        }
    }
}
