﻿```text
@show 伊克斯 left
@say 伊克斯、弗姆、莫西:...
@show 克莱姆 left
@say 克莱姆:哎呦
@say 克莱姆:哎呦等等我哎呦
@show 弗姆 left
@say 弗姆:嘘
@say 弗姆:安静！
@hide left
@say ...
@show 波顿 right

@hide

@comment
@init player """
名字=伊克斯;
主属性=火;
生命值=90;
法力值=50;
初始手牌=2;
回合抽牌=0;
最大手牌=5;
牌堆=火力压制,熔铁之刃;
"""

@init enemy """
名字=波顿;
主属性=水;
生命值=100;
法力值=50;
初始手牌=0;
回合抽牌=0;
最大手牌=5;
金法印=6;
牌堆=;
"""

@start

@help """
【元素叠加】
你可以通过一次性打出多张相同属性的卡牌来加强卡牌效果，
恢复、伤害、护盾类效果都会根据一次性打出卡牌数量的多少获得效果提升。
最多可以同时打出五张同属性卡牌。
"""

@play 火力压制,熔铁之刃

@wait

@show 波顿 right
@say 波顿:...
@show 伊克斯 left
@say 伊克斯:波顿先生！
@say 伊克斯:您还好吗！
@say 波顿:传说...
@say 波顿:果然是真的...
@show 弗姆 left
@say 弗姆:什么传说？
@say 波顿:元素诅咒...
@show 伊克斯 left
@say 伊克斯、莫西、克莱姆:元素诅咒？
@say 波顿:一种古老而神秘的力量
@say 波顿:能够控制人的心智
@say 伊克斯:您是说大家刚刚的状态
@say 伊克斯:是因为元素诅咒？
@say 波顿:没错
@say 伊克斯:快看！
@say 伊克斯:神秘文字又出现了！
@say 神秘文字 """
@！￥%#@底层￥#&（#%&
#@%#驭素师$^@#%@……￥#
@%！队伍#……&%￥*多种
@#%！封印……！￥地面￥……#￥
"""
@say 伊克斯、弗姆、莫西、克莱姆:这...
@say 波顿:我一直在寻找元素核心的来源
@say 波顿:古籍中的文字和这里的极为相似
@say 波顿:看来这里藏着很多不为人知的古老秘密
@say ...
@say 前往最终房间
@hide
```