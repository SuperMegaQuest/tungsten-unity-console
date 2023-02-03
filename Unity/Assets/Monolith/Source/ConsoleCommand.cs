namespace Gruel {

/// <summary>
///     Contains information about each command.
/// </summary>
public class ConsoleCommand {

#region Constructor
    public ConsoleCommand(string commandName, CommandHandler handler, string helpText) {
        CommandName = commandName;
        Handler = handler;
        HelpText = helpText;
    }
#endregion Constructor

#region Properties
    public string CommandName { get; }
    public CommandHandler Handler { get; }
    public string HelpText { get; }
#endregion Properties

}
}