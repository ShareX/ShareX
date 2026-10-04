import gi,sys,json,signal
from pathlib import Path
gi.require_version('Gtk','3.0');from gi.repository import Gtk,Gdk,GdkPixbuf,GLib
root=Path('/home/jaex/.codex/sharex-agent-j/kde-20261004/flows');mode=sys.argv[1];w=Gtk.Window(title='ShareX synthetic clipboard publisher');w.set_default_size(360,160);w.add(Gtk.Label(label='Synthetic '+mode+' for the local ShareX upload'));w.connect('destroy',Gtk.main_quit);w.show_all();w.present();cb=Gtk.Clipboard.get(Gdk.SELECTION_CLIPBOARD)
def publish():
 if mode=='text':cb.set_text('ShareX synthetic clipboard text fixture 2026-10-04\n',-1)
 else:cb.set_image(GdkPixbuf.Pixbuf.new_from_file(str(root/'synthetic.png')))
 return False
def receipt():
 result={'display':w.get_display().get_name(),'mapped':w.get_mapped(),'active':w.is_active(),'mode':mode}
 if mode=='text':result['readback_matches_synthetic']=cb.wait_for_text()=='ShareX synthetic clipboard text fixture 2026-10-04\n'
 else:
  pix=cb.wait_for_image();result['readback_dimensions']=[pix.get_width(),pix.get_height()] if pix else None
 (root/'clipboard-upload'/('publisher-'+mode+'.json')).write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result),flush=True);return False
GLib.timeout_add(500,publish);GLib.timeout_add(1500,receipt);GLib.unix_signal_add(GLib.PRIORITY_DEFAULT,signal.SIGTERM,lambda:(w.destroy(),False)[1]);GLib.timeout_add_seconds(180,lambda:(w.destroy(),False)[1]);Gtk.main()
