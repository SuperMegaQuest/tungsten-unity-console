const inputElement: HTMLInputElement = document.getElementById("input") as HTMLInputElement;
const inputTextElement: HTMLInputElement = document.getElementById("input-text") as HTMLInputElement;

// listen for command submissions
inputElement.addEventListener("submit", function(event: SubmitEvent): void {
    event.preventDefault();
    sendCommand(inputTextElement.value);
});

// poll for console updates
window.setInterval((): void => {
    checkForUpdates();
}, 500);

function sendCommand(command: string): void {
    fetch("http://localhost:8080/", {
        method: "POST",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify({text: command})
    });
}

async function checkForUpdates(): Promise<void> {
    const response: Response = await fetch("/api/console");
    const json = await response.json();
    console.log(json);
    // const updates: string[] = await response.json();
    // console.log(updates);
    // updates.forEach((update: string): void => {
    //     console.log(update);
        // const newLine: HTMLParagraphElement = document.createElement("p");
        // newLine.innerText = update;
        // document.getElementById("console").appendChild(newLine);
    // });

}