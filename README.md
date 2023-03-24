# 地心之子

# 目录规划

- 项目强相关

```text
- Docs          # 文档相关,不直接参与项目构建
 - images       # 文档中使用的图片
 - design.md    # 系统框架设计文档
 - world.md     # 游戏世界观文档
- Assets        # 参与构建的所有项目资源
  - Arts        # 艺术资源,程序和美术负责上传
    - Scenes    # 场景资源,程序和美术配合上传
    - Prefabs   # 预制体资源,程序和美术配合上传
    - Materials # 材质资源(shader),程序和美术配合上传
    - Textures  # 纹理资源(图片),美术负责上传
    - Audios    # 音频资源,美术负责上传
    - GUIs      # GUI资源,程序和美术配合上传
  - Scripts     # 脚本资源,程序负责上传
  - Designs     # 策划文档,策划负责上传
- README.md     # 项目说明文档
- .gitignore    # git忽略文件列表
- .clang-format # 代码格式化配置

- Assembly-CSharp.csproj # C#项目配置文件
- EarthCore.sln          # C#解决方案文件
```

- 项目非强相关

```text
- ProjectSettings   # Unity项目设置
- Packages          # Unity依赖包列表
- UserSettings      # Unity用户设置
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