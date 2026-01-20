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
using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using GameFrameX.Runtime;
using UnityEngine;

namespace GameFrameX.Login.Apple.Runtime
{
    [UnityEngine.Scripting.Preserve]
    public sealed class AppleLoginManager : GameFrameworkModule, IAppleLoginManager
    {
        private const string AppleUserIdKey = "GameFrameX_" + nameof(AppleLoginManager) + "_AppleUserId";
        private const string AppleUserEmailKey = "GameFrameX_" + nameof(AppleLoginManager) + "_AppleUserEmail";
        private const string AppleUserDisplayNameKey = "GameFrameX_" + nameof(AppleLoginManager) + "_AppleUserDisplayName";
        private AppleAuthManager m_appleAuthManager;

        [UnityEngine.Scripting.Preserve]
        public AppleLoginManager()
        {
        }

        /// <summary>
        /// 初始化 Apple 登录组件。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public void Init()
        {
            if (AppleAuthManager.IsCurrentPlatformSupported)
            {
                // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
                var deserializer = new PayloadDeserializer();
                // Creates an Apple Authentication manager with the deserializer
                this.m_appleAuthManager = new AppleAuthManager(deserializer);
            }
        }

        /// <summary>
        /// 登录 Apple 账号。
        /// </summary>
        /// <param name="loginSuccess">登录成功回调。</param>
        /// <param name="loginFail">登录失败回调。</param>
        [UnityEngine.Scripting.Preserve]
        public void Login(Action<AppleLoginSuccess> loginSuccess, Action<string> loginFail)
        {
            var appleLoginSuccess = new AppleLoginSuccess();
#if UNITY_EDITOR
            appleLoginSuccess.PlayerId = SystemInfo.deviceUniqueIdentifier;
            appleLoginSuccess.Email = "test_login@apple.com";
            appleLoginSuccess.DisplayName = "Editor_Apple_Test";
            loginSuccess?.Invoke(appleLoginSuccess);
            return;
#endif
            var appleUserId = PlayerPrefs.GetString(AppleUserIdKey, string.Empty);
            var appleUserEmail = PlayerPrefs.GetString(AppleUserEmailKey, string.Empty);
            var appleUserDisplayName = PlayerPrefs.GetString(AppleUserDisplayNameKey, string.Empty);

            if (appleUserId.IsNotNullOrEmpty())
            {
                appleLoginSuccess.PlayerId = appleUserId;
                appleLoginSuccess.Email = appleUserEmail;
                appleLoginSuccess.DisplayName = appleUserDisplayName;
                loginSuccess?.Invoke(appleLoginSuccess);
                return;
            }

            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

            this.m_appleAuthManager.LoginWithAppleId(loginArgs, credential =>
                                                     {
                                                         // Obtained credential, cast it to IAppleIDCredential
                                                         if (credential is IAppleIDCredential appleIdCredential)
                                                         {
                                                             // Apple User ID
                                                             // You should save the user ID somewhere in the device
                                                             var userId = appleIdCredential.User;
                                                             PlayerPrefs.SetString(AppleUserIdKey, userId);
                                                             appleLoginSuccess.PlayerId = userId;
                                                             // Email (Received ONLY in the first login)
                                                             var email = appleIdCredential.Email;
                                                             appleLoginSuccess.Email = email;
                                                             PlayerPrefs.SetString(AppleUserEmailKey, email);
                                                             // Full name (Received ONLY in the first login)
                                                             var fullName = appleIdCredential.FullName;
                                                             appleLoginSuccess.DisplayName = fullName?.Nickname;
                                                             PlayerPrefs.SetString(AppleUserDisplayNameKey, appleLoginSuccess.DisplayName);
                                                             // Identity token
                                                             var identityToken = Encoding.UTF8.GetString(
                                                                 appleIdCredential.IdentityToken,
                                                                 0,
                                                                 appleIdCredential.IdentityToken.Length);
                                                             appleLoginSuccess.IdToken = identityToken;
                                                             // Authorization code
                                                             var authorizationCode = Encoding.UTF8.GetString(
                                                                 appleIdCredential.AuthorizationCode,
                                                                 0,
                                                                 appleIdCredential.AuthorizationCode.Length);
                                                             appleLoginSuccess.AuthorizationCode = authorizationCode;
                                                             PlayerPrefs.Save();
                                                             // And now you have all the information to create/login a user in your system
                                                             loginSuccess?.Invoke(appleLoginSuccess);
                                                         }
                                                     },
                                                     error =>
                                                     {
                                                         // Something went wrong
                                                         var authorizationErrorCode = error.GetAuthorizationErrorCode();
                                                         loginFail?.Invoke(authorizationErrorCode.ToString());
                                                     });
        }

        /// <summary>
        /// 退出登录 Apple 账号。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public void LogOut()
        {
            PlayerPrefs.DeleteKey(AppleUserIdKey);
            PlayerPrefs.DeleteKey(AppleUserEmailKey);
            PlayerPrefs.DeleteKey(AppleUserDisplayNameKey);
            PlayerPrefs.Save();
        }

        protected override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (this.m_appleAuthManager != null)
            {
                this.m_appleAuthManager.Update();
            }
        }

        protected override void Shutdown()
        {
        }
    }
}