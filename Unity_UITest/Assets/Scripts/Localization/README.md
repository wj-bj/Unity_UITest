# Unity多语言系统

这是一个基于JSON文件的Unity多语言系统，支持在运行时切换UI文本语言。

## 功能特点

- 支持多种语言（当前实现了英文和中文）
- 基于JSON文件存储文本数据，易于维护和扩展
- 支持运行时切换语言，UI文本实时更新
- 支持Unity UI Text和TextMeshPro组件

## 使用方法

### 1. 设置LanguageManager

在场景中添加LanguageManager组件：

```csharp
// 在任何脚本中获取LanguageManager实例
LanguageManager languageManager = LanguageManager.Instance;
```

LanguageManager会自动加载默认语言（英文）的文本数据。

### 2. 为UI文本添加本地化支持

在包含Text或TextMeshProUGUI组件的GameObject上添加LocalizedText组件：

1. 选择GameObject
2. 点击"Add Component"
3. 搜索并添加"LocalizedText"
4. 在Inspector中设置"Text Key"字段，对应JSON文件中的键

### 3. 添加语言切换按钮

方法1：使用LanguageSwitcher组件

1. 在Button GameObject上添加LanguageSwitcher组件
2. 在Inspector中设置"Target Language"

方法2：直接调用LanguageManager

```csharp
// 切换到英文
LanguageManager.Instance.SwitchLanguage(LanguageType.English);

// 切换到中文
LanguageManager.Instance.SwitchLanguage(LanguageType.Chinese);
```

### 4. 添加新的文本

1. 在`Assets/Resources/Localization/Localization_en.json`和`Assets/Resources/Localization/Localization_zh.json`中添加新的键值对
2. 在LocalizedText组件中使用新添加的键

### 5. 添加新的语言

1. 在LanguageType枚举中添加新的语言类型
2. 在LanguageManager.GetLanguageCode方法中添加新语言的代码映射
3. 创建新的JSON文件，如`Localization_fr.json`（法语）

## 文件结构

- `LanguageManager.cs`: 核心管理器，负责加载文本数据和语言切换
- `LocalizedText.cs`: 挂载在UI文本组件上，自动更新文本内容
- `LanguageSwitcher.cs`: 挂载在按钮上，用于切换语言
- `LanguageType.cs`: 定义支持的语言类型
- `Localization_en.json`: 英文文本数据
- `Localization_zh.json`: 中文文本数据

## 示例

查看`LocalizationDemo.cs`了解如何在场景中使用多语言系统。 