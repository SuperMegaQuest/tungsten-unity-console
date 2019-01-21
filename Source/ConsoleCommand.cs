namespace HUDConsole {
	
	/// <summary>
	/// Contains information about each command.
	/// </summary>
	public struct ConsoleCommand {
		public string commandName { get; private set; }
		public CommandHandler handler { get; private set; }
		public string helpText { get; private set; }
		
		public ConsoleCommand(string commandName, CommandHandler handler, string helpText) : this() {
			this.commandName = commandName;
			this.handler = handler;
			this.helpText = helpText;
		}
	}
	
}