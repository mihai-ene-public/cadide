using System.Diagnostics;

namespace IDE.Core.Utilities
{
    public static class ProcessStarter
	{
		public static void Start(string fileName)
		{
			var psi = new ProcessStartInfo
			{
				FileName = fileName,
				UseShellExecute = true
			};
			Process.Start(psi);
		}
	}
}
