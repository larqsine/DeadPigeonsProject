import React, { useState } from 'react';
import axios from 'axios';
import styles from './LoginPage.module.css';

interface LoginPageProps {
    onLogin: (username: string, roles: string[]) => void;
}

const LoginPage: React.FC<LoginPageProps> = ({ onLogin }) => {
    const [isRegistering, setIsRegistering] = useState(false);
    const [loginForm, setLoginForm] = useState({ username: '', password: '' });
    const [registerForm, setRegisterForm] = useState({
        name: '',
        phone: '',
        email: '',
        password: '',
    });

    const handleLoginChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setLoginForm((prev) => ({ ...prev, [name]: value }));
    };

    const handleRegisterChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setRegisterForm((prev) => ({ ...prev, [name]: value }));
    };

    const handleLoginSubmit = async () => {
        if (loginForm.username && loginForm.password) {
            try {
                const response = await axios.post('/api/account/login', {
                    email: loginForm.username,
                    password: loginForm.password,
                });

                const { user, roles } = response.data;
                alert(`Login successful! Welcome ${user}`);
                onLogin(user, roles); // Pass user details to parent component or state
            } catch (error) {
                console.error(error);
                alert('Login failed. Please check your credentials.');
            }
        } else {
            alert('Please enter username and password.');
        }
    };

    const handleRegisterSubmit = async () => {
        if (registerForm.name && registerForm.phone && registerForm.email && registerForm.password) {
            try {
                await axios.post('/api/account/register', {
                    fullName: registerForm.name,
                    phone: registerForm.phone,
                    email: registerForm.email,
                    password: registerForm.password,
                    role: 'Player', // Default role for registration
                });
                alert(`Registration successful!`);
                setIsRegistering(false); // Switch back to login view
            } catch (error) {
                console.error(error);
                alert('Registration failed. Please try again.');
            }
        } else {
            alert('Please fill all registration fields.');
        }
    };

    return (
        <div className={styles.container}>
            <img src="/logo.png" alt="App Logo" className={styles.logo} />
            <div className={styles.content}>
                <h1>{isRegistering ? 'Register' : 'Login'}</h1>
                {isRegistering ? (
                    <div className={styles.form}>
                        <input
                            type="text"
                            name="name"
                            placeholder="Name"
                            value={registerForm.name}
                            onChange={handleRegisterChange}
                        />
                        <input
                            type="text"
                            name="phone"
                            placeholder="Phone"
                            value={registerForm.phone}
                            onChange={handleRegisterChange}
                        />
                        <input
                            type="email"
                            name="email"
                            placeholder="Email"
                            value={registerForm.email}
                            onChange={handleRegisterChange}
                        />
                        <input
                            type="password"
                            name="password"
                            placeholder="Password"
                            value={registerForm.password}
                            onChange={handleRegisterChange}
                        />
                        <button onClick={handleRegisterSubmit}>Register</button>
                        <p>
                            Already have an account?{' '}
                            <span className={styles.link} onClick={() => setIsRegistering(false)}>
                                Login here
                            </span>
                        </p>
                    </div>
                ) : (
                    <div className={styles.form}>
                        <input
                            type="text"
                            name="username"
                            placeholder="Email"
                            value={loginForm.username}
                            onChange={handleLoginChange}
                        />
                        <input
                            type="password"
                            name="password"
                            placeholder="Password"
                            value={loginForm.password}
                            onChange={handleLoginChange}
                        />
                        <button onClick={handleLoginSubmit}>Login</button>
                        <p>
                            Don't have an account?{' '}
                            <span className={styles.link} onClick={() => setIsRegistering(true)}>
                                Register now
                            </span>
                        </p>
                    </div>
                )}
            </div>
        </div>
    );
};

export default LoginPage;
