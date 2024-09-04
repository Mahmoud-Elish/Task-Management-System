function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(String(email).toLowerCase());
}
function validateRegisterForm(username, email, password, confirmPassword, role) {
    if (!username || !email || !password || !confirmPassword || !role) {
        return "All fields are required.";
    }
    if (!validateEmail(email)) {
        return "Invalid email format.";
    }
    if (password !== confirmPassword) {
        return "Passwords do not match.";
    }
    if (password.length < 6) {
        return "Password must be at least 6 characters long.";
    }
    return null;
}
function validateLoginForm(email, password) {
    if (!email || !password) {
        return "Email and password are required.";
    }
    if (!validateEmail(email)) {
        return "Invalid email format.";
    }
    return null;
}
async function registerUser() {
    const username = document.getElementById("registerUsername").value;
    const email = document.getElementById("registerEmail").value;
    const password = document.getElementById("registerPassword").value;
    const confirmPassword = document.getElementById("confirmPassword").value;
    const role = document.getElementById("registerRole").value;
    const errorDiv = document.getElementById("registerError");

   
    const error = validateRegisterForm(username, email, password, confirmPassword, role);
    if (error) {
        errorDiv.textContent = error;
        return;
    }
    const data = {
        username: username,
        email: email,
        password: password,
        role: parseInt(role)
    };

    try {
        const response = await fetch('https://localhost:44337/api/auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });
        const result = await response.json();
        if (response.ok) {
            document.getElementById("registerForm").reset();
            errorDiv.style.color = "green"; 
            errorDiv.textContent = "registered successfully .. go to login";
        } else {
            errorDiv.style.color = "red"; 
            errorDiv.textContent = result.message;
        }
    } catch (error) {
        errorDiv.textContent = "An error occurred. Please try again.";
        console.error("Registration error:", error);
    }
}
async function loginUser() {
    const email = document.getElementById("loginEmail").value;
    const password = document.getElementById("loginPassword").value;
    const errorDiv = document.getElementById("loginError");

    const error = validateLoginForm(email, password);
    if (error) {
        errorDiv.textContent = error;
        return;
    }

    const data = {
        email: email,
        password: password
    };

    try {
        const response = await fetch('https://localhost:44337/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();
        if (response.ok) {
            localStorage.setItem('authToken', result.token);
            window.location.href = 'index.html'; 
        } else {
            errorDiv.textContent = "Invalid email or password.";
        }
    } catch (error) {
        errorDiv.textContent = "An error occurred. Please try again.";
        console.error("Login error:", error);
    }
}