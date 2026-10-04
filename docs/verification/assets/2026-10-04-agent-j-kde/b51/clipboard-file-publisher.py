import ctypes as c,ctypes.util,json,signal,time
from pathlib import Path
r=Path('/home/jaex/.codex/sharex-agent-j/kde-20261004/flows/clipboard-upload');f=r/'clipboard synthetic file.txt';f.write_text('ShareX synthetic clipboard text fixture 2026-10-04\n')
x=c.CDLL(ctypes.util.find_library('X11'))
x.XOpenDisplay.argtypes=[c.c_char_p];x.XOpenDisplay.restype=c.c_void_p
x.XDefaultRootWindow.argtypes=[c.c_void_p];x.XDefaultRootWindow.restype=c.c_ulong
x.XCreateSimpleWindow.argtypes=[c.c_void_p,c.c_ulong,c.c_int,c.c_int,c.c_uint,c.c_uint,c.c_uint,c.c_ulong,c.c_ulong];x.XCreateSimpleWindow.restype=c.c_ulong
x.XInternAtom.argtypes=[c.c_void_p,c.c_char_p,c.c_int];x.XInternAtom.restype=c.c_ulong
x.XSetSelectionOwner.argtypes=[c.c_void_p,c.c_ulong,c.c_ulong,c.c_ulong]
x.XGetSelectionOwner.argtypes=[c.c_void_p,c.c_ulong];x.XGetSelectionOwner.restype=c.c_ulong
x.XChangeProperty.argtypes=[c.c_void_p,c.c_ulong,c.c_ulong,c.c_ulong,c.c_int,c.c_int,c.c_void_p,c.c_int]
x.XSync.argtypes=[c.c_void_p,c.c_int];x.XFlush.argtypes=[c.c_void_p]
class Request(c.Structure):_fields_=[('type',c.c_int),('serial',c.c_ulong),('send',c.c_int),('display',c.c_void_p),('owner',c.c_ulong),('requestor',c.c_ulong),('selection',c.c_ulong),('target',c.c_ulong),('property',c.c_ulong),('time',c.c_ulong)]
class Notify(c.Structure):_fields_=[('type',c.c_int),('serial',c.c_ulong),('send',c.c_int),('display',c.c_void_p),('requestor',c.c_ulong),('selection',c.c_ulong),('target',c.c_ulong),('property',c.c_ulong),('time',c.c_ulong)]
class Event(c.Union):_fields_=[('req',Request),('notify',Notify),('pad',c.c_long*24)]
x.XNextEvent.argtypes=[c.c_void_p,c.POINTER(Event)];x.XSendEvent.argtypes=[c.c_void_p,c.c_ulong,c.c_int,c.c_long,c.POINTER(Event)];d=x.XOpenDisplay(None);root=x.XDefaultRootWindow(d);w=x.XCreateSimpleWindow(d,root,-100,-100,1,1,0,0,0)
def atom(n):return x.XInternAtom(d,n.encode(),0)
clip=atom('CLIPBOARD');targets=atom('TARGETS');uri=atom('text/uri-list');atype=atom('ATOM');x.XSetSelectionOwner(d,clip,w,0);x.XSync(d,0);assert x.XGetSelectionOwner(d,clip)==w
print(json.dumps({'owner':hex(w),'format':'text/uri-list','synthetic_file':f.name}),flush=True)
while True:
 e=Event();x.XNextEvent(d,c.byref(e))
 if e.req.type!=30:continue
 q=e.req;p=q.property or q.target;prop=0
 if q.target==targets:
  a=(c.c_ulong*2)(targets,uri);x.XChangeProperty(d,q.requestor,p,atype,32,0,c.cast(a,c.c_void_p),2);prop=p
 elif q.target==uri:
  b=(f.as_uri()+'\r\n').encode();a=c.create_string_buffer(b);x.XChangeProperty(d,q.requestor,p,uri,8,0,c.cast(a,c.c_void_p),len(b));prop=p
 n=Event();n.notify=Notify(31,0,1,d,q.requestor,q.selection,q.target,prop,q.time);x.XSendEvent(d,q.requestor,0,0,c.byref(n));x.XFlush(d)
