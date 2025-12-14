// Simple script: smooth scroll for internal anchors
document.querySelectorAll('a[href^="#"]').forEach(a => {
    a.addEventListener('click', function (e) {
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            e.preventDefault();
            target.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
    })
});

//Cart Toast
document.querySelectorAll('.add-to-cart-btn').forEach(button => {
    button.addEventListener('click', function (event) {
        const toast = document.getElementById('cartToast');
        if (!toast) return;

        // Show toast
        toast.style.opacity = "1";
        toast.style.transform = "translateX(-50%) translateY(0)";

        // Hide after 3 seconds
        setTimeout(() => {
            toast.style.opacity = "0";
            toast.style.transform = "translateX(-50%) translateY(10px)";
        }, 120000);
    });
});


// small mobile nav enhancement could be added here if desired
document.addEventListener('DOMContentLoaded', () => {
    const hamburger = document.querySelector('.hamburger');
    const navList = document.querySelector('.nav-list ul');

    hamburger.addEventListener('click', () => {
        hamburger.classList.toggle('active');
        navList.classList.toggle('active');
    });

    // Optional: close menu when clicking a link
    document.querySelectorAll('.nav-list ul li a').forEach(link => {
        link.addEventListener('click', () => {
            hamburger.classList.remove('active');
            navList.classList.remove('active');
        });
    });
});

// Wait for page to load
document.addEventListener('DOMContentLoaded', function () {
    console.log('DOM loaded - updating cart count');
    updateCartCount();
});

function updateCartCount() {
    console.log('Fetching cart count...');

    // Fetch cart data from server
    fetch('/Home/GetCartSummary')
        .then(response => {
            console.log('Response status:', response.status);
            if (!response.ok) {
                throw new Error('Network response was not ok: ' + response.status);
            }
            return response.json();
        })
        .then(data => {
            console.log('Cart data received:', data);

            // Find the cart count element
            const cartCountElement = document.getElementById('cart-count');
            console.log('Cart element:', cartCountElement);

            if (cartCountElement) {
                // Update the count
                cartCountElement.textContent = data.totalItems;
                console.log('Cart count updated to:', data.totalItems);

                // Optional: Hide if empty
                if (data.totalItems === 0) {
                    cartCountElement.style.display = 'none';
                } else {
                    cartCountElement.style.display = 'inline';
                }
            } else {
                console.error('Cart count element not found!');
            }
        })
        .catch(error => {
            console.error('Error updating cart count:', error);
            // Show error in UI (optional)
            const cartCountElement = document.getElementById('cart-count');
            if (cartCountElement) {
                cartCountElement.textContent = '!';
                cartCountElement.title = 'Error loading cart count';
            }
        });
}



