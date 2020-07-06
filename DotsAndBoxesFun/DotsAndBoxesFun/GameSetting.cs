using DotsAndBoxesFun.Views;
using System;
using Xamarin.Forms;

namespace DotsAndBoxesFun
{
    public enum GameMode { Classic, Challenge};

    public class Setting
    {
        public static void Reset()
        {
            DependencyService.Get<IUserPreferences>().SetString(SettingKey.BoardSize.ToString(), string.Empty);
            DependencyService.Get<IUserPreferences>().SetString(SettingKey.DifficulyLevel.ToString(), string.Empty);
            DependencyService.Get<IUserPreferences>().SetString(SettingKey.FirstMove.ToString(), string.Empty);
            DependencyService.Get<IUserPreferences>().SetString(SettingKey.ChallengeLevel.ToString(), string.Empty);
            DependencyService.Get<IUserPreferences>().SetString(SettingKey.IsMute.ToString(), string.Empty);
            DependencyService.Get<IUserPreferences>().SetString(SettingKey.StarCount.ToString(), string.Empty);
        }

        public static void Set<T>(SettingKey key, T value)
        {
            DependencyService.Get<IUserPreferences>().SetString(key.ToString(), value.ToString());
        }

        public static string Get(SettingKey key)
        {
            return DependencyService.Get<IUserPreferences>().GetString(key.ToString());
        }
    }

    public enum SettingKey
    {
        BoardSize,
        DifficulyLevel,
        FirstMove,        
        ChallengeLevel,
        IsMute,
        StarCount,
        Player1Name,
        Player2Name,
        Is2Players
    }

    public static class ClasicGameSetting
    {
        private static int _boardSize = 6;
        public static int BoardSize
        {
            get { return _boardSize; }
            set
            {
                _boardSize = value;
                Setting.Set(SettingKey.BoardSize, _boardSize);
            }
        }

        private static DifficultyLevel _difficultyLevel = DotsAndBoxesFun.DifficultyLevel.Medium;
        public static DifficultyLevel DifficultyLevel
        {
            get { return _difficultyLevel; }
            set
            {
                _difficultyLevel = value;
                Setting.Set(SettingKey.DifficulyLevel, _difficultyLevel);
            }
        }

        private static bool _firstMove = false;
        public static bool FirstMove
        {
            get { return _firstMove; }
            set
            {
                _firstMove = value;
                Setting.Set(SettingKey.FirstMove, _firstMove);
            }
        }

        private static bool _is2Players = false;
        public static bool Is2Players
        {
            get { return _is2Players; }
            set
            {
                _is2Players = value;
                Setting.Set(SettingKey.Is2Players, _is2Players);
            }
        }

        static ClasicGameSetting()
        {
            var data = Setting.Get(SettingKey.BoardSize);
            if (!string.IsNullOrWhiteSpace(data))
                _boardSize = int.Parse(data);

            data = Setting.Get(SettingKey.DifficulyLevel);
            if (!string.IsNullOrWhiteSpace(data))
                _difficultyLevel = (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), data);
            else
                _difficultyLevel = DifficultyLevel.Medium;

            data = Setting.Get(SettingKey.FirstMove);
            if (!string.IsNullOrWhiteSpace(data))
                _firstMove = bool.Parse(data);

            data = Setting.Get(SettingKey.Is2Players);
            if (!string.IsNullOrWhiteSpace(data))
                _is2Players = bool.Parse(data);
        }
    }

    public static class ChallengeGameSetting
    {
        public static int BoardSize { get; } = 5;

        private static int _challengeLevel = 1;
        public static int ChallengeLevel
        {
            get { return _challengeLevel; }
            set
            {
                _challengeLevel = value;
                Setting.Set(SettingKey.ChallengeLevel, _challengeLevel);
            }
        }

        public static int RequestedChallengeLevel { get; set; }

        private static int _starCount = 0;
        public static int StarCount
        {
            get { return _starCount; }
            set
            {
                _starCount = value;
                Setting.Set(SettingKey.StarCount, _starCount);
            }
        }

        public static DifficultyLevel DifficultyLevel { get; set; }

        public static int TargetScore { get; set; }

        static ChallengeGameSetting()
        {
            var data = Setting.Get(SettingKey.StarCount);
            if (!string.IsNullOrWhiteSpace(data))
                _starCount = int.Parse(data);

            data = Setting.Get(SettingKey.ChallengeLevel);
            if (!string.IsNullOrWhiteSpace(data))
                _challengeLevel = int.Parse(data);
        }
    }

    public static class GlobalSetting
    {
        private static bool _isMute = false;
        public static bool IsMute
        {
            get
            {
                return _isMute;
            }
            set
            {
                _isMute = value;
                Setting.Set(SettingKey.IsMute, _isMute);
            }
        }

        private static string _player1Name = "Player 1";
        public static string Player1Name
        {
            get
            {
                return _player1Name;
            }
            set
            {
                _player1Name = value;
                Setting.Set(SettingKey.Player1Name, _player1Name);
            }
        }

        private static string _player2Name = "Player 2";
        public static string Player2Name
        {
            get
            {
                return _player2Name;
            }
            set
            {
                _player2Name = value;
                Setting.Set(SettingKey.Player2Name, _player2Name);
            }
        }

        public static GameMode CurrentGameMode { get; set; } = GameMode.Classic;

        static GlobalSetting()
        {
            var data = Setting.Get(SettingKey.IsMute);
            if (!string.IsNullOrWhiteSpace(data))
                _isMute = bool.Parse(data);

            data = Setting.Get(SettingKey.Player1Name);
            if (!string.IsNullOrWhiteSpace(data))
                _player1Name = data;

            data = Setting.Get(SettingKey.Player2Name);
            if (!string.IsNullOrWhiteSpace(data))
                _player2Name = data;
        }
    }

    public class ClassicGameRunner : IGame
    {
        public void Start()
        {
            try
            {
                GlobalSetting.CurrentGameMode = GameMode.Classic;
                Application.Current.MainPage = new NavigationPage(new ClassicGame());
            }
            catch (Exception ex)
            {

            }
        }
    }

    public class ChallengeGameRunner : IGame
    {
        private int requestedLevel = 0;
        public ChallengeGameRunner()
        {

        }
        public ChallengeGameRunner(int requestedLevel)
        {
            this.requestedLevel = requestedLevel;
        }
        public void Start()
        {
            try
            {
                GlobalSetting.CurrentGameMode = GameMode.Challenge;
                ChallengeGameSetting.RequestedChallengeLevel = requestedLevel == 0 ? ChallengeGameSetting.ChallengeLevel : requestedLevel;
                ChallengeGameSetting.DifficultyLevel = (DifficultyLevel)((ChallengeGameSetting.RequestedChallengeLevel - 1) % 3);// == 0 ? 2 : (ChallengeGameSetting.ChallengeLevel % 3) - 1);
                ChallengeGameSetting.TargetScore = (ChallengeGameSetting.BoardSize * ChallengeGameSetting.BoardSize) / 2 + ChallengeGameSetting.RequestedChallengeLevel;
                Application.Current.MainPage = new NavigationPage(new ChallengeGame());
            }
            catch (Exception ex)
            {

            }
        }
    }

    public interface IGame
    {
        void Start();
    }
}
