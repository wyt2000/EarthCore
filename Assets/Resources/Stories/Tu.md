﻿```text
@say 到达土属性房间
@show 伊克斯 left
@say 伊克斯、弗姆:...
@show 莫西 left
@say 莫西:喂！
@say 莫西:都说了叫你们等等我！
@show 伊克斯 left
@say 伊克斯、弗姆:小心！
@show 克莱姆 right
@show 莫西 left
@say 莫西:啊！！！！！
@hide left
@hide

@comment
@init player """
名字=伊克斯;
主属性=火;
生命值=90;
法力值=50;
初始手牌=0;
回合抽牌=2;
最大手牌=5;
牌堆=汲取,熔铁之刃,火力压制;
"""

@init enemy """
名字=克莱姆;
主属性=土;
生命值=100;
法力值=50;
初始手牌=0;
回合抽牌=1;
最大手牌=5;
土法印=1;
牌堆=;
"""

@start

@help """
【元素联动】
同时打出两张、三张或五张不同属性的卡牌可以触发【元素联动】效果，不同属性的【元素联动】附加buff不同，可尝试自行搭配。任何元素联动都能触发一次摸牌效果
但需要注意的是，【元素联动】的参与元素必须在元素五芒星中位置相邻，例如【火元素卡牌】可以和【木元素卡牌】联动触发【引燃】效果，但【金元素卡牌】和【木元素卡牌】无法一同打出，因为它们在元素五芒星中的位置【不相邻】。
"""
@hide

@help """
现在，同时选中【汲取】和【熔铁之刃】打出，看看会触发何种效果。
"""

@hide

@play 汲取,熔铁之刃

@play 火力压制

@wait

@show 克莱姆 right
@say 克莱姆:...
@say 克莱姆:...好饿...
@show 伊克斯 left
@say 众人:...
@show 莫西 left
@say 莫西:你没事吧？
@say 克莱姆:啊？没...
@say 克莱姆:啊没事就怪了哎呦~
@say 克莱姆:哎呀我腿好痛啊
@say 克莱姆:莫西姐姐快来帮我看看
@say 克莱姆:哎呦痛死了~
@show 伊克斯 left
@say 众人:...
@say 伊克斯 """
这里该不会也...
（克莱姆:哎呦~腿不行了站不起来了）
"""
@show 弗姆 left
@say 弗姆 """
没错
（克莱姆:莫西姐姐快帮我看看呀哎呦~）
"""

@say 克莱姆身后的墙上出现棕色神秘文字
@say 神秘文字 """
%！#%！￥诅咒&%*……#%#￥%
%#@研究!^!&^1…富集…￥！#&￥
#@$#$失控@%@%$&3^$@^#&
#@^%#^@7……%@#侵蚀&%……￥#
"""
@show 伊克斯 left
@say 伊克斯、弗姆 """
果然
（克莱姆:哎呦都没人管人家吗哎呦~）
"""
@show 莫西 left
@say 莫西:你给我安静点！
@say 克莱姆:你干嘛~哼哼~哎呦~
@say 克莱姆:就不能照顾下伤员吗？
@hide right
@say 莫西:诅咒...侵蚀...失控？
@say 莫西:这都什么跟什么呀？
@show 伊克斯 left
@say 伊克斯:结合你们刚才的状态
@say 伊克斯:我感觉很可能是某种未知的力量控制了你们的心智
@say 伊克斯:也许洛斯特艾斯号的失踪也与它有关
@say 伊克斯:我们继续往前走吧，也许能发现点什么
@say 克莱姆身后的墙消失，变为通往下一场景的通道
@show 莫西 left
@say 莫西:哎！！！
@say 莫西:怎么又往前跑了！！！
@say 莫西:家人们咱就是说一整个无语住了
@show 克莱姆 left
@say 克莱姆:喂！！！
@say 克莱姆:没人管我吗？？？
@say 克莱姆:我腿真的受伤了啊！！！
@say 克莱姆:救一下啊！！！！
@say 克莱姆:喂！！！！！！！！！
@say 前往水属性房间
@hide
```