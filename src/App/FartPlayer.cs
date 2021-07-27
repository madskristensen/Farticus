using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace LigerShark.Farticus
{
    internal class FartPlayer
    {
        private static readonly MediaPlayer _player = new MediaPlayer();
        private static readonly string[] _files;

        static FartPlayer()
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var audio = Path.Combine(folder, "audio");

            _files = Directory.GetFiles(audio, "*.mp3", SearchOption.TopDirectoryOnly);
            _player.MediaEnded += (s, e) => _player.Close();
        }

        public static void PlayFart(Farts fart)
        {
            if (fart == Farts.SilentButDeadly)
            {
                return;
            }

            if (fart == Farts.RandomFart)
            {
                var rn = new Random();
                var index = rn.Next(0, _files.Length);

                PlayFart(_files[index]);
            }
            else
            {
                var fileName = _files.FirstOrDefault(f => f.EndsWith("\\" + fart + ".mp3", StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(fileName))
                {
                    PlayFart(fileName);
                }
            }
        }

        private static void PlayFart(string fileName)
        {
            _player.Open(new Uri(fileName, UriKind.Absolute));
            _player.Play();
        }
    }
}