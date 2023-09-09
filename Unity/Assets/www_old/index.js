"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
const inputElement = document.getElementById("input");
const inputTextElement = document.getElementById("input-text");
// listen for command submissions
inputElement.addEventListener("submit", function (event) {
    event.preventDefault();
    sendCommand(inputTextElement.value);
});
// poll for console updates
window.setInterval(() => {
    checkForUpdates();
}, 500);
function sendCommand(command) {
    fetch("http://localhost:8080/", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ text: command })
    });
}
function checkForUpdates() {
    return __awaiter(this, void 0, void 0, function* () {
        const response = yield fetch("/api/console");
        const json = yield response.json();
        console.log(json);
        // const updates: string[] = await response.json();
        // console.log(updates);
        // updates.forEach((update: string): void => {
        //     console.log(update);
        // const newLine: HTMLParagraphElement = document.createElement("p");
        // newLine.innerText = update;
        // document.getElementById("console").appendChild(newLine);
        // });
    });
}
