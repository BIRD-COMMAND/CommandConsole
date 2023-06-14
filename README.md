# Unity CommandConsole

Unity Command Console is a powerful in-editor command console for Unity. It allows developers to execute commands in real-time in the Unity editor. You can customize commands according to your needs. This repository provides a comprehensive framework to create, execute and manage commands in a simplified manner.
<!--![CommandConsole Screenshot](https://your-screenshot-url.png)-->

## Features

- Simple, intuitive, and easy to use
- Provides a mechanism for creating custom commands
- Supports long-running commands with coroutine-based implementations
- Automatic formatting of help text for commands

## Usage

1. Clone the repository or download the ZIP file and extract it.
2. Open the extracted folder in Unity.
3. Access the Command Console via the Unity editor by navigating to `Window -> Command Console`.

### Creating Custom Commands

To create a custom command, create a new class that inherits from `ConsoleCommand` and implement its abstract methods and properties:

- `Constructor`: The constructor just needs to pass the CommandConsole instance to the base class constructor.
- `Trigger`: This is the name of the command that will be typed into the console.
- `HelpText`: This text will be displayed when the `help` command is used.
- `ArgHelpText`: This text will be used to guide the user on what arguments (if any) the command accepts.
- `Execute`: This method will be called when your command is executed. Any arguments will be passed in as an array of strings.

```csharp
public class MyCommand : ConsoleCommand {
	
    public MyCommand(CommandConsole console) : base(console) { }
    
    public override string Trigger => "myCommand";
    public override string HelpText => "this custom command does something incredible";
    public override string ArgHelpText => "[argument1 description] [argument2 description]...";
    
    public override void Execute(params string[] args) { /* Command logic here */ }

}
```

## Contributing

Contributions are welcome! Feel free to create a new issue for bugs or feature requests, or create a new pull request to propose changes.

## License

This project is licensed under the GNU GPLv3.0 License. See [LICENSE](LICENSE) for more information.
