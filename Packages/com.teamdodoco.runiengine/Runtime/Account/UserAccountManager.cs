#nullable enable
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RuniEngine.Booting;
using RuniEngine.Data;
using RuniEngine.Json;
using RuniEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RuniEngine.Account
{
    public static class UserAccountManager
    {
        [ProjectData]
        public struct ProjectData
        {
            [JsonProperty] public static string loginScenePath { get; set; } = "Packages/com.teamdodoco.runiengine/Runtime/Account/Default Login Scene.unity";
        }

        public static string accountsPath => Path.Combine(Kernel.persistentDataPath, "Users");
        public static string accountsInfoFilePath => Path.Combine(accountsPath, "accounts.json");

        public static UserAccount? currentAccount { get; private set; }

        public static UserAccountInfo[] accountList { get; private set; } = new UserAccountInfo[0];

        public static StorableClass[] userData
        {
            get
            {
                BasicDataNotLoadedException.Exception();
                return _userData;
            }
        }
        private static StorableClass[] _userData = null!;

        public static void Add(UserAccountInfo info)
        {
            List<UserAccountInfo> infos = accountList.ToList();
            infos.Add(info);

            File.WriteAllText(accountsInfoFilePath, JsonManager.ToJson(infos));
        }

        public static void Remove(int index)
        {
            List<UserAccountInfo> infos = accountList.ToList();
            infos.RemoveAt(index);

            File.WriteAllText(accountsInfoFilePath, JsonManager.ToJson(infos));
        }

        public static void ListRefresh()
        {
            if (File.Exists(accountsPath))
            {
                try
                {
                    UserAccountInfo[]? result = JsonManager.JsonRead<UserAccountInfo[]>(accountsInfoFilePath);
                    if (result != null)
                        accountList = result;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    accountList = new UserAccountInfo[0];
                }
            }
            else
                accountList = new UserAccountInfo[0];
        }

        public static async UniTask<bool> Login(UserAccountInfo info, string password)
        {
            BasicDataNotLoadedException.Exception();

            if (currentAccount != null)
                LogoutNoSceneMove();

            UserAccount? account = await UserAccount.Create(info, password);
            if (account == null)
                return false;

            currentAccount = account;
            UserDataLoad();

            await SceneManager.LoadScene(3);
            return true;
        }

        public static async UniTask Logout()
        {
            BasicDataNotLoadedException.Exception();

            if (currentAccount == null)
                throw LogoutException.AlreadyLoggedException();

            await SceneManager.LoadScene(2);
            LogoutNoSceneMove();
        }

        public static void LogoutNoSceneMove()
        {
            BasicDataNotLoadedException.Exception();

            if (currentAccount == null)
                throw LogoutException.AlreadyLoggedException();

            UserDataSave();
            UserDataSetDefault();

            currentAccount.Dispose();
            currentAccount = null;
        }

        public static void UserDataInit() => _userData = StorableClassUtility.AutoInitialize<UserDataAttribute>();

        public static void UserDataSave()
        {
            BasicDataNotLoadedException.Exception();

            if (currentAccount == null)
                throw LogoutException.LoggedOutUserDataException();

            StorableClassUtility.SaveAll(_userData, currentAccount.path);
        }

        public static void UserDataLoad()
        {
            BasicDataNotLoadedException.Exception();

            if (currentAccount == null)
                throw LogoutException.LoggedOutUserDataException();

            StorableClassUtility.LoadAll(_userData, currentAccount.path);
        }

        public static void UserDataSetDefault()
        {
            BasicDataNotLoadedException.Exception();

            if (currentAccount == null)
                throw LogoutException.LoggedOutUserDataException();

            StorableClassUtility.SetDefaultAll(_userData);
        }
    }
}
