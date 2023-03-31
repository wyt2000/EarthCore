# 战斗模块

## 战斗属性

- 生命值
- 最大生命值
- 法力值
- 最大法力值

## 战斗流程图

```mermaid
graph TD  
S1[备战界面] --> |战斗开始| S2[战斗开始]
S0[任意状态] --> |逃跑请求| S3[战斗结束]
S7 --> |死亡事件| S3[战斗结束]
S2 --> |摸牌请求x1| S4[回合开始]
S4 --> |摸牌请求x2| S7
S5 --> |等待超时| S6[等待玩家弃牌]
S5[等待玩家出牌] --> |出牌请求| S7[结算所有请求]
S7 --> |"结算完毕"| S5
S8[弃牌并回合结束]
S6 --> |"等待超时"| S8
S6 --> |弃牌请求| S8
S8 --> |切换玩家| S4
S3 --> |结算收益| S1
```

```mermaid
graph TD  
subgraph 外循环
S0[初始化状态]
S1[备战界面]
S2[战斗开始]
S3[战斗结束]
S4[回合开始]
end
subgraph 内循环
S5[等待玩家出牌]
S6[等待玩家弃牌]
S7[结算所有请求]
S8[弃牌并回合结束]
end
S1 --> S2
S2 --> S0 --> |摸牌请求x1| S4
S3 --> |结算收益| S1
S4 --> |摸牌请求x2| S7
S5 --> |主动结束| S6
S5 --> |等待超时| S6
S5 --> |出牌请求| S7
S6 --> |等待超时| S8
S6 --> |弃牌请求| S8
S7 --> |逃跑请求/死亡事件| S3
S7 --> |结算完毕| S5
S8 --> |切换玩家| S4
```

// Todo 添加判断状态

## 请求系统

```csharp
class GameRequest {
    // 用于拦截请求
    public bool Reject;

    // 修改逻辑状态前的动画
    public virtual void PlayPreAnimation();
    // 修改逻辑状态的代码
    public virtual void ExecuteLogic();
    // 修改逻辑状态后的动画
    public virtual void PlayPostAnimation();
}
```

## 战斗时间节点

- 战斗开始