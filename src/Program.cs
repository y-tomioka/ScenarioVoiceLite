using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STVoice
{
    internal static class Program
    {
        static public Logger logger = LogManager.GetCurrentClassLogger();

        public static void InitializeLogger()
        {
            // LoggingConfigurationを生成 
            var config = new LoggingConfiguration();

            // FileTargetを生成し LoggingConfigurationに設定 
            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // fileTargetのプロパティを設定
            fileTarget.Name = "f";
            fileTarget.FileName = "${basedir}/log/ScenarioVoice_${shortdate}.log";
            fileTarget.Layout = "${longdate} [${uppercase:${level}}] ${message}";

            // LoggingRuleを定義
            var rule1 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule1);

            // 設定を有効化
            LogManager.Configuration = config;
        }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            InitializeLogger();
            bool ret = healthCheck();
            if(ret == false)
            {
                return;
            }
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            logger.Info("ScenarioVoiceを起動しました。");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(logger));
        }

        private static bool healthCheck()
        {
            WebControl webControl = new WebControl(logger);
            string port = gerPortNumber();
            string url = "http://127.0.0.1:" + port + "/version";
            string speakers = webControl.get(url);
            if (speakers == "")
            {
                logger.Error("VOICEVOXが起動していないため、アプリを起動できません。");
                //MessageBox.Show("VOICEVOXを起動してからアプリを起動してください。");
                DialogResult result = MessageBox.Show("VOICEVOXを起動してからアプリを起動してください。\nVOICEVOXのダウンロードがまだの方は「はい」をクリックしてください", "確認", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://voicevox.hiroshiba.jp/");
                }
                return false;
            }
            return true;
        }

        private static string gerPortNumber()
        {
            return ConfigurationManager.AppSettings["port"];
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            try
            {
                Exception ex = (Exception)args.ExceptionObject;
                logger.Error("例外が発生しました。" + ex.Message + ex.Source + ex.StackTrace);
                MessageBox.Show("例外が発生しました。詳細は、ログをご確認ください。");
            }
            finally
            {
                Application.Exit(); // 明示的に終了させる必要がある
            }
        }
    }
}
