namespace Monolith {

/// <summary>
/// Contains information about each command.
/// </summary>
public struct ConsoleCommand {

#region Public
    public readonly string commandName;
    public readonly CommandHandler handler;
    public readonly string helpText;

    public ConsoleCommand(string commandName, CommandHandler handler, string helpText) {
        this.commandName = commandName;
        this.handler = handler;
        this.helpText = helpText;
    }
#endregion Public

}
}