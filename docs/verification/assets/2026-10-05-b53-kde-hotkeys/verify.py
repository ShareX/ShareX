import os, subprocess, pathlib, time, signal, json
root = pathlib.Path('/tmp/sharex-b53-real-portal')
env = os.environ.copy()
for key, folder in [('XDG_DATA_HOME','data'),('XDG_CONFIG_HOME','config'),('XDG_CACHE_HOME','cache')]:
    env[key] = str(root/folder)
    (root/folder).mkdir(exist_ok=True)
env['XDG_DATA_DIRS'] = '/usr/local/share:/usr/share'
daemon = subprocess.Popen(['/usr/bin/dbus-daemon','--session','--nofork','--print-address=1'], stdout=subprocess.PIPE, stderr=open(root/'bus.log','w'), text=True, env=env, start_new_session=True)
portal = None
receipt = {}
try:
    env['DBUS_SESSION_BUS_ADDRESS'] = daemon.stdout.readline().strip()
    portal = subprocess.Popen(['/usr/libexec/xdg-desktop-portal','--verbose'], stdout=open(root/'portal.log','w'), stderr=subprocess.STDOUT, env=env, start_new_session=True)
    prefix = ['gdbus','call','--session','--dest','org.freedesktop.portal.Desktop','--object-path','/org/freedesktop/portal/desktop']
    for _ in range(40):
        state = subprocess.run(prefix + ['--method','org.freedesktop.DBus.Properties.Get','org.freedesktop.host.portal.Registry','version'], capture_output=True, text=True, env=env, timeout=3)
        if state.returncode == 0: break
        time.sleep(.1)
    receipt['registry_version'] = state.stdout.strip()
    baseline = subprocess.run(prefix + ['--method','org.freedesktop.host.portal.Registry.Register','sharex','{}'], capture_output=True, text=True, env=env, timeout=5)
    receipt['baseline_exit'] = baseline.returncode
    receipt['baseline_error'] = baseline.stderr.strip()
    run = subprocess.run([str(root/'bin/x64/Release/net10.0/ShareX')], capture_output=True, text=True, env=env, timeout=15)
    receipt['patched_native_exit'] = run.returncode
    receipt['patched_native_output'] = run.stdout.strip()
    receipt['patched_native_error'] = run.stderr.strip()
    entry = root/'data/applications/sharex.desktop'
    receipt['desktop_entry'] = entry.read_text() if entry.exists() else '<missing>'
    if entry.exists():
        validate = subprocess.run(['desktop-file-validate',str(entry)], capture_output=True,text=True,timeout=5)
        receipt['desktop_validate_exit'] = validate.returncode
        receipt['desktop_validate_output'] = validate.stdout + validate.stderr
    if entry.exists():
        original = entry.read_text()
        entry.write_text(original.replace(str(root/'bin/x64/Release/net10.0/ShareX'), str(root/'missing-ShareX')))
        stale = subprocess.run(prefix + ['--method','org.freedesktop.host.portal.Registry.Register','sharex','{}'], capture_output=True, text=True, env=env, timeout=5)
        receipt['stale_exec_registration_exit'] = stale.returncode
        receipt['stale_exec_registration_error'] = stale.stderr.strip()
        recovered = subprocess.run([str(root/'bin/x64/Release/net10.0/ShareX')], capture_output=True, text=True, env=env, timeout=15)
        receipt['stale_identity_recovery_exit'] = recovered.returncode
        receipt['stale_identity_recovery_output'] = recovered.stdout.strip()
        receipt['stale_identity_recovery_error'] = recovered.stderr.strip()
    managed = subprocess.run(['dotnet',str(root/'bin/x64/Release/net10.0/ShareX.dll')], capture_output=True, text=True, env=env, timeout=15)
    receipt['patched_framework_exit'] = managed.returncode
    receipt['patched_framework_output'] = managed.stdout.strip()
    receipt['patched_framework_error'] = managed.stderr.strip()
    receipt['framework_desktop_entry'] = entry.read_text() if entry.exists() else '<missing>'
    (root/'receipt.json').write_text(json.dumps(receipt,indent=2)+'\n')
    print(json.dumps(receipt,indent=2))
finally:
    for proc in [portal, daemon]:
        if proc is not None:
            try: os.killpg(proc.pid, signal.SIGTERM)
            except ProcessLookupError: pass
            try: proc.wait(timeout=5)
            except subprocess.TimeoutExpired:
                os.killpg(proc.pid, signal.SIGKILL)
                proc.wait(timeout=5)
