// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
// 
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
// 
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
// 
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
// 
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System;
using GameFrameX.Runtime;
using UnityEngine;

namespace GameFrameX.Login.Apple.Runtime
{
    /// <summary>
    /// Apple登录组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("GameFrameX/AppleLogin")]
    [UnityEngine.Scripting.Preserve]
    [RequireComponent(typeof(GameFrameXAppleLoginCroppingHelper))]
    public class AppleLoginComponent : GameFrameworkComponent
    {
        private IAppleLoginManager _AppleLoginManager = null;

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
                Log.Fatal("Apple login manager is invalid.");
                return;
            }
        }

        /// <summary>
        /// 初始化 Apple 登录模块。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public void Init()
        {
            _AppleLoginManager.Init();
        }

        /// <summary>
        /// 发起 Apple 登录流程。
        /// </summary>
        /// <param name="loginSuccess">登录成功回调，携带 Apple 登录成功信息。</param>
        /// <param name="loginFail">登录失败回调，携带错误描述信息。</param>
        [UnityEngine.Scripting.Preserve]
        public void Login(Action<AppleLoginSuccess> loginSuccess, Action<string> loginFail)
        {
            _AppleLoginManager.Login(loginSuccess, loginFail);
        }

        /// <summary>
        /// 执行 Apple 登出操作。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public void LogOut()
        {
            _AppleLoginManager.LogOut();
        }
    }
}