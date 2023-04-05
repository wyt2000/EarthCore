```mermaid
graph RL
subgraph 战斗外流程
    A[菜单界面] --> B[选择敌人]
    B --> C[选择出战卡牌]
    C --> D[初始化战斗信息]
    D --> E[等待具体战斗结果]
    E --> |逃跑| F[结算逃跑收益]
    E --> |重试| D
    E --> |胜利| G[结算胜利收益]
    E --> |失败| H[结算失败收益]
    F & G & H --> A
end
```

```mermaid
graph TB
subgraph "具体战斗流程(Effects表示玩家附着的所有效果,Effect表示新增/移除的效果)"
    A[战斗开始] --> B[初始摸牌请求x2]
    B --> BC[Effects.OnBeforeTurnStart] --> C[回合开始]
    C --> 发起摸牌请求x1 --> F
    D --> |主动结束| E[等待玩家弃牌]
    D --> |等待超时| E
    D --> |出牌请求| F[结算所有请求]
    E --> |等待超时| G[弃牌并回合结束]
    E --> |弃牌请求| G
    F --> |结算完毕| D[出牌阶段]
    G --> Effects.OnAfterTurnEnd --> |切换玩家| BC
    收到新增效果请求 --> Effects.OnBeforeAttach --> |全都返回false| 附着效果 --> Effect.OnAfterAttach
    收到移除效果请求 --> 移除效果 --> Effect.OnLeaveAttach
    收到生命值修改请求 --> Effects.OnBeforeTakeHpChange --> Effects.OnBeforeSelfHpChange --> |全都返回false| 修改生命值 --> OnAfterSelfHpChange --> Effects.OnAfterTakeHpChange
end
```

```mermaid
graph TB
subgraph "具体出牌流程(Cards表示即将要出的若干张牌)"
    收到出牌请求 --> Cards.OnBeforePlayBatch --> Effects.OnBeforePlayBatchCard --> Cards.OnExecute
    所有请求执行完毕 --> Effects.OnAfterPlayBatchCard --> Cards.OnAfterPlayBatch
    尝试选中卡牌 -- "卡牌可用(无元素/元素联动)" --> 计算当前总法力消耗 --> Cards.OnPostDealManaCost --> Effects.OnPostDealManaCost --> 预测元素联动效果调用OnPostDealManaCost --> |法力消耗足够| 修改卡牌选中状态
end
```

