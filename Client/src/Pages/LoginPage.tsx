import React from 'react';
import { useAtom } from 'jotai';
import axios from 'axios';
import styles from './LoginPage.module.css';
import {loginFormAtom, userAtom, isLoggedInAtom, playerIdAtom, authAtom} from './PagesJotaiStore.ts';

interface LoginPageProps {
    onLogin: (email: string, roles: string[], passwordChangeRequired: boolean) => void;
}

const LoginPage: React.FC<LoginPageProps> = ({ onLogin }) => {
    const [loginForm, setLoginForm] = useAtom(loginFormAtom);
    const [, setUser] = useAtom(userAtom);
    const [, setIsLoggedIn] = useAtom(isLoggedInAtom);
    const [, setIsAdmin] = useAtom(playerIdAtom);
    const [, setAuth] = useAtom(authAtom);
    
    const handleLoginChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setLoginForm((prev) => ({ ...prev, [name]: value }));
    };

    const handleLoginSubmit = async () => {
        if (loginForm.email && loginForm.password) {
            try {
                const response = await axios.post('https://server-587187818392.europe-west1.run.app/api/account/login', {
                    email: loginForm.email,
                    password: loginForm.password,
                });

                const { userName, roles, token, passwordChangeRequired } = response.data;

                // Store the token in localStorage
                localStorage.setItem("token", token);
                setAuth(token);
                console.log("in loginPage  " + token)
                // Update user and roles in state
                setUser({ userName, roles, token, passwordChangeRequired });

                // Check if the user has an admin role
                const isAdmin = roles.includes("admin");
                setIsLoggedIn(true);
                setIsAdmin(isAdmin);

                // Redirect or update the UI accordingly
                onLogin(userName, roles, passwordChangeRequired);
            }
            catch (error) {
                if (axios.isAxiosError(error)) {
                    // Handle Axios errors
                    console.error("Login Error:", error.response?.data || error.message);
                    alert(error.response?.data?.message || "Login failed. Please check your credentials.");
                } else {
                    // Handle non-Axios errors
                    console.error("Unexpected Error:", error);
                    alert("An unexpected error occurred.");
                }
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
