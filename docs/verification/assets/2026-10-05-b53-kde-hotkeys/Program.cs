using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShareX.Platform.Linux;
using Tmds.DBus.Protocol;

var type = typeof(LinuxPlatformServices).Assembly.GetType("ShareX.Platform.Linux.DBus.DBusSession")!;
var task = (Task<DBusConnection>)type.GetMethod("GetConnectionAsync")!.Invoke(null, new object[] { default(System.Threading.CancellationToken) })!;
using DBusConnection bus = await task.WaitAsync(TimeSpan.FromSeconds(5));
Console.WriteLine("RegistrationError=" + (type.GetProperty("RegistrationError")!.GetValue(null) ?? "<none>"));
if (type.GetProperty("RegistrationError")!.GetValue(null) is string) return 1;
var version = (Task<uint?>)type.GetMethod("GetPortalVersionAsync")!.Invoke(null, new object[] { "org.freedesktop.portal.GlobalShortcuts", default(System.Threading.CancellationToken) })!;
Console.WriteLine("GlobalShortcutsVersion=" + await version.WaitAsync(TimeSpan.FromSeconds(5)));
string sessionToken = "sharexb53session";
Func<DBusConnection, string, MessageBuffer> create = (connection, token) =>
{
    using MessageWriter writer = connection.GetMessageWriter();
    writer.WriteMethodCallHeader(destination: "org.freedesktop.portal.Desktop", path: "/org/freedesktop/portal/desktop",
        @interface: "org.freedesktop.portal.GlobalShortcuts", member: "CreateSession", signature: "a{sv}");
    writer.WriteDictionary(new Dictionary<string, VariantValue> { ["handle_token"] = token, ["session_handle_token"] = sessionToken });
    return writer.CreateMessage();
};
Task request = (Task)type.GetMethod("CallPortalRequestAsync")!.Invoke(null, new object[] { create, default(System.Threading.CancellationToken) })!;
await request.WaitAsync(TimeSpan.FromSeconds(10));
object response = request.GetType().GetProperty("Result")!.GetValue(request)!;
Console.WriteLine("CreateSessionResponse=" + response.GetType().GetProperty("Code")!.GetValue(response));
var results = (Dictionary<string, VariantValue>)response.GetType().GetProperty("Results")!.GetValue(response)!;
if (!results.TryGetValue("session_handle", out var handle)) return 2;
string session = handle.Type == VariantValueType.ObjectPath ? handle.GetObjectPathAsString() : handle.GetString();
Task closing;
{
using MessageWriter close = bus.GetMessageWriter();
close.WriteMethodCallHeader(destination: "org.freedesktop.portal.Desktop", path: session,
    @interface: "org.freedesktop.portal.Session", member: "Close");
closing = bus.CallMethodAsync(close.CreateMessage());
}
await closing.WaitAsync(TimeSpan.FromSeconds(5));
return 0;
