```mermaid
graph TD

%%S0[回合开始]
S1[OnBeforeTurnStart]

%%S2[回合结束]
S3[OnAfterTurnEnd]

%%S4[上buff]
S5[OnBeforeAttach]
S6[OnAfterAttach]

%%S7[下buff]
S8[OnLeaveAttach]

%%S9[生命修改]
S10[OnBeforeTakeHpChange]
S11[OnBeforeSelfHpChange]
%%S12[元素击碎]
S13[OnAfterSelfHpChange]
S14[OnAfterTakeHpChange]

%%S15[出牌]
S16[OnBeforePlayBatchCard]
%%S17[元素联动]
S18[每张卡的OnExecute]
S19[OnAfterPlayBatchCard]

%%S20[预览法力消耗]
%%S21[计算卡片动态法力消耗]
S22[OnPostDealManaCost]

%%S23[上buff]
S24[OnBeforeAttach]
S25[OnAfterAttach]

%%S26[下buff]
S27[OnLeaveAttach]

%%S29[生命修改]
S30[OnBeforeTakeHpChange]
S31[OnBeforeSelfHpChange]
%%S32[元素击碎]
S33[OnAfterSelfHpChange]
S34[OnAfterTakeHpChange]

%%S44[上buff]
S45[OnBeforeAttach]
S46[OnAfterAttach]

%%S47[下buff]
S48[OnLeaveAttach]

%%S49[生命修改]
S50[OnBeforeTakeHpChange]
S51[OnBeforeSelfHpChange]
%%S52[元素击碎]
S53[OnAfterSelfHpChange]
S54[OnAfterTakeHpChange]
subgraph 回合开始阶段
S1--回合开始-->S5-->S6--上buff-->S8--下buff-->S10-->S11--元素击碎-->S13-->S14
end
subgraph 出牌
S14--生命修改-->S22--预览法力消耗--计算卡牌动态法力消耗-->S16--出牌--元素联动-->S18-->S19-->S24-->S25--上buff-->S27--下buff-->S30-->S31--元素击碎-->S33-->S34--生命修改-->S22
end
subgraph 回合结束阶段
S34--弃牌-->S3-->S45-->S46--上buff-->S48--下buff-->S50-->S51--元素击碎-->S53-->S54
end
```

