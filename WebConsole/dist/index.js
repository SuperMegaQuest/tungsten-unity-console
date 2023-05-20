"use strict";
// console.log("Hello world");
// document.getElementById("input").addEventListener("submit", function (event: SubmitEvent): void {
//
// });
const inputElement = document.getElementById("input");
inputElement.addEventListener("submit", function (event) {
    event.preventDefault();
    const inputTextElement = document.getElementById("input-text");
    const inputText = inputTextElement.value;
    fetch("http://localhost:8080/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            text: inputText
        })
    });
});
