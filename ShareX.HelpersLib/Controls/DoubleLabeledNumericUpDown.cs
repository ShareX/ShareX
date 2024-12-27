﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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

using ShareX.HelpersLib.Extensions;

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ShareX.HelpersLib;

public partial class DoubleLabeledNumericUpDown : UserControl
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new string Text
    {
        get => lblText.Text;
        set => lblText.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Text2
    {
        get
        {
            return lblText2.Text;
        }
        set
        {
            lblText2.Text = value;
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Value
    {
        get
        {
            return nudValue.Value;
        }
        set
        {
            nudValue.SetValue(value);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Value2
    {
        get
        {
            return nudValue2.Value;
        }
        set
        {
            nudValue2.SetValue(value);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Maximum
    {
        get
        {
            return nudValue.Maximum;
        }
        set
        {
            nudValue.Maximum = nudValue2.Maximum = value;
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Minimum
    {
        get
        {
            return nudValue.Minimum;
        }
        set
        {
            nudValue.Minimum = nudValue2.Minimum = value;
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Increment
    {
        get
        {
            return nudValue.Increment;
        }
        set
        {
            nudValue.Increment = nudValue2.Increment = value;
        }
    }

    public EventHandler ValueChanged;

    public DoubleLabeledNumericUpDown()
    {
        InitializeComponent();
        nudValue.ValueChanged += OnValueChanged;
        nudValue2.ValueChanged += OnValueChanged;
    }

    private void OnValueChanged(object sender, EventArgs e)
    {
        ValueChanged?.Invoke(sender, e);
    }
}