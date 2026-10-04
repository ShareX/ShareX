from http.server import ThreadingHTTPServer,BaseHTTPRequestHandler
from pathlib import Path
from PIL import Image
import json,hashlib,io,signal,threading
root=Path('/home/jaex/.codex/sharex-agent-j/kde-20261004/flows/clipboard-upload');expected_text=b'ShareX synthetic clipboard text fixture 2026-10-04\n';expected=Image.open('/home/jaex/.codex/sharex-agent-j/kde-20261004/flows/synthetic.png').convert('RGBA');requests=[]
class Handler(BaseHTTPRequestHandler):
 def log_message(self,*args):pass
 def do_POST(self):
  size=int(self.headers.get('Content-Length','0'));body=self.rfile.read(size);kind='unknown'
  if body==expected_text:kind='text'
  else:
   try:
    im=Image.open(io.BytesIO(body)).convert('RGBA')
    if im.size==expected.size and im.tobytes()==expected.tobytes():kind='image'
   except Exception:pass
  n=len(requests)+1;record={'index':n,'path':self.path,'kind':kind,'bytes':len(body),'sha256':hashlib.sha256(body).hexdigest(),'matches_expected':kind!='unknown'}
  if kind=='unknown':self.send_response(400);self.end_headers();requests.append(record);return
  filename=f'received-{n}.'+('png' if kind=='image' else 'txt');(root/filename).write_bytes(body);record['file']=filename;requests.append(record);(root/'requests.json').write_text(json.dumps(requests,indent=2)+'\n')
  response=json.dumps({'url':f'http://127.0.0.1:{server.server_port}/result/{n}'}).encode();self.send_response(200);self.send_header('Content-Type','application/json');self.send_header('Content-Length',str(len(response)));self.end_headers();self.wfile.write(response)
 def do_GET(self):
  self.send_response(200);self.end_headers();self.wfile.write(b'ShareX synthetic local result')
server=ThreadingHTTPServer(('127.0.0.1',0),Handler);endpoint=f'http://127.0.0.1:{server.server_port}/upload';(root/'endpoint.json').write_text(json.dumps({'endpoint':endpoint,'synthetic_only':True},indent=2)+'\n');print(endpoint,flush=True)
signal.signal(signal.SIGTERM,lambda *args:threading.Thread(target=server.shutdown).start());server.serve_forever();server.server_close()
