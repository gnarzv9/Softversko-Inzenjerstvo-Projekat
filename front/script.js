const gamesContainer = document.getElementById("games");
const cartItemsContainer = document.getElementById("cart-items");
const checkoutBtn = document.getElementById("checkout-btn");

const cart = {};

// Fetch games from API
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

// Add game to cart
function addToCart(game) {
  if (cart[game.id]) {
    cart[game.id].quantity += 1;
  } else {
    cart[game.id] = { game: game, quantity: 1 };
  }
  renderCart();
}

// Remove game from cart
function removeFromCart(gameId) {
  if (cart[gameId]) {
    cart[gameId].quantity -= 1;
    if (cart[gameId].quantity <= 0) delete cart[gameId];
    renderCart();
  }
}

// Render cart items and toggle checkout button
function renderCart() {
  cartItemsContainer.innerHTML = "";

  const items = Object.values(cart);

  if (items.length === 0) {
    checkoutBtn.style.display = "none"; // hide button if cart empty
    return;
  }

  // Show checkout button if cart has items
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

// Checkout button click
checkoutBtn.addEventListener("click", () => {
  localStorage.setItem("cart", JSON.stringify(cart)); // save cart
  window.location.href = "checkout.html"; // go to checkout page
});