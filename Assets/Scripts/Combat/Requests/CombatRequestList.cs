using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat.Requests.Details;
using UnityEngine;
using Utils;

namespace Combat.Requests {
public class CombatRequestList {
    private readonly LinkedList<CombatRequest> m_requests   = new();
    private readonly Queue<RequestPostLogic>   m_postLogics = new();

    public CombatJudge Judge;

    public int Count => m_requests.Count + m_postLogics.Count;

    public bool Running { get; private set; }

    private bool InValid(CombatRequest request) {
        if (request is null or RequestPostLogic) {
            Debug.LogError("后处理逻辑需要用AddPostLogic入队");
            return true;
        }
        request.Judge = Judge;
        var reject = !request.CanEnqueue();
        if (!reject) return false;
        Debug.LogException(request.CreateContext);
        Judge.logger.AddLog($"Request {request} rejected: {request.RejectReason}");
        return true;
    }

    public void Add(CombatRequest request) {
        AddLast(request);
    }

    public void AddPost(RequestPostLogic request) {
        m_postLogics.Enqueue(request);
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

    private CombatRequest PopFirst() {
        if (m_requests.Count == 0) return m_postLogics.Dequeue();
        var node = m_requests.First;
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
            Judge.Players.ForEach(p => p.State.BeginRecord());
            yield return task.Execute();
            Judge.Players.ForEach(p => p.State.EndRecord());
        }
        yield return Judge.Players.Select(p => p.cardSlot.FreshUI());
        Running = false;
    }
}
}
