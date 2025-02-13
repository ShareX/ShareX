#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class GradientPickerForm : Form
    {
        public delegate void GradientChangedEventHandler();
        public event GradientChangedEventHandler GradientChanged;

        public GradientInfo Gradient { get; private set; }

        private bool isReady;

        public GradientPickerForm(GradientInfo gradient)
        {
            Gradient = gradient;

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            cbGradientType.Items.AddRange(Helpers.GetLocalizedEnumDescriptions<LinearGradientMode>());
            cbGradientType.SelectedIndex = (int)Gradient.Type;
            UpdateGradientList(true);
        }

        protected virtual void OnGradientChanged()
        {
            GradientChanged?.Invoke();
        }

        private void AddPresets()
        {
            GradientInfo[] gradients = new GradientInfo[]
            {
                new GradientInfo(Color.FromArgb(184, 11, 195), Color.FromArgb(98, 54, 255)),
                new GradientInfo(Color.FromArgb(255, 3, 135), Color.FromArgb(255, 143, 3)),
                new GradientInfo(Color.FromArgb(0, 187, 138), Color.FromArgb(0, 105, 163)),

                new GradientInfo(Color.FromArgb(152, 142, 148), Color.FromArgb(219, 211, 216)),
                new GradientInfo(Color.FromArgb(9, 30, 58), Color.FromArgb(47, 128, 237), Color.FromArgb(45, 158, 224)),
                new GradientInfo(Color.FromArgb(148, 0, 211), Color.FromArgb(75, 0, 130)),
                new GradientInfo(Color.FromArgb(200, 78, 137), Color.FromArgb(241, 95, 121)),
                new GradientInfo(Color.FromArgb(0, 245, 160), Color.FromArgb(0, 217, 245)),
                new GradientInfo(Color.FromArgb(247, 148, 30), Color.FromArgb(114, 198, 239), Color.FromArgb(0, 166, 81)),
                new GradientInfo(Color.FromArgb(247, 148, 30), Color.FromArgb(0, 78, 143)),
                new GradientInfo(Color.FromArgb(114, 198, 239), Color.FromArgb(0, 78, 143)),
                new GradientInfo(Color.FromArgb(253, 129, 18), Color.FromArgb(0, 133, 202)),
                new GradientInfo(Color.FromArgb(191, 90, 224), Color.FromArgb(168, 17, 218)),
                new GradientInfo(Color.FromArgb(0, 65, 106), Color.FromArgb(228, 229, 230)),
                new GradientInfo(Color.FromArgb(251, 237, 150), Color.FromArgb(171, 236, 214)),
                new GradientInfo(Color.FromArgb(255, 224, 0), Color.FromArgb(121, 159, 12)),
                new GradientInfo(Color.FromArgb(247, 248, 248), Color.FromArgb(172, 187, 120)),
                new GradientInfo(Color.FromArgb(0, 65, 106), Color.FromArgb(121, 159, 12), Color.FromArgb(255, 224, 0)),
                new GradientInfo(Color.FromArgb(51, 77, 80), Color.FromArgb(203, 202, 165)),
                new GradientInfo(Color.FromArgb(0, 82, 212), Color.FromArgb(67, 100, 247), Color.FromArgb(111, 177, 252)),
                new GradientInfo(Color.FromArgb(84, 51, 255), Color.FromArgb(32, 189, 255), Color.FromArgb(165, 254, 203)),
                new GradientInfo(Color.FromArgb(121, 159, 12), Color.FromArgb(172, 187, 120)),
                new GradientInfo(Color.FromArgb(255, 226, 89), Color.FromArgb(255, 167, 81)),
                new GradientInfo(Color.FromArgb(0, 65, 106), Color.FromArgb(228, 229, 230)),
                new GradientInfo(Color.FromArgb(255, 224, 0), Color.FromArgb(121, 159, 12)),
                new GradientInfo(Color.FromArgb(172, 182, 229), Color.FromArgb(134, 253, 232)),
                new GradientInfo(Color.FromArgb(83, 105, 118), Color.FromArgb(41, 46, 73)),
                new GradientInfo(Color.FromArgb(187, 210, 197), Color.FromArgb(83, 105, 118), Color.FromArgb(41, 46, 73)),
                new GradientInfo(Color.FromArgb(183, 152, 145), Color.FromArgb(148, 113, 107)),
                new GradientInfo(Color.FromArgb(151, 150, 240), Color.FromArgb(251, 199, 212)),
                new GradientInfo(Color.FromArgb(187, 210, 197), Color.FromArgb(83, 105, 118)),
                new GradientInfo(Color.FromArgb(7, 101, 133), Color.FromArgb(255, 255, 255)),
                new GradientInfo(Color.FromArgb(0, 70, 127), Color.FromArgb(165, 204, 130)),
                new GradientInfo(Color.FromArgb(0, 12, 64), Color.FromArgb(96, 125, 139)),
                new GradientInfo(Color.FromArgb(20, 136, 204), Color.FromArgb(43, 50, 178)),
                new GradientInfo(Color.FromArgb(236, 0, 140), Color.FromArgb(252, 103, 103)),
                new GradientInfo(Color.FromArgb(204, 43, 94), Color.FromArgb(117, 58, 136)),
                new GradientInfo(Color.FromArgb(33, 147, 176), Color.FromArgb(109, 213, 237)),
                new GradientInfo(Color.FromArgb(230, 92, 0), Color.FromArgb(249, 212, 35)),
                new GradientInfo(Color.FromArgb(43, 88, 118), Color.FromArgb(78, 67, 118)),
                new GradientInfo(Color.FromArgb(49, 71, 85), Color.FromArgb(38, 160, 218)),
                new GradientInfo(Color.FromArgb(119, 161, 211), Color.FromArgb(121, 203, 202), Color.FromArgb(230, 132, 174)),
                new GradientInfo(Color.FromArgb(255, 110, 127), Color.FromArgb(191, 233, 255)),
                new GradientInfo(Color.FromArgb(229, 45, 39), Color.FromArgb(179, 18, 23)),
                new GradientInfo(Color.FromArgb(96, 56, 19), Color.FromArgb(178, 159, 148)),
                new GradientInfo(Color.FromArgb(22, 160, 133), Color.FromArgb(244, 208, 63)),
                new GradientInfo(Color.FromArgb(211, 16, 39), Color.FromArgb(234, 56, 77)),
                new GradientInfo(Color.FromArgb(237, 229, 116), Color.FromArgb(225, 245, 196)),
                new GradientInfo(Color.FromArgb(2, 170, 176), Color.FromArgb(0, 205, 172)),
                new GradientInfo(Color.FromArgb(218, 34, 255), Color.FromArgb(151, 51, 238)),
                new GradientInfo(Color.FromArgb(52, 143, 80), Color.FromArgb(86, 180, 211)),
                new GradientInfo(Color.FromArgb(60, 165, 92), Color.FromArgb(181, 172, 73)),
                new GradientInfo(Color.FromArgb(204, 149, 192), Color.FromArgb(219, 212, 180), Color.FromArgb(122, 161, 210)),
                new GradientInfo(Color.FromArgb(0, 57, 115), Color.FromArgb(229, 229, 190)),
                new GradientInfo(Color.FromArgb(229, 93, 135), Color.FromArgb(95, 195, 228)),
                new GradientInfo(Color.FromArgb(64, 59, 74), Color.FromArgb(231, 233, 187)),
                new GradientInfo(Color.FromArgb(240, 152, 25), Color.FromArgb(237, 222, 93)),
                new GradientInfo(Color.FromArgb(255, 81, 47), Color.FromArgb(221, 36, 118)),
                new GradientInfo(Color.FromArgb(170, 7, 107), Color.FromArgb(97, 4, 95)),
                new GradientInfo(Color.FromArgb(26, 41, 128), Color.FromArgb(38, 208, 206)),
                new GradientInfo(Color.FromArgb(255, 81, 47), Color.FromArgb(240, 152, 25)),
                new GradientInfo(Color.FromArgb(29, 43, 100), Color.FromArgb(248, 205, 218)),
                new GradientInfo(Color.FromArgb(31, 162, 255), Color.FromArgb(18, 216, 250), Color.FromArgb(166, 255, 203)),
                new GradientInfo(Color.FromArgb(76, 184, 196), Color.FromArgb(60, 211, 173)),
                new GradientInfo(Color.FromArgb(221, 94, 137), Color.FromArgb(247, 187, 151)),
                new GradientInfo(Color.FromArgb(235, 51, 73), Color.FromArgb(244, 92, 67)),
                new GradientInfo(Color.FromArgb(29, 151, 108), Color.FromArgb(147, 249, 185)),
                new GradientInfo(Color.FromArgb(255, 128, 8), Color.FromArgb(255, 200, 55)),
                new GradientInfo(Color.FromArgb(22, 34, 42), Color.FromArgb(58, 96, 115)),
                new GradientInfo(Color.FromArgb(31, 28, 44), Color.FromArgb(146, 141, 171)),
                new GradientInfo(Color.FromArgb(97, 67, 133), Color.FromArgb(81, 99, 149)),
                new GradientInfo(Color.FromArgb(71, 118, 230), Color.FromArgb(142, 84, 233)),
                new GradientInfo(Color.FromArgb(8, 80, 120), Color.FromArgb(133, 216, 206)),
                new GradientInfo(Color.FromArgb(43, 192, 228), Color.FromArgb(234, 236, 198)),
                new GradientInfo(Color.FromArgb(19, 78, 94), Color.FromArgb(113, 178, 128)),
                new GradientInfo(Color.FromArgb(92, 37, 141), Color.FromArgb(67, 137, 162)),
                new GradientInfo(Color.FromArgb(117, 127, 154), Color.FromArgb(215, 221, 232)),
                new GradientInfo(Color.FromArgb(35, 37, 38), Color.FromArgb(65, 67, 69)),
                new GradientInfo(Color.FromArgb(28, 216, 210), Color.FromArgb(147, 237, 199)),
                new GradientInfo(Color.FromArgb(61, 126, 170), Color.FromArgb(255, 228, 122)),
                new GradientInfo(Color.FromArgb(40, 48, 72), Color.FromArgb(133, 147, 152)),
                new GradientInfo(Color.FromArgb(36, 198, 220), Color.FromArgb(81, 74, 157)),
                new GradientInfo(Color.FromArgb(220, 36, 36), Color.FromArgb(74, 86, 157)),
                new GradientInfo(Color.FromArgb(237, 66, 100), Color.FromArgb(255, 237, 188)),
                new GradientInfo(Color.FromArgb(218, 226, 248), Color.FromArgb(214, 164, 164)),
                new GradientInfo(Color.FromArgb(236, 233, 230), Color.FromArgb(255, 255, 255)),
                new GradientInfo(Color.FromArgb(116, 116, 191), Color.FromArgb(52, 138, 199)),
                new GradientInfo(Color.FromArgb(236, 111, 102), Color.FromArgb(243, 161, 131)),
                new GradientInfo(Color.FromArgb(95, 44, 130), Color.FromArgb(73, 160, 157)),
                new GradientInfo(Color.FromArgb(192, 72, 72), Color.FromArgb(72, 0, 72)),
                new GradientInfo(Color.FromArgb(228, 58, 21), Color.FromArgb(230, 82, 69)),
                new GradientInfo(Color.FromArgb(65, 77, 11), Color.FromArgb(114, 122, 23)),
                new GradientInfo(Color.FromArgb(252, 53, 76), Color.FromArgb(10, 191, 188)),
                new GradientInfo(Color.FromArgb(75, 108, 183), Color.FromArgb(24, 40, 72)),
                new GradientInfo(Color.FromArgb(248, 87, 166), Color.FromArgb(255, 88, 88)),
                new GradientInfo(Color.FromArgb(167, 55, 55), Color.FromArgb(122, 40, 40)),
                new GradientInfo(Color.FromArgb(213, 51, 105), Color.FromArgb(203, 173, 109)),
                new GradientInfo(Color.FromArgb(233, 211, 98), Color.FromArgb(51, 51, 51)),
                new GradientInfo(Color.FromArgb(222, 98, 98), Color.FromArgb(255, 184, 140)),
                new GradientInfo(Color.FromArgb(102, 102, 0), Color.FromArgb(153, 153, 102)),
                new GradientInfo(Color.FromArgb(255, 238, 238), Color.FromArgb(221, 239, 187)),
                new GradientInfo(Color.FromArgb(239, 239, 187), Color.FromArgb(212, 211, 221)),
                new GradientInfo(Color.FromArgb(194, 21, 0), Color.FromArgb(255, 197, 0)),
                new GradientInfo(Color.FromArgb(33, 95, 0), Color.FromArgb(228, 228, 217)),
                new GradientInfo(Color.FromArgb(80, 201, 195), Color.FromArgb(150, 222, 218)),
                new GradientInfo(Color.FromArgb(97, 97, 97), Color.FromArgb(155, 197, 195)),
                new GradientInfo(Color.FromArgb(221, 214, 243), Color.FromArgb(250, 172, 168)),
                new GradientInfo(Color.FromArgb(93, 65, 87), Color.FromArgb(168, 202, 186)),
                new GradientInfo(Color.FromArgb(230, 218, 218), Color.FromArgb(39, 64, 70)),
                new GradientInfo(Color.FromArgb(242, 112, 156), Color.FromArgb(255, 148, 114)),
                new GradientInfo(Color.FromArgb(218, 210, 153), Color.FromArgb(176, 218, 185)),
                new GradientInfo(Color.FromArgb(211, 149, 155), Color.FromArgb(191, 230, 186)),
                new GradientInfo(Color.FromArgb(0, 210, 255), Color.FromArgb(58, 123, 213)),
                new GradientInfo(Color.FromArgb(135, 0, 0), Color.FromArgb(25, 10, 5)),
                new GradientInfo(Color.FromArgb(185, 147, 214), Color.FromArgb(140, 166, 219)),
                new GradientInfo(Color.FromArgb(100, 145, 115), Color.FromArgb(219, 213, 164)),
                new GradientInfo(Color.FromArgb(201, 255, 191), Color.FromArgb(255, 175, 189)),
                new GradientInfo(Color.FromArgb(96, 108, 136), Color.FromArgb(63, 76, 107)),
                new GradientInfo(Color.FromArgb(251, 211, 233), Color.FromArgb(187, 55, 125)),
                new GradientInfo(Color.FromArgb(173, 209, 0), Color.FromArgb(123, 146, 10)),
                new GradientInfo(Color.FromArgb(255, 78, 80), Color.FromArgb(249, 212, 35)),
                new GradientInfo(Color.FromArgb(240, 194, 123), Color.FromArgb(75, 18, 72)),
                new GradientInfo(Color.FromArgb(0, 0, 0), Color.FromArgb(231, 76, 60)),
                new GradientInfo(Color.FromArgb(170, 255, 169), Color.FromArgb(17, 255, 189)),
                new GradientInfo(Color.FromArgb(179, 255, 171), Color.FromArgb(18, 255, 247)),
                new GradientInfo(Color.FromArgb(120, 2, 6), Color.FromArgb(6, 17, 97)),
                new GradientInfo(Color.FromArgb(157, 80, 187), Color.FromArgb(110, 72, 170)),
                new GradientInfo(Color.FromArgb(85, 98, 112), Color.FromArgb(255, 107, 107)),
                new GradientInfo(Color.FromArgb(112, 225, 245), Color.FromArgb(255, 209, 148)),
                new GradientInfo(Color.FromArgb(0, 198, 255), Color.FromArgb(0, 114, 255)),
                new GradientInfo(Color.FromArgb(254, 140, 0), Color.FromArgb(248, 54, 0)),
                new GradientInfo(Color.FromArgb(82, 194, 52), Color.FromArgb(6, 23, 0)),
                new GradientInfo(Color.FromArgb(72, 85, 99), Color.FromArgb(41, 50, 60)),
                new GradientInfo(Color.FromArgb(131, 164, 212), Color.FromArgb(182, 251, 255)),
                new GradientInfo(Color.FromArgb(253, 252, 71), Color.FromArgb(36, 254, 65)),
                new GradientInfo(Color.FromArgb(171, 186, 171), Color.FromArgb(255, 255, 255)),
                new GradientInfo(Color.FromArgb(115, 200, 169), Color.FromArgb(55, 59, 68)),
                new GradientInfo(Color.FromArgb(211, 131, 18), Color.FromArgb(168, 50, 121)),
                new GradientInfo(Color.FromArgb(30, 19, 12), Color.FromArgb(154, 132, 120)),
                new GradientInfo(Color.FromArgb(148, 142, 153), Color.FromArgb(46, 20, 55)),
                new GradientInfo(Color.FromArgb(54, 0, 51), Color.FromArgb(11, 135, 147)),
                new GradientInfo(Color.FromArgb(255, 161, 127), Color.FromArgb(0, 34, 62)),
                new GradientInfo(Color.FromArgb(67, 206, 162), Color.FromArgb(24, 90, 157)),
                new GradientInfo(Color.FromArgb(255, 179, 71), Color.FromArgb(255, 204, 51)),
                new GradientInfo(Color.FromArgb(100, 65, 165), Color.FromArgb(42, 8, 69)),
                new GradientInfo(Color.FromArgb(254, 172, 94), Color.FromArgb(199, 121, 208), Color.FromArgb(75, 192, 200)),
                new GradientInfo(Color.FromArgb(131, 58, 180), Color.FromArgb(253, 29, 29), Color.FromArgb(252, 176, 69)),
                new GradientInfo(Color.FromArgb(255, 0, 132), Color.FromArgb(51, 0, 27)),
                new GradientInfo(Color.FromArgb(0, 191, 143), Color.FromArgb(0, 21, 16)),
                new GradientInfo(Color.FromArgb(19, 106, 138), Color.FromArgb(38, 120, 113)),
                new GradientInfo(Color.FromArgb(142, 158, 171), Color.FromArgb(238, 242, 243)),
                new GradientInfo(Color.FromArgb(123, 67, 151), Color.FromArgb(220, 36, 48)),
                new GradientInfo(Color.FromArgb(209, 145, 60), Color.FromArgb(255, 209, 148)),
                new GradientInfo(Color.FromArgb(241, 242, 181), Color.FromArgb(19, 80, 88)),
                new GradientInfo(Color.FromArgb(106, 145, 19), Color.FromArgb(20, 21, 23)),
                new GradientInfo(Color.FromArgb(0, 79, 249), Color.FromArgb(255, 249, 76)),
                new GradientInfo(Color.FromArgb(82, 82, 82), Color.FromArgb(61, 114, 180)),
                new GradientInfo(Color.FromArgb(186, 139, 2), Color.FromArgb(24, 24, 24)),
                new GradientInfo(Color.FromArgb(238, 156, 167), Color.FromArgb(255, 221, 225)),
                new GradientInfo(Color.FromArgb(48, 67, 82), Color.FromArgb(215, 210, 204)),
                new GradientInfo(Color.FromArgb(204, 204, 178), Color.FromArgb(117, 117, 25)),
                new GradientInfo(Color.FromArgb(44, 62, 80), Color.FromArgb(52, 152, 219)),
                new GradientInfo(Color.FromArgb(252, 0, 255), Color.FromArgb(0, 219, 222)),
                new GradientInfo(Color.FromArgb(229, 57, 53), Color.FromArgb(227, 93, 91)),
                new GradientInfo(Color.FromArgb(0, 92, 151), Color.FromArgb(54, 55, 149)),
                new GradientInfo(Color.FromArgb(244, 107, 69), Color.FromArgb(238, 168, 73)),
                new GradientInfo(Color.FromArgb(0, 201, 255), Color.FromArgb(146, 254, 157)),
                new GradientInfo(Color.FromArgb(103, 58, 183), Color.FromArgb(81, 45, 168)),
                new GradientInfo(Color.FromArgb(118, 184, 82), Color.FromArgb(141, 194, 111)),
                new GradientInfo(Color.FromArgb(142, 14, 0), Color.FromArgb(31, 28, 24)),
                new GradientInfo(Color.FromArgb(255, 183, 94), Color.FromArgb(237, 143, 3)),
                new GradientInfo(Color.FromArgb(194, 229, 156), Color.FromArgb(100, 179, 244)),
                new GradientInfo(Color.FromArgb(64, 58, 62), Color.FromArgb(190, 88, 105)),
                new GradientInfo(Color.FromArgb(192, 36, 37), Color.FromArgb(240, 203, 53)),
                new GradientInfo(Color.FromArgb(178, 69, 146), Color.FromArgb(241, 95, 121)),
                new GradientInfo(Color.FromArgb(69, 127, 202), Color.FromArgb(86, 145, 200)),
                new GradientInfo(Color.FromArgb(106, 48, 147), Color.FromArgb(160, 68, 255)),
                new GradientInfo(Color.FromArgb(234, 205, 163), Color.FromArgb(214, 174, 123)),
                new GradientInfo(Color.FromArgb(253, 116, 108), Color.FromArgb(255, 144, 104)),
                new GradientInfo(Color.FromArgb(17, 67, 87), Color.FromArgb(242, 148, 146)),
                new GradientInfo(Color.FromArgb(30, 60, 114), Color.FromArgb(42, 82, 152)),
                new GradientInfo(Color.FromArgb(47, 115, 54), Color.FromArgb(170, 58, 56)),
                new GradientInfo(Color.FromArgb(86, 20, 176), Color.FromArgb(219, 214, 92)),
                new GradientInfo(Color.FromArgb(77, 160, 176), Color.FromArgb(211, 157, 56)),
                new GradientInfo(Color.FromArgb(90, 63, 55), Color.FromArgb(44, 119, 68)),
                new GradientInfo(Color.FromArgb(41, 128, 185), Color.FromArgb(44, 62, 80)),
                new GradientInfo(Color.FromArgb(0, 153, 247), Color.FromArgb(241, 23, 18)),
                new GradientInfo(Color.FromArgb(131, 77, 155), Color.FromArgb(208, 78, 214)),
                new GradientInfo(Color.FromArgb(75, 121, 161), Color.FromArgb(40, 62, 81)),
                new GradientInfo(Color.FromArgb(0, 0, 0), Color.FromArgb(67, 67, 67)),
                new GradientInfo(Color.FromArgb(76, 161, 175), Color.FromArgb(196, 224, 229)),
                new GradientInfo(Color.FromArgb(224, 234, 252), Color.FromArgb(207, 222, 243)),
                new GradientInfo(Color.FromArgb(186, 83, 112), Color.FromArgb(244, 226, 216)),
                new GradientInfo(Color.FromArgb(255, 75, 31), Color.FromArgb(31, 221, 255)),
                new GradientInfo(Color.FromArgb(247, 255, 0), Color.FromArgb(219, 54, 164)),
                new GradientInfo(Color.FromArgb(168, 0, 119), Color.FromArgb(102, 255, 0)),
                new GradientInfo(Color.FromArgb(29, 67, 80), Color.FromArgb(164, 57, 49)),
                new GradientInfo(Color.FromArgb(238, 205, 163), Color.FromArgb(239, 98, 159)),
                new GradientInfo(Color.FromArgb(22, 191, 253), Color.FromArgb(203, 48, 102)),
                new GradientInfo(Color.FromArgb(255, 75, 31), Color.FromArgb(255, 144, 104)),
                new GradientInfo(Color.FromArgb(255, 95, 109), Color.FromArgb(255, 195, 113)),
                new GradientInfo(Color.FromArgb(33, 150, 243), Color.FromArgb(244, 67, 54)),
                new GradientInfo(Color.FromArgb(0, 210, 255), Color.FromArgb(146, 141, 171)),
                new GradientInfo(Color.FromArgb(58, 123, 213), Color.FromArgb(58, 96, 115)),
                new GradientInfo(Color.FromArgb(11, 72, 107), Color.FromArgb(245, 98, 23)),
                new GradientInfo(Color.FromArgb(233, 100, 67), Color.FromArgb(144, 78, 149)),
                new GradientInfo(Color.FromArgb(44, 62, 80), Color.FromArgb(76, 161, 175)),
                new GradientInfo(Color.FromArgb(44, 62, 80), Color.FromArgb(253, 116, 108)),
                new GradientInfo(Color.FromArgb(240, 0, 0), Color.FromArgb(220, 40, 30)),
                new GradientInfo(Color.FromArgb(20, 30, 48), Color.FromArgb(36, 59, 85)),
                new GradientInfo(Color.FromArgb(66, 39, 90), Color.FromArgb(115, 75, 109)),
                new GradientInfo(Color.FromArgb(0, 4, 40), Color.FromArgb(0, 78, 146)),
                new GradientInfo(Color.FromArgb(86, 171, 47), Color.FromArgb(168, 224, 99)),
                new GradientInfo(Color.FromArgb(203, 45, 62), Color.FromArgb(239, 71, 58)),
                new GradientInfo(Color.FromArgb(247, 157, 0), Color.FromArgb(100, 243, 140)),
                new GradientInfo(Color.FromArgb(248, 80, 50), Color.FromArgb(231, 56, 39)),
                new GradientInfo(Color.FromArgb(252, 234, 187), Color.FromArgb(248, 181, 0)),
                new GradientInfo(Color.FromArgb(128, 128, 128), Color.FromArgb(63, 173, 168)),
                new GradientInfo(Color.FromArgb(255, 216, 155), Color.FromArgb(25, 84, 123)),
                new GradientInfo(Color.FromArgb(189, 195, 199), Color.FromArgb(44, 62, 80)),
                new GradientInfo(Color.FromArgb(190, 147, 197), Color.FromArgb(123, 198, 204)),
                new GradientInfo(Color.FromArgb(161, 255, 206), Color.FromArgb(250, 255, 209)),
                new GradientInfo(Color.FromArgb(78, 205, 196), Color.FromArgb(85, 98, 112)),
                new GradientInfo(Color.FromArgb(58, 97, 134), Color.FromArgb(137, 37, 62)),
                new GradientInfo(Color.FromArgb(239, 50, 217), Color.FromArgb(137, 255, 253)),
                new GradientInfo(Color.FromArgb(222, 97, 97), Color.FromArgb(38, 87, 235)),
                new GradientInfo(Color.FromArgb(255, 0, 204), Color.FromArgb(51, 51, 153)),
                new GradientInfo(Color.FromArgb(255, 252, 0), Color.FromArgb(255, 255, 255)),
                new GradientInfo(Color.FromArgb(255, 126, 95), Color.FromArgb(254, 180, 123)),
                new GradientInfo(Color.FromArgb(0, 195, 255), Color.FromArgb(255, 255, 28)),
                new GradientInfo(Color.FromArgb(244, 196, 243), Color.FromArgb(252, 103, 250)),
                new GradientInfo(Color.FromArgb(65, 41, 90), Color.FromArgb(47, 7, 67)),
                new GradientInfo(Color.FromArgb(167, 112, 239), Color.FromArgb(207, 139, 243), Color.FromArgb(253, 185, 155)),
                new GradientInfo(Color.FromArgb(238, 9, 121), Color.FromArgb(255, 106, 0)),
                new GradientInfo(Color.FromArgb(243, 144, 79), Color.FromArgb(59, 67, 113)),
                new GradientInfo(Color.FromArgb(103, 178, 111), Color.FromArgb(76, 162, 205)),
                new GradientInfo(Color.FromArgb(52, 148, 230), Color.FromArgb(236, 110, 173)),
                new GradientInfo(Color.FromArgb(219, 230, 246), Color.FromArgb(197, 121, 109)),
                new GradientInfo(Color.FromArgb(156, 236, 251), Color.FromArgb(101, 199, 247), Color.FromArgb(0, 82, 212)),
                new GradientInfo(Color.FromArgb(192, 192, 170), Color.FromArgb(28, 239, 255)),
                new GradientInfo(Color.FromArgb(220, 227, 91), Color.FromArgb(69, 182, 73)),
                new GradientInfo(Color.FromArgb(232, 203, 192), Color.FromArgb(99, 111, 164)),
                new GradientInfo(Color.FromArgb(240, 242, 240), Color.FromArgb(0, 12, 64)),
                new GradientInfo(Color.FromArgb(255, 175, 189), Color.FromArgb(255, 195, 160)),
                new GradientInfo(Color.FromArgb(67, 198, 172), Color.FromArgb(248, 255, 174)),
                new GradientInfo(Color.FromArgb(9, 48, 40), Color.FromArgb(35, 122, 87)),
                new GradientInfo(Color.FromArgb(67, 198, 172), Color.FromArgb(25, 22, 84)),
                new GradientInfo(Color.FromArgb(69, 104, 220), Color.FromArgb(176, 106, 179)),
                new GradientInfo(Color.FromArgb(5, 117, 230), Color.FromArgb(2, 27, 121)),
                new GradientInfo(Color.FromArgb(32, 1, 34), Color.FromArgb(111, 0, 0)),
                new GradientInfo(Color.FromArgb(68, 160, 141), Color.FromArgb(9, 54, 55)),
                new GradientInfo(Color.FromArgb(97, 144, 232), Color.FromArgb(167, 191, 232)),
                new GradientInfo(Color.FromArgb(52, 232, 158), Color.FromArgb(15, 52, 67)),
                new GradientInfo(Color.FromArgb(247, 151, 30), Color.FromArgb(255, 210, 0)),
                new GradientInfo(Color.FromArgb(195, 55, 100), Color.FromArgb(29, 38, 113)),
                new GradientInfo(Color.FromArgb(32, 0, 44), Color.FromArgb(203, 180, 212)),
                new GradientInfo(Color.FromArgb(214, 109, 117), Color.FromArgb(226, 149, 135)),
                new GradientInfo(Color.FromArgb(48, 232, 191), Color.FromArgb(255, 130, 53)),
                new GradientInfo(Color.FromArgb(178, 254, 250), Color.FromArgb(14, 210, 247)),
                new GradientInfo(Color.FromArgb(74, 194, 154), Color.FromArgb(189, 255, 243)),
                new GradientInfo(Color.FromArgb(228, 77, 38), Color.FromArgb(241, 101, 41)),
                new GradientInfo(Color.FromArgb(235, 87, 87), Color.FromArgb(0, 0, 0)),
                new GradientInfo(Color.FromArgb(242, 153, 74), Color.FromArgb(242, 201, 76)),
                new GradientInfo(Color.FromArgb(86, 204, 242), Color.FromArgb(47, 128, 237)),
                new GradientInfo(Color.FromArgb(0, 121, 145), Color.FromArgb(120, 255, 214)),
                new GradientInfo(Color.FromArgb(0, 0, 70), Color.FromArgb(28, 181, 224)),
                new GradientInfo(Color.FromArgb(21, 153, 87), Color.FromArgb(21, 87, 153)),
                new GradientInfo(Color.FromArgb(192, 57, 43), Color.FromArgb(142, 68, 173)),
                new GradientInfo(Color.FromArgb(239, 59, 54), Color.FromArgb(255, 255, 255)),
                new GradientInfo(Color.FromArgb(40, 60, 134), Color.FromArgb(69, 162, 71)),
                new GradientInfo(Color.FromArgb(58, 28, 113), Color.FromArgb(215, 109, 119), Color.FromArgb(255, 175, 123)),
                new GradientInfo(Color.FromArgb(203, 53, 107), Color.FromArgb(189, 63, 50)),
                new GradientInfo(Color.FromArgb(54, 209, 220), Color.FromArgb(91, 134, 229)),
                new GradientInfo(Color.FromArgb(0, 0, 0), Color.FromArgb(15, 155, 15)),
                new GradientInfo(Color.FromArgb(28, 146, 210), Color.FromArgb(242, 252, 254)),
                new GradientInfo(Color.FromArgb(100, 43, 115), Color.FromArgb(198, 66, 110)),
                new GradientInfo(Color.FromArgb(6, 190, 182), Color.FromArgb(72, 177, 191)),
                new GradientInfo(Color.FromArgb(12, 235, 235), Color.FromArgb(32, 227, 178), Color.FromArgb(41, 255, 198)),
                new GradientInfo(Color.FromArgb(217, 167, 199), Color.FromArgb(255, 252, 220)),
                new GradientInfo(Color.FromArgb(57, 106, 252), Color.FromArgb(41, 72, 255)),
                new GradientInfo(Color.FromArgb(201, 214, 255), Color.FromArgb(226, 226, 226)),
                new GradientInfo(Color.FromArgb(127, 0, 255), Color.FromArgb(225, 0, 255)),
                new GradientInfo(Color.FromArgb(255, 153, 102), Color.FromArgb(255, 94, 98)),
                new GradientInfo(Color.FromArgb(34, 193, 195), Color.FromArgb(253, 187, 45)),
                new GradientInfo(Color.FromArgb(26, 42, 108), Color.FromArgb(178, 31, 31), Color.FromArgb(253, 187, 45)),
                new GradientInfo(Color.FromArgb(225, 238, 195), Color.FromArgb(240, 80, 83)),
                new GradientInfo(Color.FromArgb(173, 169, 150), Color.FromArgb(242, 242, 242), Color.FromArgb(219, 219, 219), Color.FromArgb(234, 234, 234)),
                new GradientInfo(Color.FromArgb(102, 125, 182), Color.FromArgb(0, 130, 200), Color.FromArgb(0, 130, 200), Color.FromArgb(102, 125, 182)),
                new GradientInfo(Color.FromArgb(3, 0, 30), Color.FromArgb(115, 3, 192), Color.FromArgb(236, 56, 188), Color.FromArgb(253, 239, 249)),
                new GradientInfo(Color.FromArgb(109, 96, 39), Color.FromArgb(211, 203, 184)),
                new GradientInfo(Color.FromArgb(116, 235, 213), Color.FromArgb(172, 182, 229)),
                new GradientInfo(Color.FromArgb(252, 74, 26), Color.FromArgb(247, 183, 51)),
                new GradientInfo(Color.FromArgb(0, 242, 96), Color.FromArgb(5, 117, 230)),
                new GradientInfo(Color.FromArgb(128, 0, 128), Color.FromArgb(255, 192, 203)),
                new GradientInfo(Color.FromArgb(202, 197, 49), Color.FromArgb(243, 249, 167)),
                new GradientInfo(Color.FromArgb(60, 59, 63), Color.FromArgb(96, 92, 60)),
                new GradientInfo(Color.FromArgb(211, 204, 227), Color.FromArgb(233, 228, 240)),
                new GradientInfo(Color.FromArgb(0, 176, 155), Color.FromArgb(150, 201, 61)),
                new GradientInfo(Color.FromArgb(15, 12, 41), Color.FromArgb(48, 43, 99), Color.FromArgb(36, 36, 62)),
                new GradientInfo(Color.FromArgb(255, 251, 213), Color.FromArgb(178, 10, 44)),
                new GradientInfo(Color.FromArgb(35, 7, 77), Color.FromArgb(204, 83, 51)),
                new GradientInfo(Color.FromArgb(201, 75, 75), Color.FromArgb(75, 19, 79)),
                new GradientInfo(Color.FromArgb(252, 70, 107), Color.FromArgb(63, 94, 251)),
                new GradientInfo(Color.FromArgb(252, 92, 125), Color.FromArgb(106, 130, 251)),
                new GradientInfo(Color.FromArgb(16, 141, 199), Color.FromArgb(239, 142, 56)),
                new GradientInfo(Color.FromArgb(17, 153, 142), Color.FromArgb(56, 239, 125)),
                new GradientInfo(Color.FromArgb(62, 81, 81), Color.FromArgb(222, 203, 164)),
                new GradientInfo(Color.FromArgb(64, 224, 208), Color.FromArgb(255, 140, 0), Color.FromArgb(255, 0, 128)),
                new GradientInfo(Color.FromArgb(188, 78, 156), Color.FromArgb(248, 7, 89)),
                new GradientInfo(Color.FromArgb(53, 92, 125), Color.FromArgb(108, 91, 123), Color.FromArgb(192, 108, 132)),
                new GradientInfo(Color.FromArgb(78, 84, 200), Color.FromArgb(143, 148, 251)),
                new GradientInfo(Color.FromArgb(51, 51, 51), Color.FromArgb(221, 24, 24)),
                new GradientInfo(Color.FromArgb(168, 192, 255), Color.FromArgb(63, 43, 150)),
                new GradientInfo(Color.FromArgb(173, 83, 137), Color.FromArgb(60, 16, 83)),
                new GradientInfo(Color.FromArgb(99, 99, 99), Color.FromArgb(162, 171, 88)),
                new GradientInfo(Color.FromArgb(218, 68, 83), Color.FromArgb(137, 33, 107)),
                new GradientInfo(Color.FromArgb(0, 90, 167), Color.FromArgb(255, 253, 228)),
                new GradientInfo(Color.FromArgb(89, 193, 115), Color.FromArgb(161, 127, 224), Color.FromArgb(93, 38, 193)),
                new GradientInfo(Color.FromArgb(255, 239, 186), Color.FromArgb(255, 255, 255)),
                new GradientInfo(Color.FromArgb(0, 180, 219), Color.FromArgb(0, 131, 176)),
                new GradientInfo(Color.FromArgb(253, 200, 48), Color.FromArgb(243, 115, 53)),
                new GradientInfo(Color.FromArgb(237, 33, 58), Color.FromArgb(147, 41, 30)),
                new GradientInfo(Color.FromArgb(30, 150, 0), Color.FromArgb(255, 242, 0), Color.FromArgb(255, 0, 0)),
                new GradientInfo(Color.FromArgb(168, 255, 120), Color.FromArgb(120, 255, 214)),
                new GradientInfo(Color.FromArgb(138, 35, 135), Color.FromArgb(233, 64, 87), Color.FromArgb(242, 113, 33)),
                new GradientInfo(Color.FromArgb(255, 65, 108), Color.FromArgb(255, 75, 43)),
                new GradientInfo(Color.FromArgb(101, 78, 163), Color.FromArgb(234, 175, 200)),
                new GradientInfo(Color.FromArgb(0, 159, 255), Color.FromArgb(236, 47, 75)),
                new GradientInfo(Color.FromArgb(84, 74, 125), Color.FromArgb(255, 212, 82)),
                new GradientInfo(Color.FromArgb(131, 96, 195), Color.FromArgb(46, 191, 145)),
                new GradientInfo(Color.FromArgb(221, 62, 84), Color.FromArgb(107, 229, 133)),
                new GradientInfo(Color.FromArgb(101, 153, 153), Color.FromArgb(244, 121, 31)),
                new GradientInfo(Color.FromArgb(241, 39, 17), Color.FromArgb(245, 175, 25)),
                new GradientInfo(Color.FromArgb(195, 20, 50), Color.FromArgb(36, 11, 54)),
                new GradientInfo(Color.FromArgb(127, 127, 213), Color.FromArgb(134, 168, 231), Color.FromArgb(145, 234, 228)),
                new GradientInfo(Color.FromArgb(249, 83, 198), Color.FromArgb(185, 29, 115)),
                new GradientInfo(Color.FromArgb(31, 64, 55), Color.FromArgb(153, 242, 200)),
                new GradientInfo(Color.FromArgb(142, 45, 226), Color.FromArgb(74, 0, 224)),
                new GradientInfo(Color.FromArgb(170, 75, 107), Color.FromArgb(107, 107, 131), Color.FromArgb(59, 141, 153)),
                new GradientInfo(Color.FromArgb(255, 0, 153), Color.FromArgb(73, 50, 64)),
                new GradientInfo(Color.FromArgb(41, 128, 185), Color.FromArgb(109, 213, 250), Color.FromArgb(255, 255, 255)),
                new GradientInfo(Color.FromArgb(55, 59, 68), Color.FromArgb(66, 134, 244)),
                new GradientInfo(Color.FromArgb(185, 43, 39), Color.FromArgb(21, 101, 192)),
                new GradientInfo(Color.FromArgb(18, 194, 233), Color.FromArgb(196, 113, 237), Color.FromArgb(246, 79, 89)),
                new GradientInfo(Color.FromArgb(15, 32, 39), Color.FromArgb(32, 58, 67), Color.FromArgb(44, 83, 100)),
                new GradientInfo(Color.FromArgb(198, 255, 221), Color.FromArgb(251, 215, 134), Color.FromArgb(247, 121, 125)),
                new GradientInfo(Color.FromArgb(33, 147, 176), Color.FromArgb(109, 213, 237)),
                new GradientInfo(Color.FromArgb(238, 156, 167), Color.FromArgb(255, 221, 225)),
                new GradientInfo(Color.FromArgb(189, 195, 199), Color.FromArgb(44, 62, 80)),
                new GradientInfo(Color.FromArgb(243, 98, 34), Color.FromArgb(92, 182, 68), Color.FromArgb(0, 127, 195)),
                new GradientInfo(Color.FromArgb(42, 45, 62), Color.FromArgb(254, 203, 110)),
                new GradientInfo(Color.FromArgb(138, 43, 226), Color.FromArgb(0, 0, 205), Color.FromArgb(34, 139, 34), Color.FromArgb(204, 255, 0)),
                new GradientInfo(Color.FromArgb(5, 25, 55), Color.FromArgb(0, 77, 122), Color.FromArgb(0, 135, 147), Color.FromArgb(0, 191, 114), Color.FromArgb(168, 235, 18)),
                new GradientInfo(Color.FromArgb(96, 37, 245), Color.FromArgb(255, 85, 85)),
                new GradientInfo(Color.FromArgb(138, 43, 226), Color.FromArgb(255, 165, 0), Color.FromArgb(248, 248, 255)),
                new GradientInfo(Color.FromArgb(39, 116, 174), Color.FromArgb(0, 46, 93), Color.FromArgb(0, 46, 93)),
                new GradientInfo(Color.FromArgb(0, 70, 128), Color.FromArgb(68, 132, 186)),
                new GradientInfo(Color.FromArgb(126, 198, 188), Color.FromArgb(235, 231, 23)),
                new GradientInfo(Color.FromArgb(255, 30, 86), Color.FromArgb(249, 201, 66), Color.FromArgb(30, 144, 255)),
                new GradientInfo(Color.FromArgb(222, 138, 65), Color.FromArgb(42, 218, 83)),
                new GradientInfo(Color.FromArgb(247, 240, 172), Color.FromArgb(172, 247, 240), Color.FromArgb(240, 172, 247)),
                new GradientInfo(Color.FromArgb(255, 0, 0), Color.FromArgb(253, 207, 88)),
                new GradientInfo(Color.FromArgb(54, 177, 199), Color.FromArgb(150, 11, 51)),
                new GradientInfo(Color.FromArgb(29, 161, 242), Color.FromArgb(0, 159, 252)),
                new GradientInfo(Color.FromArgb(109, 166, 190), Color.FromArgb(75, 133, 158), Color.FromArgb(109, 166, 190)),
                new GradientInfo(Color.FromArgb(181, 185, 255), Color.FromArgb(43, 44, 73)),
                new GradientInfo(Color.FromArgb(159, 160, 168), Color.FromArgb(92, 120, 82)),
                new GradientInfo(Color.FromArgb(220, 255, 189), Color.FromArgb(204, 134, 209)),
                new GradientInfo(Color.FromArgb(139, 222, 218), Color.FromArgb(67, 173, 208), Color.FromArgb(153, 142, 224), Color.FromArgb(225, 125, 194), Color.FromArgb(239, 147, 147)),
                new GradientInfo(Color.FromArgb(230, 174, 140), Color.FromArgb(168, 206, 207)),
                new GradientInfo(Color.FromArgb(0, 255, 240), Color.FromArgb(0, 131, 254)),
                new GradientInfo(Color.FromArgb(51, 51, 51), Color.FromArgb(162, 171, 88), Color.FromArgb(164, 57, 49)),
                new GradientInfo(Color.FromArgb(12, 12, 109), Color.FromArgb(222, 81, 43), Color.FromArgb(152, 208, 193), Color.FromArgb(91, 178, 38), Color.FromArgb(2, 60, 13)),
                new GradientInfo(Color.FromArgb(5, 56, 107), Color.FromArgb(92, 219, 149)),
                new GradientInfo(Color.FromArgb(66, 132, 219), Color.FromArgb(41, 234, 196)),
                new GradientInfo(Color.FromArgb(85, 64, 35), Color.FromArgb(201, 152, 70)),
                new GradientInfo(Color.FromArgb(81, 107, 139), Color.FromArgb(5, 107, 59)),
                new GradientInfo(Color.FromArgb(215, 6, 82), Color.FromArgb(255, 2, 94)),
                new GradientInfo(Color.FromArgb(21, 35, 49), Color.FromArgb(0, 0, 0)),
                new GradientInfo(Color.FromArgb(247, 247, 247), Color.FromArgb(185, 160, 160), Color.FromArgb(121, 71, 71), Color.FromArgb(78, 32, 32), Color.FromArgb(17, 17, 17)),
                new GradientInfo(Color.FromArgb(89, 205, 233), Color.FromArgb(10, 42, 136)),
                new GradientInfo(Color.FromArgb(235, 0, 0), Color.FromArgb(149, 0, 138), Color.FromArgb(51, 0, 252)),
                new GradientInfo(Color.FromArgb(255, 117, 195), Color.FromArgb(255, 166, 71), Color.FromArgb(255, 232, 63), Color.FromArgb(159, 255, 91), Color.FromArgb(112, 226, 255), Color.FromArgb(205, 147, 255)),
                new GradientInfo(Color.FromArgb(129, 255, 138), Color.FromArgb(100, 150, 94)),
                new GradientInfo(Color.FromArgb(212, 252, 121), Color.FromArgb(150, 230, 161)),
                new GradientInfo(Color.FromArgb(0, 61, 77), Color.FromArgb(0, 201, 150)),

                new GradientInfo(Color.FromArgb(255, 0, 0), Color.FromArgb(255, 0, 255), Color.FromArgb(0, 0, 255), Color.FromArgb(0, 255, 255), Color.FromArgb(0, 255, 0), Color.FromArgb(255, 255, 0), Color.FromArgb(255, 0, 0))
            };

            lvPresets.BeginUpdate();

            lvPresets.Items.Clear();
            ilPresets.Images.Clear();

            Bitmap[] gradientBitmaps = new Bitmap[gradients.Length];

            for (int i = 0; i < gradients.Length; i++)
            {
                GradientInfo gradient = gradients[i];
                gradient.Type = Gradient.Type;
                gradientBitmaps[i] = gradient.CreateGradientPreview(64, 64, true);
            }

            ilPresets.Images.AddRange(gradientBitmaps);

            for (int i = 0; i < gradients.Length; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.ImageIndex = i;
                lvi.Tag = gradients[i];
                lvPresets.Items.Add(lvi);
            }

            lvPresets.EndUpdate();
        }

        private void UpdateGradientList(bool selectFirst = false)
        {
            isReady = false;
            Gradient.Sort();

            lvGradientPoints.Items.Clear();
            foreach (GradientStop gradientStop in Gradient.Colors)
            {
                AddGradientStop(gradientStop);
            }

            if (selectFirst && lvGradientPoints.Items.Count > 0)
            {
                lvGradientPoints.SelectedIndex = 0;
            }

            isReady = true;
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (isReady)
            {
                Bitmap bmp = Gradient.CreateGradientPreview(pbPreview.ClientRectangle.Width, pbPreview.ClientRectangle.Height, true);
                pbPreview.Image?.Dispose();
                pbPreview.Image = bmp;

                OnGradientChanged();
            }
        }

        private void AddGradientStop(GradientStop gradientStop)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = string.Format(" {0:0.##}%", gradientStop.Location);
            UpdateListViewItemColor(lvi, gradientStop.Color);
            lvi.Tag = gradientStop;
            lvGradientPoints.Items.Add(lvi);
        }

        private void UpdateListViewItemColor(ListViewItem lvi, Color color)
        {
            string argb = color.ToArgb().ToString();

            if (!ilColors.Images.ContainsKey(argb))
            {
                ilColors.Images.Add(argb, ImageHelpers.CreateColorPickerIcon(color, new Rectangle(0, 0, 16, 16)));
            }

            lvi.ImageKey = argb;
        }

        private GradientStop GetSelectedGradientStop()
        {
            return GetSelectedGradientStop(out _);
        }

        private GradientStop GetSelectedGradientStop(out ListViewItem lvi)
        {
            if (lvGradientPoints.SelectedItems.Count > 0)
            {
                lvi = lvGradientPoints.SelectedItems[0];
                return lvi.Tag as GradientStop;
            }

            lvi = null;
            return null;
        }

        private ListViewItem FindListViewItem(GradientStop gradientStop)
        {
            foreach (ListViewItem lvi in lvGradientPoints.Items)
            {
                GradientStop itemGradientstop = lvi.Tag as GradientStop;
                if (itemGradientstop == gradientStop)
                {
                    return lvi;
                }
            }

            return null;
        }

        private ListViewItem SelectGradientStop(GradientStop gradientStop)
        {
            ListViewItem lvi = FindListViewItem(gradientStop);
            if (lvi != null)
            {
                lvi.Selected = true;
            }
            return lvi;
        }

        private void GradientPickerForm_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();

            AddPresets();
        }

        private void cbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isReady)
            {
                Gradient.Type = (LinearGradientMode)cbGradientType.SelectedIndex;
                UpdatePreview();
                AddPresets();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Color color = cbtnCurrentColor.Color;
            float offset = (float)nudLocation.Value;
            GradientStop gradientStop = new GradientStop(color, offset);
            Gradient.Colors.Add(gradientStop);
            UpdateGradientList();
            SelectGradientStop(gradientStop);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            GradientStop gradientStop = GetSelectedGradientStop();

            if (gradientStop != null)
            {
                Gradient.Colors.Remove(gradientStop);
                UpdateGradientList();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Gradient.Clear();
            UpdateGradientList();
        }

        private void btnReverse_Click(object sender, EventArgs e)
        {
            if (Gradient.IsValid)
            {
                Gradient.Reverse();
                UpdateGradientList();
            }
        }

        private void cbtnCurrentColor_ColorChanged(Color color)
        {
            GradientStop gradientStop = GetSelectedGradientStop(out ListViewItem lvi);

            if (gradientStop != null)
            {
                gradientStop.Color = color;
                UpdateListViewItemColor(lvi, gradientStop.Color);
                UpdatePreview();
            }
        }

        private void nudLocation_ValueChanged(object sender, EventArgs e)
        {
            if (isReady)
            {
                GradientStop gradientStop = GetSelectedGradientStop();

                if (gradientStop != null)
                {
                    gradientStop.Location = (float)nudLocation.Value;
                    UpdateGradientList();
                    SelectGradientStop(gradientStop);
                }
            }
        }

        private void lvGradientPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            GradientStop gradientStop = GetSelectedGradientStop();

            if (gradientStop != null)
            {
                isReady = false;
                cbtnCurrentColor.Color = gradientStop.Color;
                nudLocation.SetValue((decimal)gradientStop.Location);
                isReady = true;
            }
        }

        private void lvGradientPoints_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lvGradientPoints.SelectedItems.Count > 0)
            {
                cbtnCurrentColor.ShowColorDialog();
            }
        }

        private void lvPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isReady && lvPresets.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvPresets.SelectedItems[0];
                if (lvi.Tag is GradientInfo gradientInfo)
                {
                    Gradient = gradientInfo.Copy();
                    UpdateGradientList(true);
                    lvi.Selected = false;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}