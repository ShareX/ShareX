using HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX
{
    public partial class VideoEncodersForm : Form
    {
        public VideoEncodersForm()
        {
            InitializeComponent();
        }

        private void btnEncoderDuplicate_Click(object sender, EventArgs e)
        {
            var encoders = lvEncoders.SelectedItems.Cast<ListViewItem>().Select(x => ((VideoEncoder)x.Tag).Copy()).ToList();
            encoders.ForEach(x => AddVideoEncoder(x));
            encoders.ForEach(x => Program.Settings.VideoEncoders.Add(x));
        }

        private void lvEncoders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnEncodersEdit_Click(sender, e);
        }

        private void btnEncodersAdd_Click(object sender, EventArgs e)
        {
            using (EncoderProgramForm form = new EncoderProgramForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    VideoEncoder encoder = form.encoder;
                    Program.Settings.VideoEncoders.Add(encoder);
                    AddVideoEncoder(encoder);
                }
            }
        }

        private void btnEncodersEdit_Click(object sender, EventArgs e)
        {
            if (lvEncoders.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvEncoders.SelectedItems[0];
                VideoEncoder encoder = lvi.Tag as VideoEncoder;

                using (EncoderProgramForm form = new EncoderProgramForm(encoder))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        lvi.Text = encoder.Name ?? "";
                        lvi.SubItems[1].Text = encoder.Path ?? "";
                        lvi.SubItems[2].Text = encoder.Args ?? "";
                        lvi.SubItems[3].Text = encoder.OutputExtension ?? "";
                    }
                }
            }
        }

        private void btnEncodersRemove_Click(object sender, EventArgs e)
        {
            if (lvEncoders.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvEncoders.SelectedItems[0];
                VideoEncoder encoder = lvi.Tag as VideoEncoder;

                Program.Settings.VideoEncoders.Remove(encoder);
                lvEncoders.Items.Remove(lvi);
            }
        }

        private void AddVideoEncoder(VideoEncoder encoder)
        {
            ListViewItem lvi = new ListViewItem(encoder.Name ?? "");
            lvi.Tag = encoder;
            lvi.SubItems.Add(encoder.Path ?? "");
            lvi.SubItems.Add(encoder.Args ?? "");
            lvi.SubItems.Add(encoder.OutputExtension ?? "");
            lvEncoders.Items.Add(lvi);
        }

        private void VideoEncodersForm_Load(object sender, EventArgs e)
        {
            if (Program.Settings.VideoEncoders.Count == 0)
            {
                Program.Settings.VideoEncoders.Add(new VideoEncoder() { Name = "Encode using x264.exe to H.264", Path = "x264.exe", Args = "--output %output %input", OutputExtension = "mp4" });
                Program.Settings.VideoEncoders.Add(new VideoEncoder() { Name = "Encode using ffmpeg.exe to WebM", Path = "ffmpeg.exe", Args = "-i %input -c:v libvpx -crf 12 -b:v 500K %output", OutputExtension = "webm" });
                Program.Settings.VideoEncoders.Add(new VideoEncoder() { Name = "Change container to MP4 using ffmpeg.exe", Path = "ffmpeg.exe", Args = "-i %input -c:v copy %output", OutputExtension = "mp4" });
                Program.Settings.VideoEncoders.Add(new VideoEncoder() { Name = "Optimize GIF using gifsicle.exe", Path = "gifsicle.exe", Args = "-O2 %input -o %output", OutputExtension = "gif" });
            }

            Program.Settings.VideoEncoders.ForEach(x => AddVideoEncoder(x));
        }
    }
}