using NUnit.Framework;
using HUDConsole;

public class ConsoleTest {

	[Test]
	public void AddCommandTest() {
		Console.AddCommand("TestCommand", TestCommand, "This is a test command.");

		var testCommandFound = false;

		var commands = Console.GetOrderedCommands();
		foreach(var command in commands) {
			if(command.commandName == "TestCommand") {
				testCommandFound = true;
			}
		}
		Assert.IsTrue(testCommandFound);
	}

	private void TestCommand(string[] args) {

	}
}
