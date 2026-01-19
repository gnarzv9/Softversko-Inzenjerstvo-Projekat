const loginBtn = document.getElementById("loginBtn");
const loginMessage = document.getElementById("loginMessage");

loginBtn.addEventListener("click", () => {
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value.trim();

    if (!username || !password) {
        loginMessage.textContent = "Please enter both username and password!";
        loginMessage.style.color = "red";
        return;
    }

    fetch("http://localhost:5000/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password })
    })
    .then(res => {
        if (!res.ok) throw new Error("Invalid username or password");
        return res.json();
    })
    .then(userData => {
        localStorage.setItem("user", JSON.stringify(userData));
        loginMessage.textContent = `Welcome ${userData.username}! Redirecting to shop...`;
        loginMessage.style.color = "green";

        setTimeout(() => {
            window.location.href = "index.html"; 
        }, 1000);
    })
    .catch(err => {
        loginMessage.textContent = err.message;
        loginMessage.style.color = "red";
    });
});