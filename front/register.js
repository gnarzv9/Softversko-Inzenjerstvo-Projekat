const registerBtn = document.getElementById("registerBtn");
const registerMessage = document.getElementById("registerMessage");

registerBtn.addEventListener("click", () => {
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value.trim();

    if (!username || !password) {
        registerMessage.textContent = "Please enter both username and password!";
        registerMessage.style.color = "red";
        return;
    }

    fetch("http://localhost:5000/api/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
    })
    .then(res => {
        if (!res.ok) throw new Error("Registration failed. User may exist.");
        return res.json();
    })
    .then(() => {
        return fetch("http://localhost:5000/api/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password })
        });
    })
    .then(res => {
        if (!res.ok) throw new Error("Login failed after registration.");
        return res.json();
    })
    .then(userData => {
        localStorage.setItem("user", JSON.stringify(userData));

        registerMessage.textContent = `Welcome ${userData.username}! Redirecting to shop...`;
        registerMessage.style.color = "green";
        setTimeout(() => {
            window.location.href = "index.html";
        }, 1000);
    })
    .catch(err => {
        registerMessage.textContent = err.message;
        registerMessage.style.color = "red";
    });
});