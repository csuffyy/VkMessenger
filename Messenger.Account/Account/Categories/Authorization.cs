using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using VkData.Account.Enums;
using VkData.Account.Interface;
using VkData.Account.Types;
using VkData.Helpers;
using VkData.Interface;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using PhotoSize = VkData.Account.Enums.PhotoSize;
using Timer = System.Timers.Timer;

namespace VkData.Account.Categories
{
    public class Authorization :
        AccountService
            <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo,
                StickerSize, PhotoSize>,
        IAuthorization
    {
        public Authorization(IVkAccount Account) : base(Account)
        {
            Account.Authorized += () =>
            {
                Account.IsAuthorized = true;
                Account.Refresh();
            };
        }

        public void Authorize(IUserSettings settings, bool isAsync)
        {
            if (isAsync)
                AuthorizeUseTask(settings);
            else
                Authorize(settings);
        }


        public void AuthorizeUseTask(
            IUserSettings settings)
        {
            Task.Factory.StartNew(
                () => Authorize(settings),
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        public void Authorize(
            IUserSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings), "UserSettings = null");

            Authorize(
                new ApiAuthParams
                {
                    Login = settings.GetSecureLogin.Unprotect(),
                    Password = settings.GetSecurePassword.Unprotect(),
                    ApplicationId = settings.GetAppId()
                },
                settings.CaptchaHandler,
                Account.Callbacks.OnApiException,
                Account.Callbacks.OnAuthorizationException);
        }

        public void Authorize(
            ApiAuthParams @params,
            Func<CaptchaNeededException, string> captchaHandler,
            Action onApiException,
            Action<Exception> onAuthorizationException)
        {
            @params.Settings = Settings.All;
            try
            {
                Account.VkApi.Authorize(@params);
            }
            catch (CaptchaNeededException e)
            {
                if (captchaHandler == null)
                    throw;
                @params.CaptchaSid = e.Sid;
                @params.CaptchaKey = captchaHandler(e);
                Account.VkApi.Authorize(@params);
            }
            catch (VkApiAuthorizationException e)
            {
                onAuthorizationException?.Invoke(e);
                return;
            }
            catch (WebException)
            {
                var t = new Timer(2000);
                t.Start();
                Authorize(@params, captchaHandler, onApiException, onAuthorizationException);
                return;
            }
            catch (VkApiException)
            {
                if (onApiException == null)
                    throw;
                onApiException();
            }
            catch (Exception e)
            {
                Account.Logger.Log(e);
                throw;
            }
            Account.RaiseUserAuthorized();
        }

        public async Task<Image> GetCaptcha(CaptchaNeededException e)
            => new Bitmap(await Account.Downloader.DownloadAsync(e.Img, Path.GetTempFileName()));
    }
}