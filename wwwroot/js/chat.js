const connection = new signalR.HubConnectionBuilder()
    .withUrl("/commentshub")
    .build();

connection.start().catch(err => console.error(err));

document.getElementById("sendBtn").addEventListener("click", function () {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;

    connection.invoke("SendComment", user, message).catch(err => console.error(err));
});

connection.on("ReceiveComment", function (user, message) {
    const li = document.createElement("li");
    li.textContent = `${user}: ${message}`;
    document.getElementById("commentsList").appendChild(li);
});

// Загружаем сохраненные комментарии при загрузке страницы
fetch("/api/comments")
    .then(response => response.json())
    .then(data => {
        const list = document.getElementById("commentsList");
        data.forEach(comment => {
            const li = document.createElement("li");
            li.textContent = `${comment.user}: ${comment.message}`;
            list.appendChild(li);
        });
    });
document.getElementById("commentForm").addEventListener("submit", function (event) {
    event.preventDefault();
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;

    connection.invoke("SendComment", user, message)
        .catch(err => console.error(err.toString()));

    document.getElementById("messageInput").value = "";
});
    
