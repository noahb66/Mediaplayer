using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;


namespace Mediaplayer
{
    public partial class Form1 : Form
    {
        List<string> filteredFiles = new List<string>();
        FolderBrowserDialog browser = new FolderBrowserDialog();
        int currentFile = 0;


        public Form1()
        {
            InitializeComponent();
        }

        private void LoadFolderEvent(object sender, EventArgs e)
        {
            VideoPlayer.Ctlcontrols.stop();

            if (filteredFiles.Count > 1)
            {
                filteredFiles.Clear();
                filteredFiles = null;

                Playlist.Items.Clear();

                currentFile = 0;
            }

            DialogResult results = browser.ShowDialog();
            
            if (results == DialogResult.OK)
            {
                filteredFiles = Directory.GetFiles(browser.SelectedPath, "*.*").Where
                          (file => file.ToLower().EndsWith("webm") ||
                                   file.ToLower().EndsWith("mp4") ||
                                   file.ToLower().EndsWith("wmv") ||
                                   file.ToLower().EndsWith("mkv") ||
                                   file.ToLower().EndsWith("avi")).ToList();

                LoadPlaylist();
            }


        }

        private void ShowAboutEvent(object sender, EventArgs e)
        {
            MessageBox.Show("This app is made by Noah following MOO ICT"
                            + Environment.NewLine +
                            "Hope you enjoyed the simple media player" + 
                              Environment.NewLine + 
                            "Click on Open Folder Button to load the video" +
                            " folder to the app and it will start auto playing"
                            + Environment.NewLine + 
                            "Enjoy");
        }

        private void MediaPlayerStateChangeEvent(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 0)
            {
                // undefined loaded
                lblDuration.Text = "Media Player is ready to be loaded";
            }
            else if (e.newState == 1)
            {
                // if file is stopped
                lblDuration.Text = "Media Player is stopped";
            }
            else if (e.newState == 3)
            {
                // if the file is playing
                lblDuration.Text = "Duration: " + VideoPlayer.currentMedia.durationString;
            }
            else if (e.newState == 8)
            {
                // media has ended here
                if (currentFile >= filteredFiles.Count - 1)
                {
                    currentFile = 0;
                }
                else
                {
                    currentFile += 1;
                }

                Playlist.SelectedIndex = currentFile;

                ShowFileName(fileName);

            }
            else if (e.newState == 9)
            {
                // if the media player is loading new video
                lblDuration.Text = "Loading new video";
            }
            else if (e.newState == 10)
            {
                // media is ready to play again
                timer1.Start();
            }



        }

        private void PlaylistChanged(object sender, EventArgs e)
        {
            currentFile = Playlist.SelectedIndex;
            PlayFile(Playlist.SelectedItem.ToString());
            ShowFileName(fileName);
        }

        private void TimerEvent(object sender, EventArgs e)
        {
            VideoPlayer.Ctlcontrols.play();
            timer1.Stop();
        }

        private void LoadPlaylist()
        {
            VideoPlayer.currentPlaylist = VideoPlayer.newPlaylist("Playlist", "");
            
            foreach (string videos in filteredFiles)
            {
                VideoPlayer.currentPlaylist.appendItem(VideoPlayer.newMedia(videos));
                Playlist.Items.Add(videos);
            }

            if (filteredFiles.Count > 0)
            {
                fileName.Text = "Files Found " + filteredFiles.Count;

                Playlist.SelectedIndex = currentFile;
                PlayFile(Playlist.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("No video files found in this fodler");
            }
        }
        private void PlayFile(string url)
        {
            VideoPlayer.URL = url;
        }
        private void ShowFileName(Label name)
        {
            string file = Path.GetFileName(Playlist.SelectedItem.ToString());
            name.Text = "Currently Playing: " + file;
        }
    }
}
