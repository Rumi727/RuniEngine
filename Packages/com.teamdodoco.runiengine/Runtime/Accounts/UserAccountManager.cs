#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Booting;
using RuniEngine.Datas;
using RuniEngine.Jsons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

namespace RuniEngine.Accounts
{
    public static class UserAccountManager
    {
        public static string accountsPath
        {
            get
            {
                string value = Path.Combine(Kernel.persistentDataPath, "Users");
                if (!Directory.Exists(value))
                    Directory.CreateDirectory(value);

                return value;
            }
        }

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
        static StorableClass[] _userData = null!;

        public static void Add(UserAccountInfo info)
        {
            List<UserAccountInfo> infos = accountList.ToList();
            infos.Add(info);

            File.WriteAllText(accountsInfoFilePath, JsonManager.ToJson(infos));
            ListRefresh();
        }

        public static void Remove(int index)
        {
            List<UserAccountInfo> infos = accountList.ToList();
            infos.RemoveAt(index);

            File.WriteAllText(accountsInfoFilePath, JsonManager.ToJson(infos));
            ListRefresh();
        }

        public static void ListRefresh()
        {
            if (File.Exists(accountsInfoFilePath))
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
                Logout();

            UserAccount? account = await UserAccount.Create(info, password);
            if (account == null)
                return false;

            currentAccount = account;
            UserDataLoad();

            await SceneManager.LoadSceneAsync(3).ToUniTask();
            return true;
        }

        public static void Logout()
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
