# Game Frame X Apple 登录

该包为使用 Game Frame X 框架的 Unity 项目提供 Apple 登录功能。

## 安装

要安装此包，请将以下行添加到 Unity 项目 `Packages` 目录下的 `manifest.json` 文件中：

```json
{
  "dependencies": {
    "com.gameframex.unity.login.apple": "https://github.com/gameframex/com.gameframex.unity.login.apple.git#1.0.0",
    "com.gameframex.unity": "1.1.1",
    "com.lupidan.apple-signin-unity": "1.5.0"
  }
}
```

## 用法

1.  将 `AppleLoginComponent` 添加到场景中的 GameObject。
2.  获取 `AppleLoginComponent` 实例并调用 `Init()` 方法初始化 Apple 登录管理器。
3.  调用 `Login()` 方法以启动 Apple 登录流程。此方法接受两个操作作为参数：`loginSuccess` 和 `loginFail`。
4.  成功登录后，将使用包含用户信息的 `AppleLoginSuccess` 对象调用 `loginSuccess` 操作。
5.  如果登录失败，将使用错误代码调用 `loginFail` 操作。
6.  调用 `LogOut()` 方法以注销用户。

### 示例

```csharp
using GameFrameX.Login.Apple.Runtime;
using UnityEngine;

public class AppleLoginExample : MonoBehaviour
{
    private AppleLoginComponent _appleLoginComponent;

    void Start()
    {
        _appleLoginComponent = GetComponent<AppleLoginComponent>();
        _appleLoginComponent.Init();
    }

    public void OnLoginButtonClick()
    {
        _appleLoginComponent.Login(
            (appleLoginSuccess) =>
            {
                Debug.Log("Apple 登录成功！");
                Debug.Log($"玩家 ID: {appleLoginSuccess.PlayerId}");
                Debug.Log($"电子邮件: {appleLoginSuccess.Email}");
                Debug.Log($"显示名称: {appleLoginSuccess.DisplayName}");
                Debug.Log($"ID 令牌: {appleLoginSuccess.IdToken}");
                Debug.Log($"授权码: {appleLoginSuccess.AuthorizationCode}");
            },
            (errorCode) =>
            {
                Debug.LogError($"Apple 登录失败，错误码: {errorCode}");
            }
        );
    }

    public void OnLogoutButtonClick()
    {
        _appleLoginComponent.LogOut();
    }
}
```

## 依赖

该包具有以下依赖项：

*   [com.gameframex.unity](https://github.com/gameframex/com.gameframex.unity.git)
*   [com.lupidan.apple-signin-unity](https://github.com/lupidan/apple-signin-unity.git)
