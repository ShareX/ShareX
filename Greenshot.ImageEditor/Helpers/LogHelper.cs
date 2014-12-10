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
using System.IO;
using System.Windows.Forms;

namespace Greenshot.Helpers {
	public class SpecialFolderPatternConverter : log4net.Util.PatternConverter {
		protected override void Convert(System.IO.TextWriter writer, object state) {
			// Making Greenshot portable
			string pafPath = Path.Combine(Application.StartupPath, @"App\Greenshot");
			if (Directory.Exists(pafPath)) {
				writer.Write(Path.Combine(Application.StartupPath, @"Data"));
				return;
			}
			Environment.SpecialFolder specialFolder = (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), base.Option, true);
			writer.Write(Environment.GetFolderPath(specialFolder));
		}
	}
}
