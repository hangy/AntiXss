if (WScript.Arguments.Length != 1)
{
	WScript.Quit(1);
}
var WshShell = WScript.CreateObject("WScript.Shell");
var filename = WScript.Arguments(0)
var env = WshShell.Environment("Process");
var windir = env("SYSTEMROOT");
WshShell.Exec(windir + "\\Microsoft.NET\\Framework\\v2.0.50727\\ngen install " + filename);