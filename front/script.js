const gamesContainer = document.getElementById("games");
const cartItemsContainer = document.getElementById("cart-items");
const checkoutBtn = document.getElementById("checkout-btn");

const cart = {};

fetch("http://localhost:5000/api/games")
  .then(res => res.json())
  .then(games => {
    games.forEach(game => {
      const card = document.createElement("div");
      card.className = "game-card";
      card.innerHTML = `
        <img src="${game.imageUrl}">
        <h3>${game.name}</h3>
        <p>$${game.price}</p>
        <button>Buy</button>
      `;
      gamesContainer.appendChild(card);

      const button = card.querySelector("button");
      button.addEventListener("click", () => addToCart(game));
    });
  });

function addToCart(game) {
  if (cart[game.id]) {
    cart[game.id].quantity += 1;
  } else {
    cart[game.id] = { game: game, quantity: 1 };
  }
  renderCart();
}

function removeFromCart(gameId) {
  if (cart[gameId]) {
    cart[gameId].quantity -= 1;
    if (cart[gameId].quantity <= 0) delete cart[gameId];
    renderCart();
  }
}

function renderCart() {
  cartItemsContainer.innerHTML = "";

  const items = Object.values(cart);

  if (items.length === 0) {
    checkoutBtn.style.display = "none";
    return;
  }

  checkoutBtn.style.display = "block";

  items.forEach(item => {
    const itemDiv = document.createElement("div");
    itemDiv.textContent = `${item.game.name} - $${item.game.price} x${item.quantity}`;

    const removeBtn = document.createElement("button");
    removeBtn.textContent = "Remove";
    removeBtn.style.marginLeft = "10px";
    removeBtn.addEventListener("click", () => removeFromCart(item.game.id));

    itemDiv.appendChild(removeBtn);
    cartItemsContainer.appendChild(itemDiv);
  });
}

checkoutBtn.addEventListener("click", () => {
  localStorage.setItem("cart", JSON.stringify(cart)); 
  window.location.href = "checkout.html"; 
});

document.addEventListener("DOMContentLoaded", () => {
    const logRegDiv = document.querySelector(".logreg");

    if (!logRegDiv) return;

    const loginBtn = document.getElementById("loginPageBtn");
    const registerBtn = document.getElementById("registerPageBtn");

    let currentUser = JSON.parse(localStorage.getItem("user"));

    if (currentUser) {
        logRegDiv.innerHTML = `
            <span>Welcome, ${currentUser.username}</span>
            <button id="logoutBtn">Logout</button>
        `;
        document.getElementById("logoutBtn").addEventListener("click", () => {
            localStorage.removeItem("user");
            location.reload();
        });
    } else {
        if (loginBtn)
            loginBtn.addEventListener("click", () => {
                window.location.href = "login.html";
            });

        if (registerBtn)
            registerBtn.addEventListener("click", () => {
                window.location.href = "register.html";
            });
    }

});