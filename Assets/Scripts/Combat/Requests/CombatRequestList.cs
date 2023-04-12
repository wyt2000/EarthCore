using System.Collections;
using System.Collections.Generic;
using Combat.Requests.Details;
using UnityEngine;

namespace Combat.Requests {
public class CombatRequestList {
    private readonly LinkedList<CombatRequest> m_requests = new();

    public CombatJudge Judge;

    public IEnumerable<CombatRequest> Raw => m_requests;

    public int Count => m_requests.Count;

    public bool Running { get; private set; }

    private bool InValid(CombatRequest request) {
        if (Judge == null || request == null) return true;
        request.Judge = Judge;
        var reject = !request.CanEnqueue();
        if (reject) {
            var msg = $"Request {request} rejected: {request.RejectReason}";
            Debug.Log(msg);
            Judge.logger.AddLog(msg);
        }
        return reject;
    }


    public void Add(CombatRequest request) {
        AddLast(request);
    }

    public void AddFirst(CombatRequest request) {
        if (InValid(request)) return;
        request.Node = m_requests.AddFirst(request);
    }

    public void AddLast(CombatRequest request) {
        if (InValid(request)) return;
        request.Node = m_requests.AddLast(request);
    }

    public void AddBefore(CombatRequest request, CombatRequest newRequest) {
        if (InValid(request) || InValid(newRequest)) return;
        if (request.Node == null) {
            Debug.LogError("Request node is null");
            return;
        }
        newRequest.Node = m_requests.AddBefore(request.Node, newRequest);
    }

    public void AddAfter(CombatRequest request, CombatRequest newRequest) {
        if (InValid(request) || InValid(newRequest)) return;
        if (request.Node == null) {
            Debug.LogError("Request node is null");
            return;
        }
        newRequest.Node = m_requests.AddAfter(request.Node, newRequest);
    }

    public void Clear() {
        m_requests.Clear();
    }

    private CombatRequest PopFirst() {
        if (m_requests.Count == 0) return null;
        var node = m_requests.First;
        while (node is { Value: RequestPostLogic }) node = node.Next;
        node ??= m_requests.First;
        var value = node.Value;
        m_requests.Remove(node);
        return value;
    }

    public IEnumerator RunAll() {
        if (Running) yield break;
        Running = true;
        while (Count > 0) {
            var task = PopFirst();
            var desc = task.Description();
            if (!string.IsNullOrEmpty(desc)) Judge.logger.AddLog(desc);
            yield return task.Execute();
        }
        Running = false;
    }
}
}
