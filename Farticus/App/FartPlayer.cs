using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace LigerShark.Farticus
{
    internal class FartPlayer
    {
        private static MediaPlayer _player = new MediaPlayer();
        private static string[] _files;

        static FartPlayer()
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string audio = Path.Combine(folder, "audio");

            _files = Directory.GetFiles(audio, "*.mp3", SearchOption.TopDirectoryOnly);
            _player.MediaEnded += (s, e) => _player.Close();
        }

        public static void PlayFart(Farts fart)
        {
            if (fart == Farts.RandomFart)
            {
                Random rn = new Random();
                int index = rn.Next(0, _files.Length);

                PlayFart(_files[index]);
            }
            else
            {
                PlayFart(fart + ".mp3");
            }
        }

        private static void PlayFart(string fileName)
        {
            string absolute = _files.FirstOrDefault(f => f.EndsWith("\\" + fileName, StringComparison.OrdinalIgnoreCase));

            _player.Open(new Uri(absolute, UriKind.Absolute));
            _player.Play();
        }
    }
}