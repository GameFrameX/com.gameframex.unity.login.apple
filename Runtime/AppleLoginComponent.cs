// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System;
using GameFrameX.Runtime;
using UnityEngine;

namespace GameFrameX.Login.Apple.Runtime
{
    /// <summary>
    /// Apple登录组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/AppleLogin")]
    [UnityEngine.Scripting.Preserve]
    public class AppleLoginComponent : GameFrameworkComponent
    {
        private const int DefaultPriority = 0;

        private IAppleLoginManager _AppleLoginManager = null;

        /// <summary>
        ///  Apple 登录 Web ClientId
        /// </summary>
        [SerializeField] private string m_ProjectId = string.Empty;

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            ImplementationComponentType = Utility.Assembly.GetType(componentType);
            InterfaceComponentType = typeof(IAppleLoginManager);
            base.Awake();

            _AppleLoginManager = GameFrameworkEntry.GetModule<IAppleLoginManager>();
            if (_AppleLoginManager == null)
            {
                Log.Fatal("Red system manager is invalid.");
                return;
            }
        }

        [UnityEngine.Scripting.Preserve]
        public void Init()
        {
            _AppleLoginManager.Init();
        }

        [UnityEngine.Scripting.Preserve]
        public void Login(Action<AppleLoginSuccess> loginSuccess, Action<int> loginFail)
        {
            _AppleLoginManager.Login(loginSuccess, loginFail);
        }

        [UnityEngine.Scripting.Preserve]
        public void LogOut()
        {
            _AppleLoginManager.LogOut();
        }

        // internal void ProcessAuthentication(SignInStatus status)
        // {
        //     if (status == SignInStatus.Success)
        //     {
        //         // Continue with Play Games Services
        //     }
        //     else
        //     {
        //         // 停用与 Play 游戏服务的集成或显示登录按钮 以请求用户登录。单击它应该会调用
        //         PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
        //     }
        // }
    }
}