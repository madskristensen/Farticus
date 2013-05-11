using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LigerShark.Farticus
{
    internal class FartPlayer
    {
        private static string _folder = GetFolderName();

        public static void Fart(string fileName)
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

        public static void Fart(FartOptions options)
        {
            if (options.Enabled)
            {
                string fileName = options.SelectedFart.ToString() + ".mp3";
                Fart(fileName);
            }
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
