using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Stores {
// 存档管理器
public static class StoreManager {
    // 存档文件夹
    private static readonly string StoreDir = Path.Combine(Application.persistentDataPath, "Store");

    static StoreManager() {
        if (!Directory.Exists(StoreDir)) {
            Directory.CreateDirectory(StoreDir);
        }
    }

    // 获取index对应的存档文件路径
    private static string GetStorePath(int index) {
        return Path.Combine(StoreDir, $"store_{index}.dat");
    }

    // 搜索所有存档对应的下标
    public static IEnumerable<int> All(int maxCnt) {
        for (var i = 0; i < maxCnt; i++) {
            var path = GetStorePath(i);
            if (File.Exists(path)) {
                yield return i;
            }
        }
    }

    // 新下标
    public static int NewIndex(int maxCnt) {
        for (var i = 0; i < maxCnt; i++) {
            var path = GetStorePath(i);
            if (!File.Exists(path)) {
                return i;
            }
        }
        return -1;
    }

    // 读取一个下标对应的存档
    public static bool Load(int index, out StoreGame store) {
        var path = GetStorePath(index);
        try {
            store = !File.Exists(path)
                ? null
                : JsonConvert.DeserializeObject<StoreGame>(File.ReadAllText(path));
        }
        catch (Exception e) {
            Debug.LogError(e);
            store = null;
            return false;
        }
        return true;
    }

    // 保存一个存档
    public static bool Save(int index, StoreGame store) {
        var path = GetStorePath(index);
        try {
            File.WriteAllText(path, JsonConvert.SerializeObject(store));
        }
        catch (Exception e) {
            Debug.LogError(e);
            return false;
        }
        return true;
    }

    // 删除一个存档
    public static bool Delete(int index) {
        var path = GetStorePath(index);
        try {
            File.Delete(path);
        }
        catch (Exception e) {
            Debug.LogError(e);
            return false;
        }
        return true;
    }

    // 当前游戏的全局唯一存档
    public static StoreGame Current { get; private set; }

    // 读取当前存档
    public static bool LoadCurrent(int index) {
        var ret = Load(index, out var current);
        Current = current;
        return ret;
    }

    // 保存当前存档
    public static bool SaveCurrent(int index) {
        return Save(index, Current);
    }
}
}
