using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KillObjectiveManager : MonoBehaviour
{
    [Serializable]
    public class Objective
    {
        public string id = "Ballesteros";
        public int targetKills = 3;
        public bool triggerOnce = true;

        [Header("Acción al completar")]
        public UnityEvent onReached;

        [HideInInspector] public int currentKills = 0;
        [HideInInspector] public bool completed = false;
    }

    [Header("Objetivos por tipo (ID)")]
    public Objective[] objectives;

    private Dictionary<string, Objective> map;

    void Awake()
    {
        map = new Dictionary<string, Objective>(StringComparer.OrdinalIgnoreCase);

        if (objectives != null)
        {
            foreach (var o in objectives)
            {
                if (o == null || string.IsNullOrWhiteSpace(o.id)) continue;

                // Si repites ID, se queda el primero
                if (!map.ContainsKey(o.id))
                {
                    o.currentKills = 0;
                    o.completed = false;
                    map.Add(o.id, o);
                }
            }
        }
    }

    public void AddKill(string objectiveId, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(objectiveId)) return;
        if (map == null || !map.TryGetValue(objectiveId, out var obj)) return;

        if (obj.completed && obj.triggerOnce) return;

        obj.currentKills = Mathf.Max(0, obj.currentKills + Mathf.Max(1, amount));
        Debug.Log($"[KillObjective] {obj.id}: {obj.currentKills}/{obj.targetKills}");

        if (!obj.completed && obj.currentKills >= obj.targetKills)
        {
            obj.completed = true;
            obj.onReached?.Invoke();
        }
    }

    public int GetKills(string objectiveId)
    {
        if (map != null && map.TryGetValue(objectiveId, out var obj)) return obj.currentKills;
        return 0;
    }
}