﻿# 地心之子

## TodoList

### 美术资源需求

除特殊说明 , 均导出为png格式图片

- 角色贴图 : 尺寸待定

| 人物   | 路径        | 关键字          | 进度  |
|------|-----------|--------------|-----|
| 主角   | `Char/主角` | 火系,机械元素      |     |
| 女队医  | `Char/队医` | 木系,医生元素      |     |
| 老兵   | `Char/老兵` | 金系,军队元素,硬汉形象 |     |
| 厨师小胖 | `Char/厨师` | 土系           |     |
| 老科学家 | `Char/博士` | 水系           |     |

- 卡牌贴图 : 尺寸 1000x800 , 路径均为`Card/Details/{卡牌名}`

| 卡牌名  | 关键字           | 进度  |
|------|---------------|-----|
| 试探   | 金 , 攻击 , 防守   | 完成  |
| 格挡   | 金 , 防守        | 完成  |
| 重击   | 金 , 打击感       |     |
| 盾击   | 金 , 厚重        |     |
|      |               |     |
| 汲取   | 木 , 吸血        | 完成  |
| 清醒   | 木 , 回蓝        | 完成  |
| 包扎   | 木 , 回血        |     |
| 良药   | 木 , 持续性回血     |     |
|      |               |     |
| 法力狂涌 | 水 , 爆发攻击      | 完成  |
| 积蓄   | 水 , 延迟攻击 , 蓄力 |     |
| 释放   | 水 , 攻击        |     |
| 暗潮涌动 | 水 , 延迟攻击      |     |
|      |               |     |
| 火力压制 | 火 , 多次打击      | 完成  |
| 熔铁之刃 | 火 , 破甲        | 完成  |
| 穿刺   | 火 , 穿透 , 锐利   |     |
| 焚烧   | 火 , 破坏力       |     |
|      |               |     |
| 魔法守护 | 土 , 防御        | 完成  |
| 污泥之躯 | 土 , 耗血加盾      |     |
| 厄运   | 土 , 延迟伤害      |     |
|      |               |     |
| 渴望   | 无 , 功能牌       | 完成  |
| 火之召唤 | 无 , 功能牌       | 完成  |
| 铁甲加固 | 无 , 基于护盾造成伤害  |     |

- effect图标 : 尺寸 60x60 , 路径均为`Effect/Details/{effect名}`

| effect名 | 关键字                | 进度  |
|---------|--------------------|-----|
| 免疫      | 金土联动 , 免疫伤害        | 完成  |
| 引燃      | 火木联动 , 百分比伤害       | 完成  |
| 自燃      | 火木土联动 , 反伤         | 完成  |
| 洞察      | 金木水联动 , 免疫伤害和控制    | 完成  |
| 淬炼      | 火土金联动 , 百分比伤害      | 完成  |
|         |                    |     |
| 枯竭      | 金击碎 , 减造成物理伤害      | 完成  |
| 破魔      | 水击碎 , 加受到魔法伤害      | 完成  |
| 破甲      | 火击碎 , 加受到物理伤害      | 完成  |
| 弱化      | 土击碎 , 减造成魔法伤害      | 完成  |
|         |                    |     |
| 冷却      | 任意元素击碎 , 多少回合后恢复   | 完成  |
|         |                    |     |
| 清算      | 水 , 延迟伤害           | 完成  |
| 疗养      | 木 , 回血回蓝           | 完成  |
| 厄运      | 土 , 延迟伤害           | 完成  |
| 铁甲加固    | 土 , 加盾时攻击          | 完成  |
|         |                    |     |
| 护甲      | 固有 , 回合开始回盾        | 完成  |
| 屏障      | 固有 , 魔法护盾抵消魔法伤害则反伤 |     |

- 状态栏图标 : 尺寸60x60 , 路径均为`State/Icon/{属性名}`

| 属性名      | 进度  | 备注  |
|----------|-----|-----|
| 生命值      | 完成  |     |
| 法力值      | 完成  |     |
| 物理护盾     | 完成  |     |
| 物理增伤     | 完成  |     |
| 物理减伤     | 完成  |     |
| 魔法护盾     | 完成  |     |
| 魔法增伤     | 完成  |     |
| 魔法减伤     | 完成  |     |
| {元素类型}法印 | 完成  |     |

- 杂项贴图

| 名字         | 尺寸        | 路径             | 进度  | 备注               |
|------------|-----------|----------------|-----|------------------|
| 游戏背景       | 1920x1080 | `Else/战斗背景`    | 样图  |                  |
| 卡背贴图       | 1600x2400 | `Card/卡背`      | 完成  |                  |
| 卡牌遮罩       | 1600x2400 | `Card/边框`      | 完成  |                  |
| 缺省卡牌       | 1000x800  | `Card/暂缺`      | 完成  | 后续可以用在别的地方       |
| 牌堆背景       | 800x800   | `Card/牌堆`      |     | 可以先不搞            |
| 状态栏背景      | 300x300   | `State/状态栏背景`  |     |                  |
| effect栏背景  | 600x120   | `Effect/背景`    |     | 可以先不搞            |
| 卡槽背景       | 1200x300  | `Card/卡槽`      |     |                  |
| 状态栏属性背景(长) | 260x40    | `State/属性背景_长` | 未独立 | 状态栏里属性图标的背景,一条黑纹 |
| 状态栏属性背景(短) | 110x40    | `State/属性背景_短` |     | 状态栏里属性图标的背景,一条黑纹 |

## 目录规划

- 项目强相关

```text
- Docs          # 文档相关,不直接参与项目构建
  - Frame        # 项目整体框架文档
  - Design       # 策划设计文档
  - Images       # 文档中使用的图片
- Assets        # 参与构建的所有项目资源
  - Resources   # 艺术资源,程序和美术负责上传
  - Scenes      # 场景资源,程序和美术配合上传
    - Materials # 材质资源(shader),程序和美术配合上传
    - Textures  # 纹理资源(图片),美术负责上传
    - Audios    # 音频资源,美术负责上传
  - Prefabs     # 预制体资源,程序和美术配合上传
  - Scripts     # 脚本资源,程序负责上传
  - Designs     # 策划文档,策划负责上传
  - ThirdParty  # 第三方库(程序负责上传)
    - TMPro     # 中文字体库
- README.md     # 项目说明文档
- .gitignore    # git忽略文件列表
- .clang-format # 代码格式化配置
```

- 项目非强相关

```text
- ProjectSettings   # Unity项目设置
- Packages          # Unity依赖包列表
```

# 项目规范

## IDE相关规则

- 统一使用rider作为IDE  
  美术也用这个提交资源,有可视化的git提交工具方便一些

- Git Commit Message Helper  
  统一安装.git提交信息提示模板插件,每次提交都要带tag

- One Dark Pro插件  
  程序推荐安装.代码高亮主题,代替ide默认皮肤

- Copilot插件  
  程序按需安装.提高生产力,一个月10刀不贵或者学生认证白嫖

## 代码规范

- 写完一行代码就格式化一下代码

- 不要忽略编译器的警告和拼写错误

- 不要写重复的代码,多封装

## Git规范

- 每个人都要有自己的分支,不要直接在master上开发
  - 策划 : design(瑞瓦肖幽灵)
  - 美术 : art1(飞屋闲鱼)/art2(mn)
  - 程序 : dev1(MnZn)/dev2(Sayaka)
- 每日开发前 : 将远程最新master分支合并到自己的分支,解决冲突后再进行今日开发
- 每日开发后 : 将自己分支的今日提交push到远程服务器上,等待统一合并到master
- 不要动别人的分支
- 尽量不要一次提交所有文件,把所有改动分类提交,方便查看日志
- 自己有多个任务的时候可以在本地开子分支,开发完后再合并到自己的分支