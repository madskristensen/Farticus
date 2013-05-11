using EnvDTE80;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media;

namespace LigerShark.Farticus
{
    internal class FartPlayer
    {
        private static string _folder = GetFolderName();

        public static void PlayRandomFart(DTE2 dte)
        {
            string folder = FartPlayer.GetFolderName();
            string[] files = Directory.GetFiles(folder, "*.mp3", SearchOption.TopDirectoryOnly);

            Random rn = new Random(DateTime.Now.Millisecond);
            int index = rn.Next(files.Length);

            string fart = files[index];
            
            PlayFart(fart);
            dte.StatusBar.Text = "Playing " + Path.GetFileNameWithoutExtension(fart);
        }

        public static void PlayFart(FartOptions options)
        {
            if (options.Enabled)
            {
                string fileName = options.SelectedFart.ToString() + ".mp3";
                PlayFart(fileName);
            }
        }

        private static void PlayFart(string fileName)
        {
            string absolute = Path.Combine(_folder, fileName);

            new System.Threading.Thread(() =>
            {
                MediaPlayer _player = new MediaPlayer();
                _player.MediaEnded += ClosePlayer;
                _player.MediaFailed += ClosePlayer;

                _player.Open(new Uri(absolute, UriKind.Absolute));
                _player.Play();

            }).Start();
        }

        private static void ClosePlayer(object sender, EventArgs e)
        {
            var player = (MediaPlayer)sender;

            if (player != null)
            {
                player.Stop();
                player.Close();
            }
        }
                
        public static string GetFolderName()
        {
            string assembly = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(assembly) + "\\audio";
        }
    }
}
