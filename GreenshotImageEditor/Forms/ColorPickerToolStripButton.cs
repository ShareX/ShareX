using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Diagnostics;

namespace GreenShot
{
	public delegate void ColorPickerEventHandler(object o, ColorPickerEventArgs e);
	
	public class ColorPickerToolStripButton : System.Windows.Forms.ToolStripButton
	{
		private Color color;
		public Point Offset = new Point(0,0);
		public event ColorPickerEventHandler ColorPicked;
		private ColorDialog cd;
		
		
		public ColorPickerToolStripButton()
		{
			cd = ColorDialog.GetInstance();
			this.Click += new System.EventHandler(this.ToolStripButton1Click);
		}
		
		public Color Color {
			set {color = value;this.Invalidate();}
			get {return color;}
		}
		
		protected override void OnPaint (PaintEventArgs e) {
			base.OnPaint(e);
			if(color != null) {
				// replace transparent color with selected color
				Graphics g = e.Graphics;
				//Graphics g = Graphics.FromImage(Image);
				ColorMap[] colorMap = new ColorMap[1];
			    colorMap[0] = new ColorMap();
			    colorMap[0].OldColor = Color.Magenta;//this.ImageTransparentColor;
			    colorMap[0].NewColor = color;
			    ImageAttributes attr = new ImageAttributes();
			    attr.SetRemapTable(colorMap);
			    Rectangle rect = new Rectangle(0, 0, Image.Width, Image.Height);
			  	// todo find a way to retrieve transparency offset automatically
			  	// for now, we use the public variable Offset to define this manually
			  	rect.Offset(Offset.X,Offset.Y);
			  	//Image.
			  	Debug.WriteLine("paint!"+this.Text+": "+color);
			  	//ssif(color.Equals(Color.Transparent)) ((Bitmap)Image).MakeTransparent(Color.Magenta);
			    g.DrawImage(Image, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
			    //this.Image.In
			    
			}
		}
		
		void ToolStripButton1Click(object sender, System.EventArgs e)
		{
			cd.ShowDialog(this.Owner);
			Color = cd.Color;
			if(ColorPicked != null) {
				ColorPicked(this, new ColorPickerEventArgs(Color, cd.RecentColors));
			}
		}
	}
	
	public class ColorPickerEventArgs : System.EventArgs {
		public Color Color;
		public Color[] RecentColors;
		public ColorPickerEventArgs(Color color, Color[] recentColors) {
			Color = color;
			RecentColors = recentColors;
		}
		
	}
	
	
	
	
}
