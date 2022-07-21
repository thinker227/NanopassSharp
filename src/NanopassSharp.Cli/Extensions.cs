using System.IO;
using System.Threading.Tasks;

namespace NanopassSharp;

public static class Extensions {

	public static string ReadAllText(this FileInfo file) {
		using var fs = file.OpenText();
		return fs.ReadToEnd();
	}
	public static async Task<string> ReadAllTextAsync(this FileInfo file) {
		using var fs = file.OpenText();
		return await fs.ReadToEndAsync();
	}

}
