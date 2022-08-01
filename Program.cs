using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

 // if args is empty write to the console explaining, and quit
 if (args.Length == 0)
 {
     Console.WriteLine("No arguments given");
     return;
 }
 
var initPrompt = string.Join(" ", args);

string prompt = null;

if (initPrompt.Contains("zsh"))
    prompt = $"single line, succinct, minimally verbose zshell/zsh command to {initPrompt.Replace("zsh", "")}:".Trim();
else if (initPrompt.Contains("bash"))
    prompt = $"single line, succinct, minimally verbose bash command to {initPrompt.Replace("bash", "")}:".Trim();
else // powershell is the default, reflecting the intransigent, unashamed blatant bias of the author of this code, Dr. Tim Scarfe. Flame away suckers. 
    prompt = $"single line, succinct, minimally verbose powershell command using aliases to {initPrompt}:";

prompt = prompt.Replace("\"", "");
    
// replace new lines in the summary with \n
prompt = prompt.Replace("\n", "\\n");

// I didn't make these configurable because to be honest
// so far, this "just works" for me. It's almost never wrong
var json = $@"{{
'stream' : true,
'model': 'text-davinci-002',
'prompt': '{prompt}',
'temperature': 0.3,
'max_tokens': 256,
'top_p': 0.7,
'frequency_penalty': 0,
'presence_penalty': 0 }}";

//replace line endings with nothing in json
json = json.Replace("\n", "")
    .Replace("'", "\"");

// load in const string key string from system environment variables
var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

//if key is null or empty or shorter than 10 characters, throw an exception and write error to console
if (string.IsNullOrEmpty(key) || key.Length < 10)
{
    Console.WriteLine("OpenAI API key not found in environment variables, set your OPENAI_API_KEY");
    Console.WriteLine("export OPENAI_API_KEY=<your_api_key>");
    
    return;
}

var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/completions");
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
request.Content = new StringContent(json, Encoding.UTF8, "application/json");

var client = new HttpClient();
var result = await client.SendAsync(request);
result.EnsureSuccessStatusCode(); // throw exception if not successful

var stream = await result.Content.ReadAsStreamAsync();

using var reader = new StreamReader(stream);

// turn AutoFlush to true for the console to write to the console immediately
Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
 
 while (true)
{
    var line = await reader.ReadLineAsync();

    if (line.Contains("[DONE]"))
        break;
    
    // if line is empty continue
    if (!string.IsNullOrEmpty(line))
    {
        // surround line with { and }
        line = $"{{{line}}}";
        
        // change encoding of line to UTF8
        line = Encoding.UTF8.GetString(Encoding.Default.GetBytes(line));

        var root = JsonConvert.DeserializeObject<dynamic>(line);

        if (root?.data != null)
        {
            Console.Write(root.data.choices[0].text);
            Thread.Sleep(30);
        }    
    } 
}