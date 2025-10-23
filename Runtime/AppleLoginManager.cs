// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

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


        [UnityEngine.Scripting.Preserve]
        public void Login(Action<AppleLoginSuccess> loginSuccess, Action<int> loginFail)
        {
            var appleLoginSuccess = new AppleLoginSuccess();

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
                        // And now you have all the information to create/login a user in your system
                        loginSuccess?.Invoke(appleLoginSuccess);
                    }
                },
                error =>
                {
                    // Something went wrong
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    loginFail?.Invoke((int)authorizationErrorCode);
                });
        }

        [UnityEngine.Scripting.Preserve]
        public void LogOut()
        {
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