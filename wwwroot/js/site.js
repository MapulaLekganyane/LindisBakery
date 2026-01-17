// ================================
// Smooth scroll for internal anchors
// ================================
document.querySelectorAll('a[href^="#"]').forEach(a => {
    a.addEventListener('click', function (e) {
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            e.preventDefault();
            target.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
    });
});


// ================================
// Mobile navigation (hamburger)
// ================================
document.addEventListener('DOMContentLoaded', () => {
    const hamburger = document.querySelector('.hamburger');
    const navList = document.querySelector('.nav-list ul');

    if (!hamburger || !navList) return;

    hamburger.addEventListener('click', () => {
        hamburger.classList.toggle('active');
        navList.classList.toggle('active');
    });

    document.querySelectorAll('.nav-list ul li a').forEach(link => {
        link.addEventListener('click', () => {
            hamburger.classList.remove('active');
            navList.classList.remove('active');
        });
    });
});


// ================================
// Cart badge update (SERVER-BASED)
// ================================
function updateCartCount(count) {
    const badge = document.getElementById("cart-count");
    if (!badge) return;

    if (count > 0) {
        badge.textContent = count;
        badge.style.display = "inline-block";
    } else {
        badge.style.display = "none";
    }
}


// ================================
// Load cart count on EVERY page load
// ================================
document.addEventListener("DOMContentLoaded", function () {
    fetch("/Home/GetCartSummary")
        .then(res => res.json())
        .then(data => {
            updateCartCount(data.totalItems);
        })
        .catch(() => {
            // Fail silently – cart still works
        });
});


// ================================
// Place order (checkout validation)
// ================================
function placeOrder() {
    const payment = document.querySelector('input[name="paymentMethod"]:checked');

    if (!payment) {
        alert("Please select a payment method.");
        return;
    }

    const selectedPayment = payment.value;

    if (selectedPayment === "Card") {
        alert("Redirecting to card payment...");
    } else {
        alert("Order placed with " + selectedPayment);
    }
}


// ================================
// ADD TO CART + TOAST + BADGE
// ================================
document.addEventListener("DOMContentLoaded", function () {

    const toast = document.getElementById("cartToast");
    let toastTimeout;

    if (!toast) return;

    document.querySelectorAll(".add-to-cart-btn").forEach(button => {
        button.addEventListener("click", async function () {

            const productId = this.dataset.productId;
            if (!productId) return;

            // ADD TO CART (AJAX)
            await fetch(`/Home/AddToCart?productId=${productId}`, {
                method: "POST"
            });

            // SHOW TOAST
            clearTimeout(toastTimeout);
            toast.style.opacity = "1";
            toast.style.transform = "translateX(-50%) translateY(0)";

            toastTimeout = setTimeout(() => {
                toast.style.opacity = "0";
                toast.style.transform = "translateX(-50%) translateY(10px)";
            }, 10000); // 10 seconds

            // UPDATE BADGE (SERVER TRUTH)
            const res = await fetch("/Home/GetCartSummary");
            const data = await res.json();
            updateCartCount(data.totalItems);
        });
    });
});

// ================================
// ADD TO CART + quanity
// ================================
document.addEventListener("click", function (e) {

    // PLUS button
    if (e.target.classList.contains("plus")) {
        const input = e.target.previousElementSibling;
        input.value = parseInt(input.value) + 1;
    }

    // MINUS button
    if (e.target.classList.contains("minus")) {
        const input = e.target.nextElementSibling;
        if (parseInt(input.value) > 1) {
            input.value = parseInt(input.value) - 1;
        }
    }

    // ADD TO CART
    if (e.target.closest(".add-to-cart-btn")) {
        const button = e.target.closest(".add-to-cart-btn");
        const container = button.closest(".cart-controls");
        const quantity = container.querySelector(".quantity-input").value;
        const productId = button.dataset.productId;

        addToCart(productId, quantity);
    }
});
