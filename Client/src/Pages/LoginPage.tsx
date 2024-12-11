import React from 'react';
import { useAtom } from 'jotai';
import axios from 'axios';
import styles from './LoginPage.module.css';
import { loginFormAtom, userAtom, isLoggedInAtom, playerIdAtom } from './PagesJotaiStore.ts';

interface LoginPageProps {
    onLogin: (email: string, roles: string[]) => void;
}

const LoginPage: React.FC<LoginPageProps> = ({ onLogin }) => {
    const [loginForm, setLoginForm] = useAtom(loginFormAtom);
    const [, setUser] = useAtom(userAtom);
    const [, setIsLoggedIn] = useAtom(isLoggedInAtom);
    const [, setPlayerId] = useAtom(playerIdAtom); // Use the correct playerIdAtom here

    const handleLoginChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setLoginForm((prev) => ({ ...prev, [name]: value }));
    };

    const handleLoginSubmit = async () => {
        if (loginForm.email && loginForm.password) {
            try {
                const response = await axios.post('http://localhost:6329/api/account/login', {
                    email: loginForm.email,
                    password: loginForm.password,
                });

                const { user, roles, token, playerId } = response.data;
                alert(`Login successful! Welcome ${user}`);

                // Set user data in userAtom
                setUser({ userName: user, roles, token });

                // Set playerId in playerIdAtom
                setPlayerId(playerId);

                // Mark the user as logged in
                setIsLoggedIn(true);

                onLogin(user, roles);
            } catch (error) {
                console.error("Login Error:", error);
                alert('Login failed. Please check your credentials.');
            }
        } else {
            alert('Please enter email and password.');
        }
    };

    return (
        <div className={styles.container}>
            <img src="/logo.png" alt="App Logo" className={styles.logo} />
            <div className={styles.content}>
                <h1>{'Login'}</h1>
                <div className={styles.form}>
                    <input
                        type="text"
                        name="email"
                        placeholder="Email"
                        value={loginForm.email}
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
                </div>
            </div>
        </div>
    );
};

export default LoginPage;
