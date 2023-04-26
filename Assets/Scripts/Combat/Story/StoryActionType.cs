namespace Combat.Story {
// 剧本行为类型
public enum StoryActionType {
    Init,    // 设置初始状态
    Start,   // 开始战斗
    Say,     // 显示文案
    Show,    // 显示立绘
    Hide,    // 隐藏对话框
    Give,    // 发指定牌
    Wait,    // 等待出指定牌(必须出)
    Next,    // 切换到下一回合
    WaitEnd, // 等待战斗结束
}

/*
// 一行注释,不参与解析
@comment {line}

// 设置初始状态,设置牌堆可以在顶部额外放置指定的牌
@init {player|enemy} """
生命值={value};
法力值={value};
初始手牌={value};
回合抽牌={value};
最大手牌={value};
[金木水火土]法印={value};
牌堆={card_name,}*;
"""

// 开始战斗 
@start

// 某角色说一句话,content是长句,line是短句,content不能包含""",line不能有换行,空格,和:号
@say {name} """
{content}
"""

@say {name}:{line}

// 旁白说一句话
@say """
{content}
"""

@say {line}

// 显示指定立绘.自动加载Textures/Dialog/{image_name}下的图片
@show {image_name}

// 隐藏对话框和立绘
@hide

// 发指定牌.card_name可以写多个
@give {player|enemy} {card_name,}*

// 限制出牌.card_name可以写多个
@wait {player|enemy} {card_name,}*

// 切换到下一回合
@next

// 等待战斗结束
@wait_end

 */
}
