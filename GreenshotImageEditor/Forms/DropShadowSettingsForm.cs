/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
 * 
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using Greenshot.Core;
using GreenshotPlugin.Core;

namespace Greenshot.Forms
{
    public partial class DropShadowSettingsForm : BaseForm
    {
        private DropShadowEffect effect;
        public DropShadowSettingsForm(DropShadowEffect effect)
        {
            this.effect = effect;
            InitializeComponent();
            this.Icon = GreenshotResources.getGreenshotIcon();
            trackBar1.Value = (int)(effect.Darkness * 40);
            offsetX.Value = effect.ShadowOffset.X;
            offsetY.Value = effect.ShadowOffset.Y;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            effect.Darkness = (float)trackBar1.Value / (float)40;
            effect.ShadowOffset = new Point((int)offsetX.Value, (int)offsetY.Value);
            effect.ShadowSize = (int)thickness.Value;
            DialogResult = DialogResult.OK;
        }
    }
}
