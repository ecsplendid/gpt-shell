### GPT-3 SHELL

This code interacts with the OpenAI Completion API to provide autocomplete suggestions for bash, zsh, and powershell. This can be useful if you want to find a succinct way to do something in one of these shells. 

To use this code, you will need an OpenAI API key. You can obtain one by signing up for a free account at https://openai.com/. Once you have your key, set it as an environment variable on your system called OPENAI_API_KEY. 

With your key set, you can run the code by calling the program with a query argument. By default it will generate a powershell command, if you include the word "bash", "zsh" then it will produce commands for those shells respectively. 

## Instructions

I made a video explainer too

- Clone the code
- Build the software on your target architecture with `dotnet build`
- Set an alias in your profile i.e. `sal gpt "/Users/timscarfe/RiderProjects/GptShell/GptShell/bin/Debug/net6.0/GptShell"` for powershell in `$profile` or `alias gpt="/Users/timscarfe/RiderProjects/GptShell/GptShell/bin/Debug/net6.0/GptShell"` for zsh `~/.zprofile`
- Set environment variable i.e. `export OPENAI_API_KEY=<xxxx>`

Then run i.e. `gpt command to show the files in the git repo with xxx in the title`