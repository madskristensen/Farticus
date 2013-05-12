﻿using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Media;

namespace LigerShark.Farticus
{
    internal class FartPlayer
    {
        private static readonly string _folder = GetFolderName();

        public static void PlayRandomFart()
        {
            string folder = _folder;
            string[] files = Directory.GetFiles(folder, "*.mp3", SearchOption.TopDirectoryOnly);

            Random rn = new Random(DateTime.Now.Millisecond);
            int index = rn.Next(0, files.Length);

            PlayFart(files[index]);
        }

        public static void PlayErrorFart(FartOptions options)
        {
            if (options.Enabled)
            {
                string fileName = options.SelectedErrorFart.ToString() + ".mp3";
                PlayFart(fileName);
            }
        }
        public static void PlayWarningFart(FartOptions options)
        {
            if (options.Enabled)
            {
                string fileName = options.SelectedWarningFart.ToString() + ".mp3";
                PlayFart(fileName);
            }
        }
        public static void PlayMessageFart(FartOptions options)
        {
            if (options.Enabled)
            {
                string fileName = options.SelectedMessageFart.ToString() + ".mp3";
                PlayFart(fileName);
            }
        }

        private static void PlayFart(string fileName)
        {
            string absolute = Path.Combine(_folder, fileName);
            ThreadPool.QueueUserWorkItem(o => PlayAudio(absolute));
        }

        private static void PlayAudio(string absolute)
        {
            try
            {
                MediaPlayer player = new MediaPlayer();
                player.MediaEnded += ClosePlayer;
                player.MediaFailed += ClosePlayer;

                player.Open(new Uri(absolute, UriKind.Absolute));
                player.Play();
            }
            catch
            {
                // Catching any exception to avoid crashing VS
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

        private static string GetFolderName()
        {
            string assembly = Assembly.GetExecutingAssembly().Location;
            string folder = Path.GetDirectoryName(assembly);
            return Path.Combine(folder, "audio");
        }
    }
}